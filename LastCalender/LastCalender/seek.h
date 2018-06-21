#pragma once

class __declspec(dllexport) Vacation
{
public:
	int from_month, from_day;
	int end_month, end_day;
};

class __declspec(dllexport) StatusDay
{
public:
	static const int ILLEGAL = -1;		// illegal
	StatusDay();
	StatusDay(int status);
	~StatusDay();
	void setstatus(int new_status);
	int getstatus();
private:
	int status;
	// 0 work day
	// 1 work but holiday
	// 2 holiday: 6, 7
	// 3 holiday: festival
	// 4 holiday: summer, winter
};

class __declspec(dllexport) Seek
{
public:
	static const int DAYS_YEAR = 366;
	Seek();
	Seek(int year);
	~Seek();
	void read_festival(int month, int day, int festival);	// 0 work 1 weekend 2 festival
	void read_vacation(int start_month, int start_day, int end_month, int end_day, bool is_summer);
	bool seek_in_date(int month, int day);		// serch the status of this day.
	int index_in_year(int year, int month, int day);	// get the index in this year of this day.
	void holiday_in_year();		// caculate holidays in this year, and save in status_in_year.
	int count_holiday_in_year();	// count holidays in the year.

	int test_year();		// for test
	int test_festival();
	int test_vacation();

private:
	int year;
	int sum_month[13];		// sum pre month
	bool leap_year;

	int festival_in_year[DAYS_YEAR];	// read info festival  0 work 1 weekend 2 festival
	Vacation vacation_summer, vacation_winter;	// summer, winter vacation

	StatusDay status_in_year[DAYS_YEAR];	// for seek

	void is_leap_year();
	void get_sum_month();
	int kim_larsson(int y, int m, int d);
};