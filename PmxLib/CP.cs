using System;
using System.Collections.Generic;

namespace PmxLib;

internal static class CP
{
	public static void Swap<T>(ref T v0, ref T v1) where T : struct
	{
		T val = v0;
		v0 = v1;
		v1 = val;
	}

	public static void Swap<T>(IList<T> list, int ix1, int ix2)
	{
		T value = list[ix1];
		list[ix1] = list[ix2];
		list[ix2] = value;
	}

	public static List<T> CloneList<T>(List<T> list) where T : ICloneable
	{
		List<T> list2 = new List<T>();
		int count = list.Count;
		for (int i = 0; i < count; i++)
		{
			list2.Add((T)list[i].Clone());
		}
		return list2;
	}

	public static List<T> CloneList_ValueType<T>(List<T> list) where T : struct
	{
		return new List<T>(list.ToArray());
	}

	public static T[] CloneArray<T>(T[] src) where T : ICloneable
	{
		T[] array = new T[src.Length];
		for (int i = 0; i < src.Length; i++)
		{
			array[i] = (T)src[i].Clone();
		}
		return array;
	}

	public static T[] CloneArray_ValueType<T>(T[] src) where T : struct
	{
		T[] array = new T[src.Length];
		Array.Copy(src, array, src.Length);
		return array;
	}

	public static Dictionary<Tv, int> ArrayToTable<Tl, Tv>(Tl[] arr, Func<int, Tv> objProc)
	{
		Dictionary<Tv, int> dictionary = new Dictionary<Tv, int>(arr.Length);
		for (int i = 0; i < arr.Length; i++)
		{
			Tv val = objProc(i);
			if (val != null && !dictionary.ContainsKey(val))
			{
				dictionary.Add(val, i);
			}
		}
		return dictionary;
	}

	public static Dictionary<T, int> ArrayToTable<T>(T[] arr)
	{
		Dictionary<T, int> dictionary = new Dictionary<T, int>(arr.Length);
		for (int i = 0; i < arr.Length; i++)
		{
			T val = arr[i];
			if (val != null && !dictionary.ContainsKey(val))
			{
				dictionary.Add(val, i);
			}
		}
		return dictionary;
	}

	public static Dictionary<Tv, int> ListToTable<Tl, Tv>(List<Tl> list, Func<int, Tv> objProc)
	{
		Dictionary<Tv, int> dictionary = new Dictionary<Tv, int>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			Tv val = objProc(i);
			if (val != null && !dictionary.ContainsKey(val))
			{
				dictionary.Add(val, i);
			}
		}
		return dictionary;
	}

	public static Dictionary<T, int> ListToTable<T>(List<T> list)
	{
		Dictionary<T, int> dictionary = new Dictionary<T, int>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			T val = list[i];
			if (val != null && !dictionary.ContainsKey(val))
			{
				dictionary.Add(val, i);
			}
		}
		return dictionary;
	}

	public static bool InRange<T>(T[] arr, int index)
	{
		if (0 <= index)
		{
			return index < arr.Length;
		}
		return false;
	}

	public static bool InRange<T>(List<T> list, int index)
	{
		if (0 <= index)
		{
			return index < list.Count;
		}
		return false;
	}

	public static bool InRange<T>(IList<T> list, int index)
	{
		if (0 <= index)
		{
			return index < list.Count;
		}
		return false;
	}

	public static bool InRange(int min, int max, int val)
	{
		if (min <= val)
		{
			return val <= max;
		}
		return false;
	}

	public static T SafeGet<T>(T[] arr, int index) where T : class
	{
		if (arr != null && InRange(arr, index))
		{
			return arr[index];
		}
		return null;
	}

	public static T SafeGet<T>(IList<T> arr, int index) where T : class
	{
		if (arr != null && InRange(arr, index))
		{
			return arr[index];
		}
		return null;
	}

	public static T SafeGetV<T>(T[] arr, int index) where T : struct
	{
		if (arr != null && InRange(arr, index))
		{
			return arr[index];
		}
		return default(T);
	}

	public static T SafeGetV<T>(T[] arr, int index, out bool flag) where T : struct
	{
		flag = false;
		if (arr != null && InRange(arr, index))
		{
			flag = true;
			return arr[index];
		}
		return default(T);
	}

	public static T SafeGetV<T>(IList<T> arr, int index) where T : struct
	{
		if (arr != null && InRange(arr, index))
		{
			return arr[index];
		}
		return default(T);
	}

	public static T SafeGetV<T>(IList<T> arr, int index, out bool flag) where T : struct
	{
		flag = false;
		if (arr != null && InRange(arr, index))
		{
			flag = true;
			return arr[index];
		}
		return default(T);
	}

	public static int[] SortIndexForRemove(int[] ix)
	{
		List<int> list = new List<int>(ix);
		list.Sort((int l, int r) => r - l);
		return list.ToArray();
	}

	public static void SSort<T>(List<T> list, Comparison<T> comp)
	{
		List<KeyValuePair<int, T>> list2 = new List<KeyValuePair<int, T>>(list.Count);
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(new KeyValuePair<int, T>(i, list[i]));
		}
		list2.Sort(delegate(KeyValuePair<int, T> x, KeyValuePair<int, T> y)
		{
			int num = comp(x.Value, y.Value);
			if (num == 0)
			{
				num = x.Key.CompareTo(y.Key);
			}
			return num;
		});
		for (int j = 0; j < list.Count; j++)
		{
			list[j] = list2[j].Value;
		}
	}

	public static int[] ComposeIndices(int[] ix1, int[] ix2)
	{
		List<int> list = new List<int>(ix1);
		Dictionary<int, int> dictionary = new Dictionary<int, int>(ix1.Length);
		for (int i = 0; i < ix1.Length; i++)
		{
			dictionary.Add(ix1[i], 0);
		}
		for (int j = 0; j < ix2.Length; j++)
		{
			if (!dictionary.ContainsKey(ix2[j]))
			{
				dictionary.Add(ix2[j], 0);
				list.Add(ix2[j]);
			}
		}
		return list.ToArray();
	}

	public static int[] RemoveIndices(int[] ix1, int[] ix2)
	{
		List<int> list = new List<int>(ix1.Length);
		Dictionary<int, int> dictionary = new Dictionary<int, int>(ix2.Length);
		for (int i = 0; i < ix2.Length; i++)
		{
			dictionary.Add(ix2[i], 0);
		}
		for (int j = 0; j < ix1.Length; j++)
		{
			if (!dictionary.ContainsKey(ix1[j]))
			{
				dictionary.Add(ix1[j], 0);
				list.Add(ix1[j]);
			}
		}
		return list.ToArray();
	}

	public static bool IsSameIndices(int[] arr1, int[] arr2)
	{
		if (arr1.Length != arr2.Length)
		{
			return false;
		}
		for (int i = 0; i < arr1.Length; i++)
		{
			if (arr1[i] != arr2[i])
			{
				return false;
			}
		}
		return true;
	}
}
