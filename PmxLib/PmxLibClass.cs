using System;

namespace PmxLib;

internal static class PmxLibClass
{
	private static bool m_lock;

	public static void Unlock(string key)
	{
		m_lock = true;
		if (key == RString(-167698971, "UnlockPmxLibClass"))
		{
			m_lock = false;
		}
	}

	public static bool IsLocked()
	{
		return m_lock;
	}

	public static string RString(int s, string str)
	{
		Random random = new Random(s);
		char[] array = str.ToCharArray();
		int num = array.Length;
		while (num > 1)
		{
			num--;
			int num2 = random.Next(num + 1);
			char c = array[num2];
			array[num2] = array[num];
			array[num] = c;
		}
		return new string(array);
	}
}
