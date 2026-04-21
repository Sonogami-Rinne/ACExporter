using System;

namespace PmxLib;

internal static class CMath
{
	public static float CrossVector2(Vector2 p, Vector2 q)
	{
		return (float)((double)p.X * (double)q.Y - (double)p.Y * (double)q.X);
	}

	public static Vector2 NormalizeValue(Vector2 val)
	{
		if (float.IsNaN(val.X))
		{
			val.X = 0f;
		}
		if (float.IsNaN(val.Y))
		{
			val.Y = 0f;
		}
		return val;
	}

	public static Vector3 NormalizeValue(Vector3 val)
	{
		if (float.IsNaN(val.X))
		{
			val.X = 0f;
		}
		if (float.IsNaN(val.Y))
		{
			val.Y = 0f;
		}
		if (float.IsNaN(val.Z))
		{
			val.Z = 0f;
		}
		return val;
	}

	public static Vector4 NormalizeValue(Vector4 val)
	{
		if (float.IsNaN(val.X))
		{
			val.X = 0f;
		}
		if (float.IsNaN(val.Y))
		{
			val.Y = 0f;
		}
		if (float.IsNaN(val.Z))
		{
			val.Z = 0f;
		}
		if (float.IsNaN(val.W))
		{
			val.W = 0f;
		}
		return val;
	}

	public static bool GetIntersectPoint(Vector2 p0, Vector2 p1, Vector2 q0, Vector2 q1, ref Vector2 rate, ref Vector2 pos)
	{
		Vector2 vector = p1 - p0;
		Vector2 q2 = q1 - q0;
		float num = CrossVector2(vector, q2);
		if ((double)num == 0.0)
		{
			return false;
		}
		Vector2 p2 = q0 - p0;
		float num2 = CrossVector2(p2, vector);
		float num3 = CrossVector2(p2, q2);
		float num4 = num2 / num;
		float num5 = num3 / num;
		if ((double)num5 + 9.99999974737875E-06 < 0.0 || (double)num5 - 9.99999974737875E-06 > 1.0 || (double)num4 + 9.99999974737875E-06 < 0.0 || (double)num4 - 9.99999974737875E-06 > 1.0)
		{
			return false;
		}
		rate = new Vector2(num5, num4);
		pos = p0 + vector * num5;
		return true;
	}

	public static Vector3 GetFaceNormal(Vector3 p0, Vector3 p1, Vector3 p2)
	{
		Vector3 result = Vector3.Cross(p1 - p0, p2 - p0);
		result.Normalize();
		return result;
	}

	public static Vector3 GetFaceOrigin(Vector3 p0, Vector3 p1, Vector3 p2)
	{
		return (p0 + p1 + p2) / 3f;
	}

	public static Vector3 GetTriangleIntersect(Vector3 org, Vector3 dir, Vector3 v0, Vector3 v1, Vector3 v2)
	{
		Vector3 result = new Vector3(-1f, 0f, 0f);
		Vector3 vector = v1 - v0;
		Vector3 vector2 = v2 - v0;
		Vector3 rhs = Vector3.Cross(dir, vector2);
		float num = Vector3.Dot(vector, rhs);
		float num2;
		Vector3 rhs2;
		float num3;
		if ((double)num > 0.001)
		{
			Vector3 lhs = org - v0;
			num2 = Vector3.Dot(lhs, rhs);
			if ((double)num2 < 0.0 || (double)num2 > (double)num)
			{
				return result;
			}
			rhs2 = Vector3.Cross(lhs, vector);
			num3 = Vector3.Dot(dir, rhs2);
			if ((double)num3 < 0.0 || (double)num2 + (double)num3 > (double)num)
			{
				return result;
			}
		}
		else
		{
			if ((double)num >= -0.001)
			{
				return result;
			}
			Vector3 lhs2 = org - v0;
			num2 = Vector3.Dot(lhs2, rhs);
			if ((double)num2 > 0.0 || (double)num2 < (double)num)
			{
				return result;
			}
			rhs2 = Vector3.Cross(lhs2, vector);
			num3 = Vector3.Dot(dir, rhs2);
			if ((double)num3 > 0.0 || (double)num2 + (double)num3 < (double)num)
			{
				return result;
			}
		}
		float num4 = 1f / num;
		float x = Vector3.Dot(vector2, rhs2) * num4;
		float y = num2 * num4;
		float z = num3 * num4;
		result.X = x;
		result.Y = y;
		result.Z = z;
		return result;
	}

	public static Vector3 GetLineCrossPoint(Vector3 p, Vector3 from, Vector3 dir, out float d)
	{
		Vector3 rhs = p - from;
		d = Vector3.Dot(dir, rhs);
		return dir * d + from;
	}

	public static Vector3 GetLineCrossPoint(Vector3 p, Vector3 from, Vector3 dir)
	{
		float d;
		return GetLineCrossPoint(p, from, dir, out d);
	}

	public static Matrix CreateViewportMatrix(int width, int height)
	{
		Matrix identity = Matrix.Identity;
		float num = (float)width * 0.5f;
		float num2 = (float)height * 0.5f;
		identity.M11 = num;
		identity.M22 = 0f - num2;
		identity.M41 = num;
		identity.M42 = num2;
		return identity;
	}

	public static bool InArcPosition(Vector3 pos, float cx, float cy, float r2, out float d2)
	{
		float num = pos.X - cx;
		float num2 = pos.Y - cy;
		d2 = (float)((double)num * (double)num + (double)num2 * (double)num2);
		return (double)d2 <= (double)r2;
	}

	public static bool InArcPosition(Vector3 pos, float cx, float cy, float r2)
	{
		float d;
		return InArcPosition(pos, cx, cy, r2, out d);
	}

	public static void NormalizeRotateXYZ(ref Vector3 v)
	{
		if ((double)v.X < -3.14159274101257)
		{
			v.X += 6.283185f;
		}
		else if ((double)v.X > 3.14159274101257)
		{
			v.X -= 6.283185f;
		}
		if ((double)v.Y < -3.14159274101257)
		{
			v.Y += 6.283185f;
		}
		else if ((double)v.Y > 3.14159274101257)
		{
			v.Y -= 6.283185f;
		}
		if ((double)v.Z < -3.14159274101257)
		{
			v.Z += 6.283185f;
		}
		else if ((double)v.Z > 3.14159274101257)
		{
			v.Z -= 6.283185f;
		}
	}

	public static Vector3 MatrixToEuler_ZXY0(ref Matrix m)
	{
		Vector3 zero = Vector3.Zero;
		if ((double)m.M32 == 1.0)
		{
			zero.X = 1.570796f;
			zero.Z = (float)Math.Atan2(m.M21, m.M11);
		}
		else if ((double)m.M32 == -1.0)
		{
			zero.X = -1.570796f;
			zero.Z = (float)Math.Atan2(m.M21, m.M11);
		}
		else
		{
			zero.X = 0f - (float)Math.Asin(m.M32);
			zero.Y = 0f - (float)Math.Atan2(0.0 - (double)m.M31, m.M33);
			zero.Z = 0f - (float)Math.Atan2(0.0 - (double)m.M12, m.M22);
		}
		return zero;
	}

	public static Vector3 MatrixToEuler_ZXY(ref Matrix m)
	{
		Vector3 zero = Vector3.Zero;
		zero.X = 0f - (float)Math.Asin(m.M32);
		if ((double)zero.X == 1.57079637050629 || (double)zero.X == -1.57079637050629)
		{
			zero.Y = (float)Math.Atan2(0.0 - (double)m.M13, m.M11);
		}
		else
		{
			zero.Y = (float)Math.Atan2(m.M31, m.M33);
			zero.Z = (float)Math.Asin((double)m.M12 / Math.Cos(zero.X));
			if ((double)m.M22 < 0.0)
			{
				zero.Z = 3.141593f - zero.Z;
			}
		}
		return zero;
	}

	public static Vector3 MatrixToEuler_XYZ(ref Matrix m)
	{
		Vector3 zero = Vector3.Zero;
		zero.Y = 0f - (float)Math.Asin(m.M13);
		if ((double)zero.Y == 1.57079637050629 || (double)zero.Y == -1.57079637050629)
		{
			zero.Z = (float)Math.Atan2(0.0 - (double)m.M21, m.M22);
		}
		else
		{
			zero.Z = (float)Math.Atan2(m.M12, m.M11);
			zero.X = (float)Math.Asin((double)m.M23 / Math.Cos(zero.Y));
			if ((double)m.M33 < 0.0)
			{
				zero.X = 3.141593f - zero.X;
			}
		}
		return zero;
	}

	public static Vector3 MatrixToEuler_YZX(ref Matrix m)
	{
		Vector3 zero = Vector3.Zero;
		zero.Z = 0f - (float)Math.Asin(m.M21);
		if ((double)zero.Z == 1.57079637050629 || (double)zero.Z == -1.57079637050629)
		{
			zero.X = (float)Math.Atan2(0.0 - (double)m.M32, m.M33);
		}
		else
		{
			zero.X = (float)Math.Atan2(m.M23, m.M22);
			zero.Y = (float)Math.Asin((double)m.M31 / Math.Cos(zero.Z));
			if ((double)m.M11 < 0.0)
			{
				zero.Y = 3.141593f - zero.Y;
			}
		}
		return zero;
	}

	public static Vector3 MatrixToEuler_ZXY_Lim2(ref Matrix m)
	{
		Vector3 zero = Vector3.Zero;
		zero.X = 0f - (float)Math.Asin(m.M32);
		if ((double)zero.X == 1.57079637050629 || (double)zero.X == -1.57079637050629)
		{
			zero.Y = (float)Math.Atan2(0.0 - (double)m.M13, m.M11);
		}
		else
		{
			if (1.53588974475861 < (double)zero.X)
			{
				zero.X = (float)Math.PI * 22f / 45f;
			}
			else if ((double)zero.X < -1.53588974475861)
			{
				zero.X = (float)Math.PI * -22f / 45f;
			}
			zero.Y = (float)Math.Atan2(m.M31, m.M33);
			zero.Z = (float)Math.Asin((double)m.M12 / Math.Cos(zero.X));
			if ((double)m.M22 < 0.0)
			{
				zero.Z = 3.141593f - zero.Z;
			}
		}
		return zero;
	}

	public static Vector3 MatrixToEuler_XYZ_Lim2(ref Matrix m)
	{
		Vector3 zero = Vector3.Zero;
		zero.Y = 0f - (float)Math.Asin(m.M13);
		if ((double)zero.Y == 1.57079637050629 || (double)zero.Y == -1.57079637050629)
		{
			zero.Z = (float)Math.Atan2(0.0 - (double)m.M21, m.M22);
		}
		else
		{
			if (1.53588974475861 < (double)zero.Y)
			{
				zero.Y = (float)Math.PI * 22f / 45f;
			}
			else if ((double)zero.Y < -1.53588974475861)
			{
				zero.Y = (float)Math.PI * -22f / 45f;
			}
			zero.Z = (float)Math.Atan2(m.M12, m.M11);
			zero.X = (float)Math.Asin((double)m.M23 / Math.Cos(zero.Y));
			if ((double)m.M33 < 0.0)
			{
				zero.X = 3.141593f - zero.X;
			}
		}
		return zero;
	}

	public static Vector3 MatrixToEuler_YZX_Lim2(ref Matrix m)
	{
		Vector3 zero = Vector3.Zero;
		zero.Z = 0f - (float)Math.Asin(m.M21);
		if ((double)zero.Z == 1.57079637050629 || (double)zero.Z == -1.57079637050629)
		{
			zero.X = (float)Math.Atan2(0.0 - (double)m.M32, m.M33);
		}
		else
		{
			if (1.53588974475861 < (double)zero.Z)
			{
				zero.Z = (float)Math.PI * 22f / 45f;
			}
			else if ((double)zero.Z < -1.53588974475861)
			{
				zero.Z = (float)Math.PI * -22f / 45f;
			}
			zero.X = (float)Math.Atan2(m.M23, m.M22);
			zero.Y = (float)Math.Asin((double)m.M31 / Math.Cos(zero.Z));
			if ((double)m.M11 < 0.0)
			{
				zero.Y = 3.141593f - zero.Y;
			}
		}
		return zero;
	}
}
