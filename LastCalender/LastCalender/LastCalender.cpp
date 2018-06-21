// LastCalender.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "seek.h"
#include "LastCalender.h"

namespace LastCalender
{
	Seek *seek;

	void __stdcall init(int now_year)
	{
		if (now_year > 0)
			seek = new Seek(now_year);
		else
			seek = new Seek();
	}

	void __stdcall read_festival(int month, int day, int is_festival)
	{
		seek->read_festival(month, day, is_festival);
	}

	void __stdcall read_vacation(int start_month, int start_day, int end_month, int end_day, bool is_summer)
	{
		seek->read_vacation(start_month, start_day, end_month, end_day, is_summer);
	}

	void __stdcall calcu_holiday()
	{
		seek->holiday_in_year();
	}

	bool __stdcall seek_status(int month, int day)
	{
		return seek->seek_in_date(month, day);
	}

	int __stdcall count_holiday()
	{
		return seek->count_holiday_in_year();
	}

	void __stdcall end_dll()
	{
		delete seek;
	}

	// methods for test dll

	int __stdcall test_year()
	{
		return seek->test_year();
	}

	int __stdcall test_festival()
	{
		return seek->test_festival();
	}

	int __stdcall test_vacation()
	{
		return seek->test_vacation();
	}
}
