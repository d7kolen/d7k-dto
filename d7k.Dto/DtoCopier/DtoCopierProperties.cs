using d7k.Dto.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace d7k.Dto
{
	class DtoCopierProperties
	{
		public Type ObjectType { get; }
		public Type PropertiesType { get; }
		public Type InterfType { get; }
		public Type GenericInterfType { get; }

		public DtoCopierProperties(Type objType, Type interfType)
		{
			ObjectType = objType;
			InterfType = interfType;

			if (InterfType.IsGenericType)
				GenericInterfType = InterfType.GetGenericTypeDefinition();

			PropertiesType = InitPropertiesType();
		}

		Type InitPropertiesType()
		{
			var typeInterf = ObjectType.FindInterfaces((t, fCrit) => t == InterfType, null).FirstOrDefault();

			return typeInterf ?? ObjectType;
		}

		public IEnumerable<PropertyInfo> GetProperties()
		{
			var typeInterf = InitPropertiesType();

			var allInterf = InterfType.GetInterfaces().ToList();
			allInterf.Add(InterfType);

			if (typeInterf != InterfType)
			{
				var interfProperties = allInterf.SelectMany(x => x.GetProperties()).ToList();
				var objProperties = typeInterf.GetProperties().ToDictionary(x => x.Name);

				var resProperties = interfProperties
					.Select(x => new { objP = objProperties.Get(x.Name), intP = x })
					.ToList();

				var result = new List<PropertyInfo>();

				var errorMessages = new List<string>();

				foreach (var t in resProperties)
				{
					if (t.objP == null)
					{
						errorMessages.Add($"{ObjectType.FullName} hasn't property {t.intP.Name}.");
						continue;
					}

					result.Add(t.objP);
				}

				if (errorMessages.Any())
					throw new NotImplementedException(string.Join(Environment.NewLine, errorMessages));

				return result;
			}

			if (InterfType.IsInterface)
				return allInterf.SelectMany(x => ObjectType.GetInterfaceMap(x).InterfaceType.GetProperties());
			else
				return InterfType.GetProperties();
		}
	}
}
