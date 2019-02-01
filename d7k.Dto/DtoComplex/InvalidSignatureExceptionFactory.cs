using System;
using System.Reflection;
using System.Text;

namespace d7k.Dto
{
	public class InvalidSignatureExceptionFactory
	{
		public static Exception Create(MethodInfo method)
		{
			var accum = new StringBuilder($"The method of type [{FullTypeName(method.DeclaringType)}] has invalid signature: ");

			accum.Append(FullTypeName(method.ReturnType))
				.Append(" ")
				.Append(method.Name)
				.Append("(");

			foreach (var t in method.GetParameters())
			{
				string typeName = FullTypeName(t.ParameterType);
				accum.Append(typeName).Append(" ").Append(t.Name).Append(", ");
			}

			accum.Length -= 2;
			accum.Append(")");

			return new InvalidOperationException(accum.ToString());
		}

		private static string FullTypeName(Type type)
		{
			var typeName = type.FullName;

			if (type.IsGenericType)
			{
				var typeNameAcc = new StringBuilder();

				var parts = typeName.Split('`');
				typeNameAcc.Append(parts[0]).Append("<");

				foreach (var gPar in type.GetGenericArguments())
					typeNameAcc.Append(FullTypeName(gPar)).Append(",");

				typeNameAcc.Length--;
				typeNameAcc.Append(">");

				typeName = typeNameAcc.ToString();
			}

			return typeName;
		}
	}
}
