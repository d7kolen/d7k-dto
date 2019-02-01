using d7k.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace d7k.Dto
{
	public class AnonymousDtoAdapterFactory : IDtoAdapterFactory
	{
		ConstructorInfo m_constructor;
		public FieldInfo[] Fields { get; private set; }

		public AnonymousDtoAdapterFactory(EmitTypeFactory factory, Type originalType, Type adapterType)
		{
			var unique = Guid.NewGuid();

			var typeBldr = factory.CreateClass(
				$"generatedAnonymousAdapter_{originalType.Name}_{adapterType.Name}_{unique}",
				null, adapterType, typeof(IDtoAdapter));

			var sourcePropertyName = $"Source_{unique}";
			var sourceProperty = factory.AddProperty(typeBldr, sourcePropertyName, originalType);

			var factoryPropertyName = $"Factory_{unique}";
			var factoryProperty = factory.AddProperty(typeBldr, factoryPropertyName, typeof(AnonymousDtoAdapterFactory));

			CreateProperties(factory, originalType, adapterType, typeBldr, sourceProperty, factoryProperty);
			CreateConstructor(factory, originalType, typeBldr, sourceProperty, factoryProperty);
			InternalDtoAdapterFactory.CreateIDtoAdapter(factory, typeBldr, sourceProperty);

			m_constructor = typeBldr.CreateType().GetConstructor(new[] { originalType, typeof(AnonymousDtoAdapterFactory) });
		}

		private static void CreateConstructor(EmitTypeFactory factory, Type originalType, TypeBuilder typeBldr, PropertyBuilder sourceProperty, PropertyBuilder factoryProperty)
		{
			var init = ExecBld.Return(
				ExecBld.Block(
					ExecBld.SetProp(sourceProperty, ExecBld.GetArg(0), ExecBld.GetArg(1)),
					ExecBld.SetProp(factoryProperty, ExecBld.GetArg(0), ExecBld.GetArg(2))));

			factory.CreateConstructor(typeBldr, new[] { originalType, typeof(AnonymousDtoAdapterFactory) }, init);
		}

		private void CreateProperties(EmitTypeFactory factory, Type originalType, Type adapterType, TypeBuilder typeBldr, PropertyBuilder sourceProperty, PropertyBuilder factoryProperty)
		{
			var getSource = ExecBld.GetProp(sourceProperty, ExecBld.GetThis());
			var getFactory = ExecBld.GetProp(factoryProperty, ExecBld.GetThis());

			var getMethod = this.GetType().GetMethod(nameof(GetValue));
			var setMethod = this.GetType().GetMethod(nameof(SetValue));

			var fields = new List<FieldInfo>();

			int index = 0;
			foreach (var t in adapterType.GetAllInterfaceProperties())
			{
				var tFiled = GetField(t, originalType);
				fields.Add(tFiled);

				var get =
					ExecBld.Return(
						ExecBld.Cast(
							ExecBld.Call(getMethod, getFactory, getSource, ExecBld.GetConst(index)),
							typeof(object), t.PropertyType));

				var set = ExecBld.Return(
					ExecBld.Call(setMethod, getFactory, getSource, ExecBld.GetConst(index),
					ExecBld.Cast(ExecBld.GetArg(1), t.PropertyType, typeof(object)))
					);

				factory.AddProperty(typeBldr, t, get, set, t.DeclaringType);

				index++;
			}

			Fields = fields.ToArray();
		}

		FieldInfo GetField(PropertyInfo property, Type originalType)
		{
			var backFieldName = $"<{property.Name}>i__Field";

			return originalType
				.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				.FirstOrDefault(f => f.Name == backFieldName && f.FieldType == property.PropertyType);
		}

		public object GetValue(object source, int index)
		{
			return Fields[index].GetValue(source);
		}

		public void SetValue(object source, int index, object value)
		{
			Fields[index].SetValue(source, value);
		}

		public object Create(object source)
		{
			return m_constructor.Invoke(new[] { source, this });
		}
	}
}
