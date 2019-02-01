using d7k.Utilities.Monads;
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

		Type m_interfType;

		public DtoCopierProperties(Type objType, Type interfType)
		{
			ObjectType = objType;
			m_interfType = interfType;

			PropertiesType = InitPropertiesType();
		}

		Type InitPropertiesType()
		{
			var typeInterf = ObjectType.FindInterfaces((t, fCrit) => t == m_interfType, null).FirstOrDefault();

			return typeInterf ?? ObjectType;
		}

		public IEnumerable<PropertyInfo> GetProperties()
		{
			var typeInterf = InitPropertiesType();

			var allInterf = m_interfType.GetInterfaces().ToList();
			allInterf.Add(m_interfType);

			if (typeInterf != m_interfType)
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
					if (t.intP.PropertyType != t.objP.PropertyType)
					{
						errorMessages.Add($"{t.objP.Name} property of {ObjectType.FullName} type should be {t.intP.PropertyType.FullName}.");
						continue;
					}

					result.Add(t.objP);
				}

				if (errorMessages.Any())
					throw new NotImplementedException(string.Join(Environment.NewLine, errorMessages));

				return result;
			}

			if (m_interfType.IsInterface)
				return allInterf.SelectMany(x => ObjectType.GetInterfaceMap(x).InterfaceType.GetProperties());
			else
				return m_interfType.GetProperties();
		}
	}
}
