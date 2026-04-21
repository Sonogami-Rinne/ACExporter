using System;

namespace PmxLib;

public struct Vector3
{
	public float x;

	public float y;

	public float z;

	public float X
	{
		get
		{
			return x;
		}
		set
		{
			x = value;
		}
	}

	public float Y
	{
		get
		{
			return y;
		}
		set
		{
			y = value;
		}
	}

	public float Z
	{
		get
		{
			return z;
		}
		set
		{
			z = value;
		}
	}

	public static Vector3 zero => new Vector3(0f, 0f, 0f);

	public static Vector3 Zero => new Vector3(0f, 0f, 0f);

	public Vector3(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public Vector3(float x, float y)
	{
		this.x = x;
		this.y = y;
		z = 0f;
	}

	public Vector3(Vector3 v)
	{
		x = v.x;
		y = v.y;
		z = v.z;
	}

	public static float Dot(Vector3 lhs, Vector3 rhs)
	{
		return (float)((double)lhs.x * (double)rhs.x + (double)lhs.y * (double)rhs.y + (double)lhs.z * (double)rhs.z);
	}

	public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
	{
		return new Vector3((float)((double)lhs.y * (double)rhs.z - (double)lhs.z * (double)rhs.y), (float)((double)lhs.z * (double)rhs.x - (double)lhs.x * (double)rhs.z), (float)((double)lhs.x * (double)rhs.y - (double)lhs.y * (double)rhs.x));
	}

	public static float SqrMagnitude(Vector3 a)
	{
		return (float)((double)a.x * (double)a.x + (double)a.y * (double)a.y + (double)a.z * (double)a.z);
	}

	public float Length()
	{
		double num = Y;
		double num2 = X;
		double num3 = Z;
		double num4 = num2 * num2;
		double num5 = num;
		double num6 = num4 + num5 * num5;
		double num7 = num3;
		return (float)Math.Sqrt(num6 + num7 * num7);
	}

	public void Normalize()
	{
		float num = Length();
		if ((double)num != 0.0)
		{
			float num2 = 1f / num;
			X *= num2;
			Y *= num2;
			Z *= num2;
		}
	}

	public static Vector3 operator +(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);
	}

	public static Vector3 operator -(Vector3 a, Vector3 b)
	{
		return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
	}

	public static Vector3 operator -(Vector3 a)
	{
		return new Vector3(0f - a.x, 0f - a.y, 0f - a.z);
	}

	public static Vector3 operator *(Vector3 a, float d)
	{
		return new Vector3(a.x * d, a.y * d, a.z * d);
	}

	public static Vector3 operator *(float d, Vector3 a)
	{
		return new Vector3(a.x * d, a.y * d, a.z * d);
	}

	public static Vector3 operator /(Vector3 a, float d)
	{
		return new Vector3(a.x / d, a.y / d, a.z / d);
	}

	public static bool operator ==(Vector3 lhs, Vector3 rhs)
	{
		return (double)SqrMagnitude(lhs - rhs) < 9.99999943962493E-11;
	}

	public static bool operator !=(Vector3 lhs, Vector3 rhs)
	{
		return (double)SqrMagnitude(lhs - rhs) >= 9.99999943962493E-11;
	}

	public override string ToString()
	{
		return string.Format("X:{0} Y:{1} Z:{2}", new object[3]
		{
			X.ToString("F16"),
			Y.ToString("F16"),
			Z.ToString("F16")
		});
	}

	public override bool Equals(object other)
	{
		if (other is Vector3 vector && x.Equals(vector.x) && y.Equals(vector.y))
		{
			return z.Equals(vector.z);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
	}
}
