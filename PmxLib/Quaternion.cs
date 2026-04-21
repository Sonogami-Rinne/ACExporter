namespace PmxLib;

public struct Quaternion
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

	public static Quaternion identity
	{
		get
		{
			Quaternion result = default(Quaternion);
			result.X = 0f;
			result.Y = 0f;
			result.Z = 0f;
			result.W = 1f;
			return result;
		}
	}

	public static Quaternion Identity
	{
		get
		{
			Quaternion result = default(Quaternion);
			result.X = 0f;
			result.Y = 0f;
			result.Z = 0f;
			result.W = 1f;
			return result;
		}
	}

	public Quaternion(Vector3 value, float w)
	{
		x = value.X;
		y = value.Y;
		z = value.Z;
		this.w = w;
	}

	public Quaternion(float x, float y, float z, float w)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	public static Quaternion operator *(float scale, Quaternion quaternion)
	{
		Quaternion result = default(Quaternion);
		result.X = quaternion.X * scale;
		result.Y = quaternion.Y * scale;
		result.Z = quaternion.Z * scale;
		result.W = quaternion.W * scale;
		return result;
	}

	public static Quaternion operator *(Quaternion quaternion, float scale)
	{
		Quaternion result = default(Quaternion);
		result.X = quaternion.X * scale;
		result.Y = quaternion.Y * scale;
		result.Z = quaternion.Z * scale;
		result.W = quaternion.W * scale;
		return result;
	}

	public static Quaternion operator *(Quaternion left, Quaternion right)
	{
		Quaternion result = default(Quaternion);
		float num = left.X;
		float num2 = left.Y;
		float num3 = left.Z;
		float num4 = left.W;
		float num5 = right.X;
		float num6 = right.Y;
		float num7 = right.Z;
		float num8 = right.W;
		result.X = (float)((double)num5 * (double)num4 + (double)num8 * (double)num + (double)num6 * (double)num3 - (double)num7 * (double)num2);
		result.Y = (float)((double)num6 * (double)num4 + (double)num8 * (double)num2 + (double)num7 * (double)num - (double)num5 * (double)num3);
		result.Z = (float)((double)num7 * (double)num4 + (double)num8 * (double)num3 + (double)num5 * (double)num2 - (double)num6 * (double)num);
		result.W = (float)((double)num8 * (double)num4 - ((double)num6 * (double)num2 + (double)num5 * (double)num + (double)num7 * (double)num3));
		return result;
	}

	public static Quaternion operator /(Quaternion left, float right)
	{
		Quaternion result = default(Quaternion);
		result.X = left.X / right;
		result.Y = left.Y / right;
		result.Z = left.Z / right;
		result.W = left.W / right;
		return result;
	}

	public static Quaternion operator +(Quaternion left, Quaternion right)
	{
		Quaternion result = default(Quaternion);
		result.X = left.X + right.X;
		result.Y = left.Y + right.Y;
		result.Z = left.Z + right.Z;
		result.W = left.W + right.W;
		return result;
	}

	public static Quaternion operator -(Quaternion quaternion)
	{
		Quaternion result = default(Quaternion);
		result.X = 0f - quaternion.X;
		result.Y = 0f - quaternion.Y;
		result.Z = 0f - quaternion.Z;
		result.W = 0f - quaternion.W;
		return result;
	}

	public static Quaternion operator -(Quaternion left, Quaternion right)
	{
		Quaternion result = default(Quaternion);
		result.X = left.X - right.X;
		result.Y = left.Y - right.Y;
		result.Z = left.Z - right.Z;
		result.W = left.W - right.W;
		return result;
	}

	public static bool operator ==(Quaternion left, Quaternion right)
	{
		return Equals(ref left, ref right);
	}

	public static bool operator !=(Quaternion left, Quaternion right)
	{
		return !Equals(ref left, ref right);
	}

	public override string ToString()
	{
		return $"X:{X.ToString()} Y:{Y.ToString()} Z:{Z.ToString()} W:{W.ToString()}";
	}

	public override int GetHashCode()
	{
		float num = X;
		float num2 = Y;
		float num3 = Z;
		float num4 = W;
		int num5 = num3.GetHashCode() + num4.GetHashCode() + num2.GetHashCode();
		return num.GetHashCode() + num5;
	}

	public static bool Equals(ref Quaternion value1, ref Quaternion value2)
	{
		if ((double)value1.X == (double)value2.X && (double)value1.Y == (double)value2.Y && (double)value1.Z == (double)value2.Z)
		{
			return (double)value1.W == (double)value2.W;
		}
		return false;
	}

	public bool Equals(Quaternion other)
	{
		if ((double)X == (double)other.X && (double)Y == (double)other.Y && (double)Z == (double)other.Z)
		{
			return (double)W == (double)other.W;
		}
		return false;
	}

	public override bool Equals(object obj)
	{
		if (obj != null && obj.GetType() == GetType())
		{
			return Equals((Quaternion)obj);
		}
		return false;
	}
}
