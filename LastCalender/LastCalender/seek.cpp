#include "stdafx.h"
#include "seek.h"
#include <cstring>

StatusDay::StatusDay()
{
	setstatus(0);
}

StatusDay::StatusDay(int status)
{
	setstatus(status);	// init the status of this day.
}

void StatusDay::setstatus(int nowstatus)
{
	status = nowstatus;
}

int StatusDay::getstatus()
{
	if (status >= 0 && status <= 4) return status;
	else return ILLEGAL;
}

StatusDay::~StatusDay()
{
}


Seek::Seek()
{
	std::memset(festival_in_year, 0, sizeof(festival_in_year));
	year = 2000;
	is_leap_year();
	get_sum_month();
}

Seek::Seek(int now_year)
{
	std::memset(festival_in_year, 0, sizeof(festival_in_year));
	year = now_year;
	is_leap_year();
	get_sum_month();
}

void Seek::read_festival(int month, int day, int festival)	// 2. read festival
{
	switch (festival)
	{
	case 0:
		festival_in_year[index_in_year(year, month, day)] = 0;
		break;
	case 1:
		festival_in_year[index_in_year(year, month, day)] = 1;
		break;
	case 2:
		festival_in_year[index_in_year(year, month, day)] = 2;
		break;
	default:
		break;
	}
}

void Seek::read_vacation(int start_month, int start_day, int end_month, int end_day, bool is_summer)  // 3. read summer/winter
{
	if (is_summer)
	{
		vacation_summer.from_day = start_day;
		vacation_summer.from_month = start_month;
		vacation_summer.end_day = end_day;
		vacation_summer.end_month = end_month;
	}
	else
	{
		vacation_winter.from_day = start_day;
		vacation_winter.from_month = start_month;
		vacation_winter.end_day = end_day;
		vacation_winter.end_month = end_month;
	}
}

int Seek::index_in_year(int year, int month, int day)
{
	return sum_month[month - 1] + day - 1;
}

void Seek::holiday_in_year()	// 4. caculate
{
	for (int i = 0; i < DAYS_YEAR; i++)
	{
		status_in_year[i].setstatus(0);
	}

	// summer/winter vacation -- 4
	int index_summer_start = index_in_year(year, vacation_summer.from_month, vacation_summer.from_day);
	int index_summer_end = index_in_year(year, vacation_summer.end_month, vacation_summer.end_day);
	int index_winter_start = index_in_year(year, vacation_winter.from_month, vacation_winter.from_day);
	int index_winter_end = index_in_year(year, vacation_winter.end_month, vacation_winter.end_day);
	for (int i = index_summer_start; i <= index_summer_end; i++)
	{
		status_in_year[i].setstatus(4);
	}
	for (int i = index_winter_start; i <= index_winter_end; i++)
	{
		status_in_year[i].setstatus(4);
	}

	// work/holiday 1/2
	int index_this_year_start = index_in_year(year, 1, 1);
	int week = kim_larsson(year, 1, 1);
	int index_this_year_end = index_in_year(year, 12, 31);
	while (festival_in_year[index_this_year_start] != 0)	// handle new year
	{
		status_in_year[index_this_year_start].setstatus(3);
		index_this_year_start++;
		if (week == 6) week = 0;
		else week++;
	}
	int num_work_week = 0;
	for (int i = index_this_year_start; i <= index_this_year_end; i++)
	{
		if (i >= index_summer_start && i <= index_summer_end)
		{
			num_work_week = 0;
			if (week == 6) week = 0;
			else week++;
			continue;
		}
		if (i >= index_winter_start && i <= index_winter_end)
		{
			num_work_week = 0;
			if (week == 6) week = 0;
			else week++;
			continue;
		}

		if (festival_in_year[i] == 1)	// weekend
		{
			if (num_work_week == 3)		// rest 2 days
			{
				bool next_week_fes = true;
				int days;
				if (week) days = 8;
				else days = 7;

				for (int j = i; j <= i + days; j++)
				{
					if (festival_in_year[j] == 2)
					{
						next_week_fes = false;
						break;
					}
				}

				if (next_week_fes) status_in_year[i].setstatus(2);
				else status_in_year[i].setstatus(1);
				if (!week) num_work_week = 0;
			}
			else		// rest only sunday
			{
				if (!week) num_work_week++;
				if (week == 6)
				{
					status_in_year[i].setstatus(1);		// work
				}
				else
				{
					status_in_year[i].setstatus(2);		// rest
				}
			}
		}
		else if (festival_in_year[i] == 2)
		{
			status_in_year[i].setstatus(3);  // festival -- 3
			num_work_week = 0;
		}

		if (week == 6) week = 0;
		else week++;
	}
}

bool Seek::seek_in_date(int month, int day)	// 5. seek
{
	if (status_in_year[index_in_year(year, month, day)].getstatus() > 1)
		return true;
	else
		return false;
}

int Seek::count_holiday_in_year()
{
	int ans = 0;
	for (int i = 0; i < DAYS_YEAR; i++)
	{
		if (status_in_year[i].getstatus() > 1) ans++;
	}
	return ans;
}

int Seek::test_year()
{
	return year;
}

int Seek::test_festival()
{
	int ans = 0;
	for (int i = 0; i < DAYS_YEAR; i++)
	{
		if (festival_in_year[i]) ans++;
	}
	return ans;
}

int Seek::test_vacation()
{
	return vacation_summer.end_day;
}

void Seek::is_leap_year()
{
	if (year % 400 == 0) leap_year = true;
	else if (year % 100 != 0 && year % 4 == 0) leap_year = true;
	else leap_year = false;
}

int Seek::kim_larsson(int y, int m, int d)		// 0 -- 6 £ºsun - sat
{
	if (m == 1 || m == 2)
	{
		m += 12;
		y--;
	}
	int c = y / 100;
	y = y % 100;
	int w = c / 4 - 2 * c + y + y / 4 + 26 * (m + 1) / 10 + d - 1;
	if (w < 0) return (w + (-w / 7 + 1) * 7) % 7;
	return w % 7;
}

void Seek::get_sum_month()
{
	int feb;
	if (leap_year) feb = 29;
	else feb = 28;
	std::memset(sum_month, 0, sizeof(sum_month));
	sum_month[1] = 31;
	sum_month[2] = sum_month[1] + feb;
	for (int i = 3; i <= 12; i++)
	{
		if (i == 4 || i == 6 || i == 9 || i == 11)
			sum_month[i] = sum_month[i - 1] + 30;
		else
			sum_month[i] = sum_month[i - 1] + 31;
	}
}

Seek::~Seek()
{
}
