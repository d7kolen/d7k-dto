using d7k.Emit;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace d7k.Dto
{
	public class InternalDtoAdapterFactory : IDtoAdapterFactory
	{
		ConstructorInfo m_constructor;
		public PropertyInfo[] Properties { get; private set; }

		public InternalDtoAdapterFactory(EmitTypeFactory factory, Type originalType, Type adapterType)
		{
			var unique = Guid.NewGuid();

			var typeBldr = factory.CreateClass(
				$"generatedInternalAdapter_{originalType.Name}_{adapterType.Name}_{unique}",
				null, adapterType, typeof(IDtoAdapter));

			var sourcePropertyName = $"Source_{unique}";
			var sourceProperty = factory.AddProperty(typeBldr, sourcePropertyName, originalType);

			var factoryPropertyName = $"Factory_{unique}";
			var factoryProperty = factory.AddProperty(typeBldr, factoryPropertyName, typeof(InternalDtoAdapterFactory));

			CreateProperties(factory, originalType, adapterType, typeBldr, sourceProperty, factoryProperty);
			CreateConstructor(factory, originalType, typeBldr, sourceProperty, factoryProperty);
			CreateIDtoAdapter(factory, typeBldr, sourceProperty);

			m_constructor = typeBldr.CreateType().GetConstructor(new[] { originalType, typeof(InternalDtoAdapterFactory) });
		}

		private static void CreateConstructor(EmitTypeFactory factory, Type originalType, TypeBuilder typeBldr, PropertyBuilder sourceProperty, PropertyBuilder factoryProperty)
		{
			var init = ExecBld.Return(
				ExecBld.Block(
					ExecBld.SetProp(sourceProperty, ExecBld.GetArg(0), ExecBld.GetArg(1)),
					ExecBld.SetProp(factoryProperty, ExecBld.GetArg(0), ExecBld.GetArg(2))));

			factory.CreateConstructor(typeBldr, new[] { originalType, typeof(InternalDtoAdapterFactory) }, init);
		}

		private void CreateProperties(EmitTypeFactory factory, Type originalType, Type adapterType, TypeBuilder typeBldr, PropertyBuilder sourceProperty, PropertyBuilder factoryProperty)
		{
			var getSource = ExecBld.GetProp(sourceProperty, ExecBld.GetThis());
			var getFactory = ExecBld.GetProp(factoryProperty, ExecBld.GetThis());

			var getMethod = this.GetType().GetMethod(nameof(GetValue));
			var setMethod = this.GetType().GetMethod(nameof(SetValue));

			var properties = new List<PropertyInfo>();

			int index = 0;
			foreach (var t in adapterType.GetAllInterfaceProperties())
			{
				var tProp = originalType.GetProperty(t.Name, t.PropertyType);
				properties.Add(tProp);

				var get =
					ExecBld.Return(
						ExecBld.Cast(
							ExecBld.Call(getMethod, getFactory, getSource, ExecBld.GetConst(index)),
							typeof(object), tProp.PropertyType));

				var set = ExecBld.Return(
					ExecBld.Call(setMethod, getFactory, getSource, ExecBld.GetConst(index),
					ExecBld.Cast(ExecBld.GetArg(1), tProp.PropertyType, typeof(object)))
					);

				factory.AddProperty(typeBldr, t, get, set, t.DeclaringType);

				index++;
			}

			Properties = properties.ToArray();
		}

		public static void CreateIDtoAdapter(EmitTypeFactory factory, TypeBuilder typeBldr, PropertyBuilder sourceProperty)
		{
			var body = ExecBld.Return(ExecBld.GetProp(sourceProperty, ExecBld.GetThis()));
			factory.CreateMethod(typeBldr, nameof(IDtoAdapter.GetSource), typeof(object), new Type[0], typeof(IDtoAdapter), body);
		}

		public object GetValue(object source, int index)
		{
			return Properties[index].GetValue(source);
		}

		public void SetValue(object source, int index, object value)
		{
			Properties[index].SetValue(source, value);
		}

		public object Create(object source)
		{
			return m_constructor.Invoke(new[] { source, this });
		}
	}
}
