using System;
using System.Reflection;

namespace d7k.Dto.Complex
{
	class ValidationMethodInfo
	{
		public MethodInfo Method { get; set; }
		public Type FactoryType { get; set; }
		public Type RuleType { get; set; }
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

		public ValidationMethodInfo InitTypes(Type dtoType)
		{
			FactoryType = typeof(ValidationRuleFactory<>).MakeGenericType(dtoType);
			RuleType = typeof(DtoComplexAdapterRule<>).MakeGenericType(dtoType);
			return this;
		}

		public ValidationMethodInfo Clone()
		{
			return new ValidationMethodInfo()
			{
				Method = Method,
				FactoryType = FactoryType,
				RuleType = RuleType,
				HasContext = HasContext
			};
		}
	}
}
