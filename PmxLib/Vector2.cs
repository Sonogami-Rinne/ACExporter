namespace PmxLib;

public struct Vector2
{
	public float x;

	public float y;

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

	public static Vector2 zero => new Vector2(0f, 0f);

	public Vector2(float x, float y)
	{
		this.x = x;
		this.y = y;
	}

	public Vector2(Vector2 v)
	{
		x = v.x;
		y = v.y;
	}

	public override bool Equals(object other)
	{
		if (other is Vector2 vector && x.Equals(vector.x))
		{
			return y.Equals(vector.y);
		}
		return false;
	}

	public static Vector2 operator +(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x + b.x, a.y + b.y);
	}

	public static Vector2 operator -(Vector2 a, Vector2 b)
	{
		return new Vector2(a.x - b.x, a.y - b.y);
	}

	public static Vector2 operator -(Vector2 a)
	{
		return new Vector2(0f - a.x, 0f - a.y);
	}

	public static Vector2 operator *(Vector2 a, float d)
	{
		return new Vector2(a.x * d, a.y * d);
	}

	public static Vector2 operator *(float d, Vector2 a)
	{
		return new Vector2(a.x * d, a.y * d);
	}

	public static Vector2 operator /(Vector2 a, float d)
	{
		return new Vector2(a.x / d, a.y / d);
	}

	public static bool operator ==(Vector2 lhs, Vector2 rhs)
	{
		return (double)SqrMagnitude(lhs - rhs) < 9.99999943962493E-11;
	}

	public static bool operator !=(Vector2 lhs, Vector2 rhs)
	{
		return (double)SqrMagnitude(lhs - rhs) >= 9.99999943962493E-11;
	}

	public static implicit operator Vector2(Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	public static implicit operator Vector3(Vector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}

	public static float SqrMagnitude(Vector2 a)
	{
		return (float)((double)a.x * (double)a.x + (double)a.y * (double)a.y);
	}

	public override string ToString()
	{
		return string.Format("X:{0} Y:{1}", new object[2]
		{
			X.ToString(),
			Y.ToString()
		});
	}

	public override int GetHashCode()
	{
		return x.GetHashCode() ^ (y.GetHashCode() << 2);
	}
}
