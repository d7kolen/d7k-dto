using d7k.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace d7k.Dto
{
	public class DtoCopierCastStorage
	{
		Dictionary<string, MethodInfo> m_methods = new Dictionary<string, MethodInfo>();

		/// <summary>
		/// Signature:<para/>
		/// static T1 Cast(T0 src)<para/>
		/// static T1 Cast&lt;T0,T1&gt;(T0 src)<para/>
		/// </summary>
		public void Append(MethodInfo castFunc, params Type[] availableTemplates)
		{
			var error = CheckMethod(castFunc);
			if (error != null)
				throw error;

			var parameters = castFunc.GetParameters();

			var returnType = castFunc.ReturnType;
			var srcType = parameters[0].ParameterType;

			if (availableTemplates?.Any() != true)
				m_methods[GetKey(returnType, srcType)] = castFunc;
			else
			{
				foreach (var t in availableTemplates)
					m_methods[GetKey(returnType, srcType, t)] = castFunc;
			}
		}

		public static Exception CheckMethod(MethodInfo castFunc)
		{
			if (!castFunc.IsStatic)
				return new NotImplementedException($"Cast method {castFunc.Name} can be a static only.");

			if (castFunc.ReturnType == typeof(void))
				return new NotImplementedException($"Cast method {castFunc.Name} can have a return value only.");

			if (castFunc.GetParameters().Length != 1)
				return new NotImplementedException($"Cast method {castFunc.Name} can have single parameter only.");

			if (castFunc.IsGenericMethod)
				return new NotImplementedException($"The cast method {castFunc.Name} cannot be generic.");

			return null;
		}

		public MethodInfo Get(Type to, Type from, Type template)
		{
			var strongAvailableCast = m_methods.Get(GetKey(to, from, template));
			if (strongAvailableCast != null)
				return strongAvailableCast;

			var strongCast = m_methods.Get(GetKey(to, from));
			if (strongCast != null)
				return strongCast;

			return null;
		}

		private string GetKey(Type to, Type from)
		{
			return $"{to}#$&@&$#{from}";
		}

		private string GetKey(Type to, Type from, Type template)
		{
			return $"{to}#$&@&$#{from}#$&@&$#{template}";
		}
	}
}
