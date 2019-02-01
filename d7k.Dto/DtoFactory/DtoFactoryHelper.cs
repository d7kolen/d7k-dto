using d7k.Utilities.Monads;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace d7k.Dto
{
	static class DtoFactoryHelper
	{
		public static IEnumerable<PropertyInfo> GetAllInterfaceProperties(this Type interf)
		{
			if (!interf.IsInterface)
				throw new NotImplementedException($"{interf.FullName} should be interface.");

			foreach (var tInterf in interf.GetInterfaces().AddItem(interf))
				foreach (var t in tInterf.GetProperties())
					yield return t;
		}
	}
}
