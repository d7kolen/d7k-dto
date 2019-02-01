using d7k.Emit;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace d7k.Dto
{
	class PureDtoFactory
	{
		ConstructorInfo m_constructor;

		public PureDtoFactory(EmitTypeFactory factory, Type dtoType)
		{
			var unique = Guid.NewGuid();

			var typeBldr = factory.CreateClass(
				$"generatedPureDto_{dtoType.Name}_{unique}",
				null, dtoType);

			CreateProperties(factory, dtoType, typeBldr);
			CreateConstructor(factory, typeBldr);

			m_constructor = typeBldr.CreateType().GetConstructor(new Type[0]);
		}

		private static void CreateConstructor(EmitTypeFactory factory, TypeBuilder typeBldr)
		{
			var init = ExecBld.Return();

			factory.CreateConstructor(typeBldr, new Type[0], init);
		}

		private void CreateProperties(EmitTypeFactory factory, Type adapterType, TypeBuilder typeBldr)
		{
			foreach (var t in adapterType.GetAllInterfaceProperties())
			{
				var fieldName = "_" + t.Name;
				var field = typeBldr.DefineField(fieldName, t.PropertyType, FieldAttributes.Private);

				var get = ExecBld.Return(ExecBld.GetFld(field, ExecBld.GetThis()));
				var set = ExecBld.Return(ExecBld.SetFld(field, ExecBld.GetThis(), ExecBld.GetArg(1)));

				factory.AddProperty(typeBldr, t, get, set, t.DeclaringType);
			}
		}

		public object Create()
		{
			return m_constructor.Invoke(new object[0]);
		}
	}
}
