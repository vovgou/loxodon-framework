using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Editors
{
    public class CodeGenerator
	{
		public string Generate (string className, Dictionary<string,object> dict)
		{
			string template = "using Loxodon.Framework.Localizations;\r\n${namespaces}\r\npublic static partial class ${name}\r\n{\r\n${properties}}";			
			string format = "    public readonly static V<{0}> {1} = new V<{0}>(\"{2}\"); ";

			StringBuilder propertiesBuf = new StringBuilder ();
			StringBuilder namespacesBuf = new StringBuilder ();
			List<string> namespaces = new List<string> ();

			foreach (KeyValuePair<string,object> kv in dict) {
				Type type = kv.Value.GetType ();

				if (!string.IsNullOrEmpty (type.Namespace) && !namespaces.Contains (type.Namespace)) {
					namespaces.Add (type.Namespace);

					namespacesBuf.AppendFormat ("using {0};", type.Namespace).Append (Environment.NewLine);
				}

				propertiesBuf.AppendFormat (format, GetTypeName (kv.Value.GetType ()), GetPropertyName (kv.Key), kv.Key).Append (Environment.NewLine).Append (Environment.NewLine);
			}

			var code = template.Replace ("${namespaces}", namespacesBuf.ToString ());
			code = code.Replace ("${name}", className);
			code = code.Replace ("${properties}", propertiesBuf.ToString ());
			return code;
		}

		private string GetPropertyName (string key)
		{
			return Regex.Replace (key, "[.]", "_");
		}

		private string GetTypeName (System.Type type)
		{
			TypeCode typeCode = Type.GetTypeCode (type);
			switch (typeCode) {
			case TypeCode.String:
				return "string";
			case TypeCode.Boolean:
				return "bool";
			case TypeCode.SByte:
				return "sbyte";
			case TypeCode.Byte:
				return "byte";
			case TypeCode.Int16:
				return "short";
			case TypeCode.UInt16:
				return "ushort";
			case TypeCode.Int32:
				return "int";
			case TypeCode.UInt32:
				return "uint";
			case TypeCode.Int64:
				return "long";
			case TypeCode.UInt64:
				return "ulong";
			case TypeCode.Char:
				return "char";
			case TypeCode.Single:
				return "float";
			case TypeCode.Double:
				return "double";
			case TypeCode.Decimal:
				return "decimal";
			default:
				if (type.IsArray) {
					if (type.Equals (typeof(string[])))
						return "string[]";
					if (type.Equals (typeof(bool[])))
						return "bool[]";
					if (type.Equals (typeof(sbyte[])))
						return "sbyte[]";
					if (type.Equals (typeof(byte[])))
						return "byte[]";
					if (type.Equals (typeof(short[])))
						return "short[]";
					if (type.Equals (typeof(ushort[])))
						return "ushort[]";					
					if (type.Equals (typeof(int[])))
						return "int[]";
					if (type.Equals (typeof(uint[])))
						return "uint[]";
					if (type.Equals (typeof(long[])))
						return "long[]";
					if (type.Equals (typeof(ulong[])))
						return "ulong[]";
					if (type.Equals (typeof(char[])))
						return "char[]";
					if (type.Equals (typeof(float[])))
						return "float[]";
					if (type.Equals (typeof(double[])))
						return "double[]";
					if (type.Equals (typeof(decimal[])))
						return "decimal[]";
				}
				return type.Name;
			}
		}
	}
}
