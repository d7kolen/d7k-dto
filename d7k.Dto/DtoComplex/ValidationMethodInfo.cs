using System;
using System.Reflection;

namespace d7k.Dto
{
	class ValidationMethodInfo
	{
		public MethodInfo Method { get; set; }
		public Type FactoryType { get; set; }
		public Type ComplexRuleType { get; set; }
		public bool HasContext { get; set; }

		public void Invoke(object factory, object context)
		{
			object[] parameters;
			if (HasContext)
				parameters = new[] { factory, context };
			else
				parameters = new[] { factory };

			Method.Invoke(null, parameters);
		}
	}
}
