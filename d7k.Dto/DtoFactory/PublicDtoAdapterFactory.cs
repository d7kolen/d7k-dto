using System;
using System.Reflection;
using System.Reflection.Emit;
using d7k.Dto.Emit;

namespace d7k.Dto
{
	class PublicDtoAdapterFactory : IDtoAdapterFactory
	{
		ConstructorInfo m_constructor;

		public PublicDtoAdapterFactory(EmitTypeFactory factory, Type originalType, Type adapterType)
		{
			var unique = Guid.NewGuid();

			var typeBldr = factory.CreateClass(
				$"generatedPublicAdapter_{originalType.Name}_{adapterType.Name}_{unique}",
				null, adapterType, typeof(IDtoAdapter));

			var sourcePropertyName = $"Source_{unique}";
			var sourceProperty = factory.AddProperty(typeBldr, sourcePropertyName, originalType);

			CreateProperties(factory, originalType, adapterType, typeBldr, sourceProperty);
			CreateConstructor(factory, originalType, typeBldr, sourceProperty);
			InternalDtoAdapterFactory.CreateIDtoAdapter(factory, typeBldr, sourceProperty);

			m_constructor = typeBldr.CreateTypeInfo().GetConstructor(new[] { originalType });
		}

		private static void CreateConstructor(EmitTypeFactory factory, Type originalType, TypeBuilder typeBldr, PropertyBuilder sourceProperty)
		{
			var init = ExecBld.Return(ExecBld.SetProp(sourceProperty, ExecBld.GetArg(0), ExecBld.GetArg(1)));

			factory.CreateConstructor(typeBldr, new[] { originalType }, init);
		}

		private void CreateProperties(EmitTypeFactory factory, Type originalType, Type adapterType, TypeBuilder typeBldr, PropertyBuilder sourceProperty)
		{
			var getSource = ExecBld.GetProp(sourceProperty, ExecBld.GetThis());

			foreach (var t in adapterType.GetAllInterfaceProperties())
			{
				var tProp = originalType.GetProperty(t.Name);//, t.PropertyType);

				var get = ExecBld.Return(ExecBld.GetProp(tProp, getSource));
				var set = ExecBld.Return(ExecBld.SetProp(tProp, getSource, ExecBld.GetArg(1)));

				factory.AddProperty(typeBldr, t, get, set, t.DeclaringType);
			}
		}

		public object Create(object source)
		{
			return m_constructor.Invoke(new[] { source });
		}
	}
}
