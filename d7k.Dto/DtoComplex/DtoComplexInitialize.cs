using d7k.Utilities.Monads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace d7k.Dto
{
	class DtoComplexInitialize
	{
		public Dictionary<Type, HashSet<Type>> Interfaces { get; private set; } = new Dictionary<Type, HashSet<Type>>();
		public Dictionary<Type, Dictionary<Type, ConvertMethodInfo>> Converters { get; private set; } = new Dictionary<Type, Dictionary<Type, ConvertMethodInfo>>();
		public Dictionary<Type, ValidationMethodInfo> Validators { get; private set; } = new Dictionary<Type, ValidationMethodInfo>();

		/// <summary>
		/// Load all Nested Dto STATIC classes containers which has dtoAttributes.<para/>
		/// Format of nested DTO containers should fit the InitByNestedClasses method format, because the method will load them actually.
		/// KnowAssemblyTypes parameter will help you upload an assembly which were alredy not uploaded. Never operations will do with them.
		/// When dtoAttributes parameter will has null value. Then we will use single DtoContainerAttribute for it.
		/// </summary>
		public void ByNestedClassesWithAttributes(Type[] dtoAttributes = null, Type[] knowAssemblyTypes = null)
		{
			if (dtoAttributes == null)
				dtoAttributes = new[] { typeof(DtoContainerAttribute) };

			var attributesHash = new HashSet<Type>(dtoAttributes);
			var types = new List<Type>();

			var scannedAssemblies = new HashSet<Assembly>();

			while (true)
			{
				var previousAssemblyCount = scannedAssemblies.Count;

				foreach (var tAssembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (!scannedAssemblies.Add(tAssembly))
						continue;

					try
					{
						foreach (var tType in tAssembly.GetTypes())
							if (
								tType.GetCustomAttributes().Where(x => attributesHash.Contains(x.GetType())).Any()
								&&
								tType.IsClass
								&&
								//static class components
								tType.IsSealed
								&&
								tType.IsAbstract)
							{
								types.Add(tType);
							}
					}
					catch (ReflectionTypeLoadException) { }
				}

				if (previousAssemblyCount == scannedAssemblies.Count)
					break;
			}

			if (types.Any())
				InitByNestedClasses(types.ToArray());
		}

		/// <summary>
		/// Find all types like this:<para/>
		/// class TChildClass : TBaseClass, IDtoInterface0, IDtoInterface1 { }<para/>
		/// <para/>
		/// These nested classes will use as discriptions for copying and validation rule selection.<para/>
		/// <para/>
		/// Also the method find methods like these:<para/>
		/// static void ValidateMethodName(ValidationRuleFactory/<IDtoInterface/> t)<para/>
		/// static void ValidateMethodName(ValidationRuleFactory/<IDtoInterface/> t, DtoComplex dto)<para/>
		/// <para/>
		/// These methods will use for DTO validation<para/>
		/// <para/>
		/// Also the Init method try find methods like these:<para/>
		/// static void ConvertMethodName(TDst dst, TSrc src)<para/>
		/// static void ConvertMethodName(TDst dst, TSrc src, DtoComplex dto)<para/>
		/// <para/>
		/// Validate and Convert methods cannot be generic<para/>
		/// If TDst is type then it should have a default constructor
		/// </summary>
		public void InitByNestedClasses(params Type[] classContainers)
		{
			foreach (var tType in classContainers)
			{
				var types = tType.GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Public);
				foreach (var t in types)
					if (t.IsClass)
					{
						if (!Interfaces.TryGetValue(t.BaseType, out HashSet<Type> interfaces))
							Interfaces[t.BaseType] = interfaces = new HashSet<Type>();
						interfaces.Load(t.GetInterfaces());
					}

				var methods = tType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
				foreach (var t in methods)
				{
					InitValidator(t);
					InitConverter(t);
				}
			}
		}

		void InitValidator(MethodInfo method)
		{
			if (method.GetCustomAttribute<DtoValidateAttribute>() == null)
				return;

			if (method.ReturnType != typeof(void) || method.IsGenericMethod)
				throw InvalidSignatureExceptionFactory.Create(method);

			var parameters = method.GetParameters();
			if (parameters.Length < 1 || parameters.Length > 2)
				throw InvalidSignatureExceptionFactory.Create(method);

			var tFactoryType = parameters[0].ParameterType;
			if (!tFactoryType.IsConstructedGenericType || tFactoryType.GetGenericTypeDefinition() != typeof(ValidationRuleFactory<>))
				throw InvalidSignatureExceptionFactory.Create(method);

			var dtoType = tFactoryType.GenericTypeArguments[0];

			Validators[dtoType] = new ValidationMethodInfo()
			{
				Method = method,
				FactoryType = parameters[0].ParameterType,
				ComplexRuleType = typeof(DtoComplexAdapterRule<>).MakeGenericType(dtoType)
			};

			if (parameters.Length == 2)
				Validators[dtoType].HasContext = true;
		}

		void InitConverter(MethodInfo method)
		{
			if (method.GetCustomAttribute<DtoConvertAttribute>() == null)
				return;

			if (method.ReturnType != typeof(void) || method.IsGenericMethod)
				throw InvalidSignatureExceptionFactory.Create(method);

			var parameters = method.GetParameters();
			if (parameters.Length < 2 || parameters.Length > 3)
				throw InvalidSignatureExceptionFactory.Create(method);

			var convertMethod = new ConvertMethodInfo()
			{
				Method = method,
				DstType = parameters[0].ParameterType,
				SrcType = parameters[1].ParameterType
			};

			if (parameters.Length == 3)
				convertMethod.HasContext = true;

			if (!Converters.TryGetValue(convertMethod.DstType, out Dictionary<Type, ConvertMethodInfo> tDstList))
				Converters[convertMethod.DstType] = new Dictionary<Type, ConvertMethodInfo>();

			Converters[convertMethod.DstType][convertMethod.SrcType] = convertMethod;
		}
	}
}
