using UnityEngine;
using System;
using System.Text;

namespace Loxodon.Framework.Views.Variables
{
    public static class DataConverter
    {
        public static string GetString(bool value)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(value));
        }

        public static string GetString(float value)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(value));
        }

        public static string GetString(int value)
        {
            return Convert.ToBase64String(BitConverter.GetBytes(value));
        }

        public static string GetString(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value == null ? "" : value));
        }

        public static string GetString(Color value)
        {
            byte[] data = new byte[sizeof(float) * 4];
            Buffer.BlockCopy(BitConverter.GetBytes(value.r), 0, data, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.g), 0, data, 1 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.b), 0, data, 2 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.a), 0, data, 3 * sizeof(float), sizeof(float));
            return Convert.ToBase64String(data);
        }

        public static string GetString(Vector2 value)
        {
            byte[] data = new byte[sizeof(float) * 2];
            Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, data, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, data, 1 * sizeof(float), sizeof(float));
            return Convert.ToBase64String(data);
        }

        public static string GetString(Vector3 value)
        {
            byte[] data = new byte[sizeof(float) * 3];
            Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, data, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, data, 1 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, data, 2 * sizeof(float), sizeof(float));
            return Convert.ToBase64String(data);
        }

        public static string GetString(Vector4 value)
        {
            byte[] data = new byte[sizeof(float) * 4];
            Buffer.BlockCopy(BitConverter.GetBytes(value.x), 0, data, 0 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.y), 0, data, 1 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.z), 0, data, 2 * sizeof(float), sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(value.w), 0, data, 3 * sizeof(float), sizeof(float));
            return Convert.ToBase64String(data);
        }

        public static bool ToBoolean(string value)
        {
            if (string.IsNullOrEmpty(value))
                return false;

            return BitConverter.ToBoolean(Convert.FromBase64String(value), 0);
        }

        public static float ToSingle(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0f;

            return BitConverter.ToSingle(Convert.FromBase64String(value), 0);
        }

        public static int ToInt32(string value)
        {
            if (string.IsNullOrEmpty(value))
                return 0;

            return BitConverter.ToInt32(Convert.FromBase64String(value), 0);
        }

        public static string ToString(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return Encoding.UTF8.GetString(Convert.FromBase64String(value));
        }

        public static Color ToColor(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Color.white;

            byte[] data = Convert.FromBase64String(value);
            Color color = Color.white;
            color.r = BitConverter.ToSingle(data, 0 * sizeof(float));
            color.g = BitConverter.ToSingle(data, 1 * sizeof(float));
            color.b = BitConverter.ToSingle(data, 2 * sizeof(float));
            color.a = BitConverter.ToSingle(data, 3 * sizeof(float));
            return color;
        }

        public static Vector2 ToVector2(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Vector2.zero;

            byte[] data = Convert.FromBase64String(value);
            Vector2 vector = Vector2.zero;
            vector.x = BitConverter.ToSingle(data, 0 * sizeof(float));
            vector.y = BitConverter.ToSingle(data, 1 * sizeof(float));
            return vector;
        }

        public static Vector3 ToVector3(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Vector3.zero;

            byte[] data = Convert.FromBase64String(value);
            Vector3 vector = Vector3.zero;
            vector.x = BitConverter.ToSingle(data, 0 * sizeof(float));
            vector.y = BitConverter.ToSingle(data, 1 * sizeof(float));
            vector.z = BitConverter.ToSingle(data, 2 * sizeof(float));
            return vector;
        }

        public static Vector4 ToVector4(string value)
        {
            if (string.IsNullOrEmpty(value))
                return Vector4.zero;

            byte[] data = Convert.FromBase64String(value);
            Vector4 vector = Vector4.zero;
            vector.x = BitConverter.ToSingle(data, 0 * sizeof(float));
            vector.y = BitConverter.ToSingle(data, 1 * sizeof(float));
            vector.z = BitConverter.ToSingle(data, 2 * sizeof(float));
            vector.w = BitConverter.ToSingle(data, 3 * sizeof(float));
            return vector;
        }
    }
}