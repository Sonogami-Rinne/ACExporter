using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

public static class KKBPExtensions
{
	public static bool TryGetVariable<T, T2>(this Type obj, string variableName, T instance, out T2 variable)
	{
		variable = default(T2);
		FieldInfo fieldInfo = AccessTools.Field(obj, variableName);
		if (fieldInfo != null)
		{
			variable = (T2)fieldInfo.GetValue(instance);
		}
		if (variable == null)
		{
			PropertyInfo propertyInfo = AccessTools.Property(obj, variableName);
			if (propertyInfo != null)
			{
				variable = (T2)propertyInfo.GetValue(instance, null);
			}
		}
		bool result = variable != null;
		Console.WriteLine(obj.ToString() + ": " + variableName + " | Has variable?: " + result);
		return result;
	}

	public static Vector3 TransformPointUnscaled(this Transform transform, Vector3 position)
	{
		return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).MultiplyPoint3x4(position);
	}

	public static Vector3 InverseTransformPointUnscaled(this Transform transform, Vector3 position)
	{
		return Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one).inverse.MultiplyPoint3x4(position);
	}
}
