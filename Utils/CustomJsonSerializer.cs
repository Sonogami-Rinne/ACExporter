using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class SerializableAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Field)]
public class NonSerializedAttribute : Attribute { }

public static class CustomJsonSerializer
{
    public static string Serialize(object obj)
    {
        if (obj == null)
            return "null";

        StringBuilder sb = new StringBuilder();
        SerializeValue(obj, sb, new HashSet<object>());
        return sb.ToString();
    }

    private static void SerializeValue(object value, StringBuilder sb, HashSet<object> visitedObjects)
    {
        if (value == null)
        {
            sb.Append("null");
            return;
        }

        Type type = value.GetType();

        //// 防止循环引用
        //if (IsReferenceType(type) && !type.IsPrimitive && type != typeof(string))
        //{
        //    if (visitedObjects.Contains(value))
        //    {
        //        sb.Append("null"); 
        //        return;
        //    }
        //    visitedObjects.Add(value);
        //}


        if (type == typeof(string))
        {
            SerializeString((string)value, sb);
        }

        else if (type.IsPrimitive || type == typeof(decimal))
        {
            sb.Append(value.ToString().ToLower());
        }
        else if (type.IsEnum)
        {
            sb.Append(Convert.ToInt32(value).ToString());
        }

        else if (type.IsArray || (typeof(IList).IsAssignableFrom(type) && type.IsGenericType))
        {
            SerializeArray(value, sb, visitedObjects);
        }

        else if (typeof(IDictionary).IsAssignableFrom(type) && type.IsGenericType)
        {
            SerializeDictionary(value, sb, visitedObjects);
        }

        else if (type.IsValueType || type.GetCustomAttribute<SerializableAttribute>() != null)
        {
            SerializeObject(value, sb, visitedObjects);
        }
        //else
        //{
        //    throw new NotSupportedException($"Type {type.Name} is not supported for serialization");
        //}

        //if (IsReferenceType(type) && !type.IsPrimitive && type != typeof(string))
        //{
        //    visitedObjects.Remove(value);
        //}
    }

    private static bool IsReferenceType(Type type)
    {
        return !type.IsValueType || (type.IsValueType && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
    }

    private static void SerializeString(string value, StringBuilder sb)
    {
        sb.Append('"');
        foreach (char c in value)
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\b': sb.Append("\\b"); break;
                case '\f': sb.Append("\\f"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default:
                    if (c < ' ')
                    {
                        sb.AppendFormat("\\u{0:X4}", (int)c);
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    break;
            }
        }
        sb.Append('"');
    }

    private static void SerializeArray(object value, StringBuilder sb, HashSet<object> visitedObjects)
    {
        sb.Append('[');
        bool first = true;

        if (value is IList list)
        {
            foreach (var item in list)
            {
                if (!first) sb.Append(',');
                SerializeValue(item, sb, visitedObjects);
                first = false;
            }
        }
        else if (value is Array array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (!first) sb.Append(',');
                SerializeValue(array.GetValue(i), sb, visitedObjects);
                first = false;
            }
        }

        sb.Append(']');
    }

    private static void SerializeDictionary(object value, StringBuilder sb, HashSet<object> visitedObjects)
    {
        sb.Append('{');
        bool first = true;

        IDictionary dictionary = (IDictionary)value;
        foreach (DictionaryEntry entry in dictionary)
        {
            if (!first) sb.Append(',');

            SerializeString(entry.Key.ToString(), sb);
            sb.Append(':');
            SerializeValue(entry.Value, sb, visitedObjects);

            first = false;
        }

        sb.Append('}');
    }

    private static void SerializeObject(object value, StringBuilder sb, HashSet<object> visitedObjects)
    {
        Type type = value.GetType();
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        sb.Append('{');
        bool first = true;

        foreach (var property in properties)
        {
            if (property.GetIndexParameters().Length > 0)
                continue;

            if (property.CanRead)
            {
                if (!first) sb.Append(',');

                SerializeString(property.Name, sb);
                sb.Append(':');
                SerializeValue(property.GetValue(value), sb, visitedObjects);

                first = false;
            }
        }

        foreach (var field in fields)
        {
            if (field.GetCustomAttribute<NonSerializedAttribute>() != null)
            {
                continue;
            }

            if (!first) sb.Append(',');

            SerializeString(field.Name, sb);
            sb.Append(':');
            SerializeValue(field.GetValue(value), sb, visitedObjects);

            first = false;
        }

        sb.Append('}');
    }
}