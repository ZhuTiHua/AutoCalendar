#pragma once

#include "stdafx.h"

namespace LastCalender
{
	extern "C" __declspec(dllexport) void __stdcall init(int now_year);
	extern "C" __declspec(dllexport) void __stdcall read_festival(int month, int day, int is_festival);
	extern "C" __declspec(dllexport) void __stdcall read_vacation(int start_month, int start_day, int end_month, int end_day, bool is_summer);
	extern "C" __declspec(dllexport) void __stdcall calcu_holiday();

	extern "C" __declspec(dllexport) bool __stdcall seek_status(int month, int day);
	extern "C" __declspec(dllexport) int __stdcall count_holiday();
	extern "C" __declspec(dllexport) void __stdcall end_dll();

	// methods for test dll
	extern "C" __declspec(dllexport) int __stdcall test_year();
	extern "C" __declspec(dllexport) int __stdcall test_festival();
	extern "C" __declspec(dllexport) int __stdcall test_vacation();
}