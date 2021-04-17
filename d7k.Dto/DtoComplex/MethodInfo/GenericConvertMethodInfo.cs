using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace d7k.Dto.Complex
{
	class GenericConvertMethodInfo
	{
		public MethodInfo Method { get; set; }
		public Type TemplateType { get; set; }
		public bool HasContext { get; set; }

		public ConvertMethodInfo CreateMethod(Type dstType, Type srcType)
		{
			var result = new ConvertMethodInfo();
			result.DstType = dstType;
			result.SrcType = srcType;
			result.HasContext = HasContext;

			result.Method = MakeGenericMethod(dstType, srcType);

			return result;
		}

		private MethodInfo MakeGenericMethod(Type dstType, Type srcType)
		{
			var templatePar = GetAllMethodParameters(dstType, srcType);

			return Method.MakeGenericMethod(templatePar.ToArray());
		}

		private List<Type> GetAllMethodParameters(Type dstType, Type srcType)
		{
			var templatePar = new List<Type>();

			var methodPar = Method.GetParameters();

			if (methodPar[0].ParameterType.ContainsGenericParameters)
				templatePar.AddRange(dstType.GetGenericArguments());

			if (methodPar[1].ParameterType.ContainsGenericParameters)
				templatePar.AddRange(srcType.GetGenericArguments());
			return templatePar;
		}

		public bool AvailableForFilters(Type dstType, Type srcType, DtoComplexCache dto)
		{
			var filters = Method.GetCustomAttributes<DtoConvertFilterAttribute>().ToList();
			if (!filters.Any())
				return true;

			var templatePar = GetAllMethodParameters(dstType, srcType);

			foreach (var tFilter in filters)
			{
				var tRes = true;

				for (var iPar = 0; iPar < templatePar.Count; iPar++)
				{
					if (tFilter.AvailableTemplateTypes.Length <= iPar)
						break;

					var filterType = tFilter.AvailableTemplateTypes[iPar];
					var realType = templatePar[iPar];

					if (!(
						filterType == realType
						||
						realType.IsSubclassOf(filterType)
						||
						dto.GetInterfaces(realType).Contains(filterType)
						))
					{
						tRes = false;
						break;
					}
				}

				if (tRes)
					return true;
			}

			return false;
		}
	}
}