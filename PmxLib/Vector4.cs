namespace PmxLib;

public struct Vector4
{
	public float x;

	public float y;

	public float z;

	public float w;

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

	public float W
	{
		get
		{
			return w;
		}
		set
		{
			w = value;
		}
	}

	public static Vector4 zero => new Vector4(0f, 0f, 0f, 0f);

	public Vector4(float x, float y, float z, float w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	public Vector4(float x, float y, float z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		w = 0f;
	}

	public Vector4(float x, float y)
	{
		this.x = x;
		this.y = y;
		z = 0f;
		w = 0f;
	}

	public static float Dot(Vector4 a, Vector4 b)
	{
		return (float)((double)a.x * (double)b.x + (double)a.y * (double)b.y + (double)a.z * (double)b.z + (double)a.w * (double)b.w);
	}

	public static float SqrMagnitude(Vector4 a)
	{
		return Dot(a, a);
	}

	public static Vector4 operator +(Vector4 a, Vector4 b)
	{
		return new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
	}

	public static Vector4 operator -(Vector4 a, Vector4 b)
	{
		return new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);
	}

	public static Vector4 operator -(Vector4 a)
	{
		return new Vector4(0f - a.x, 0f - a.y, 0f - a.z, 0f - a.w);
	}

	public static Vector4 operator *(Vector4 a, float d)
	{
		return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
	}

	public static Vector4 operator *(float d, Vector4 a)
	{
		return new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);
	}

	public static Vector4 operator /(Vector4 a, float d)
	{
		return new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);
	}

	public static bool operator ==(Vector4 lhs, Vector4 rhs)
	{
		return (double)SqrMagnitude(lhs - rhs) < 9.99999943962493E-11;
	}

	public static bool operator !=(Vector4 lhs, Vector4 rhs)
	{
		return (double)SqrMagnitude(lhs - rhs) >= 9.99999943962493E-11;
	}

	public static implicit operator Vector4(Vector3 v)
	{
		return new Vector4(v.x, v.y, v.z, 0f);
	}

	public static implicit operator Vector3(Vector4 v)
	{
		return new Vector3(v.x, v.y, v.z);
	}

	public static implicit operator Vector4(Vector2 v)
	{
		return new Vector4(v.x, v.y, 0f, 0f);
	}

	public static implicit operator Vector2(Vector4 v)
	{
		return new Vector2(v.x, v.y);
	}

	public override bool Equals(object other)
	{
		if (other is Vector4 vector && x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z))
		{
			return w.Equals(vector.w);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
	}
}
