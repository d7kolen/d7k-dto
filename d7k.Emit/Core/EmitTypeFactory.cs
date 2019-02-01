using System;
using System.Reflection;
using System.Reflection.Emit;

namespace d7k.Emit
{
	public class EmitTypeFactory
	{
		public static ModuleBuilder DynamicModule { get; private set; }

		static EmitTypeFactory()
		{
			var ass = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName("DataFactoryCashAssembly_" + Guid.NewGuid().ToString()),
				AssemblyBuilderAccess.Run);

			DynamicModule = ass.DefineDynamicModule("data_m");
		}

		ModuleBuilder m_module;

		public EmitTypeFactory(ModuleBuilder module)
		{
			m_module = module;
		}

		public TypeBuilder CreateClass(string typeName, Type parent, params Type[] interfaces)
		{
			return m_module.DefineType(typeName, TypeAttributes.Class, parent, interfaces);
		}

		public TypeBuilder CreateInterface(string name, params Type[] interfaces)
		{
			return m_module.DefineType(name, TypeAttributes.Interface | TypeAttributes.Abstract, null, interfaces);
		}

		public ConstructorBuilder CreateConstructor(TypeBuilder type, Type[] args, ExecBld body)
		{
			var cnstr = type.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, args);
			body.Build(cnstr.GetILGenerator());
			return cnstr;
		}

		public void AddProperty(TypeBuilder type, PropertyInfo t, ExecBld get, ExecBld set, Type prototype)
		{
			var prop = type.DefineProperty(t.Name, PropertyAttributes.None, t.PropertyType, Type.EmptyTypes);
			var propSettings = MethodAttributes.Virtual;

			var getMeth = type.DefineMethod("get_" + t.Name, propSettings, t.PropertyType, Type.EmptyTypes);
			get.Build(getMeth.GetILGenerator());
			prop.SetGetMethod(getMeth);
			type.DefineMethodOverride(getMeth, prototype.GetMethod(getMeth.Name));

			var setMeth = type.DefineMethod("set_" + t.Name, propSettings, null, new[] { t.PropertyType });
			set.Build(setMeth.GetILGenerator());
			prop.SetSetMethod(setMeth);
			type.DefineMethodOverride(setMeth, prototype.GetMethod(setMeth.Name));
		}

		public void AddProperty(TypeBuilder type, string propertyName, Type propertyType, ExecBld get, ExecBld set, Type prototype)
		{
			var prop = type.DefineProperty(propertyName, PropertyAttributes.None, propertyType, Type.EmptyTypes);
			var propSettings = MethodAttributes.Virtual;

			var getMeth = type.DefineMethod("get_" + propertyName, propSettings, propertyType, Type.EmptyTypes);
			get.Build(getMeth.GetILGenerator());
			prop.SetGetMethod(getMeth);
			type.DefineMethodOverride(getMeth, prototype.GetMethod(getMeth.Name));

			var setMeth = type.DefineMethod("set_" + propertyName, propSettings, null, new[] { propertyType });
			set.Build(setMeth.GetILGenerator());
			prop.SetSetMethod(setMeth);
			type.DefineMethodOverride(setMeth, prototype.GetMethod(setMeth.Name));
		}

		public PropertyBuilder AddProperty(TypeBuilder type, string propertyName, Type propertyType)
		{
			var fieldName = "_" + propertyName;
			var field = type.DefineField(fieldName, propertyType, FieldAttributes.Private);
			var prop = type.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
			var propSettings = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;

			var getMeth = type.DefineMethod("get_" + propertyName, propSettings, propertyType, Type.EmptyTypes);
			var get = ExecBld.Return(ExecBld.GetFld(field, ExecBld.GetThis()));
			get.Build(getMeth.GetILGenerator());
			prop.SetGetMethod(getMeth);

			var setMeth = type.DefineMethod("set_" + propertyName, propSettings, null, new Type[] { propertyType });
			var set = ExecBld.Return(ExecBld.SetFld(field, ExecBld.GetThis(), ExecBld.GetArg(1)));
			set.Build(setMeth.GetILGenerator());
			prop.SetSetMethod(setMeth);

			return prop;
		}

		public PropertyBuilder AddAbstractProperty(TypeBuilder type, PropertyInfo t)
		{
			var pr = type.DefineProperty(t.Name, PropertyAttributes.None, t.PropertyType, null);

			var set = type.DefineMethod(
				"set_" + t.Name,
				MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Public,
				null, new[] { t.PropertyType });
			pr.SetSetMethod(set);

			var get = type.DefineMethod(
				"get_" + t.Name,
				MethodAttributes.Abstract | MethodAttributes.Virtual | MethodAttributes.SpecialName | MethodAttributes.Public,
				t.PropertyType, Type.EmptyTypes);
			pr.SetGetMethod(get);
			return pr;
		}

		public void CreateMethod(TypeBuilder type, string name, Type returnType, Type[] argType, Type interf, ExecBld body)
		{
			var equelMeth = type.DefineMethod(name, MethodAttributes.Virtual, returnType, argType);
			body.Build(equelMeth.GetILGenerator());
			if (interf != null)
				type.DefineMethodOverride(equelMeth, interf.GetMethod(name));
		}
	}
}
