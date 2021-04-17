using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using d7k.Dto.Complex;
using d7k.Dto.Utilities;

namespace d7k.Dto
{
	class DtoComplexState
	{
		public Dictionary<Type, HashSet<Type>> Interfaces { get; } = new Dictionary<Type, HashSet<Type>>();
		public Dictionary<Type, Dictionary<Type, ConvertMethodInfo>> Converters { get; } = new Dictionary<Type, Dictionary<Type, ConvertMethodInfo>>();
		public Dictionary<string, List<GenericConvertMethodInfo>> GenericConverters { get; } = new Dictionary<string, List<GenericConvertMethodInfo>>();
		public Dictionary<Type, ValidationMethodInfo> Validators { get; } = new Dictionary<Type, ValidationMethodInfo>();
		public Dictionary<string, ValidationMethodInfo> GenericValidators { get; } = new Dictionary<string, ValidationMethodInfo>();
		public DtoCopierCastStorage Casts { get; } = new DtoCopierCastStorage();

		/// <summary>
		/// Load all Nested Dto STATIC classes containers which have dtoAttributes.<para/>
		/// Format of nested DTO containers should fit the InitByNestedClasses method format, because the method will load them actually.
		/// KnownAssemblyTypes parameter will help you upload assemblies which haven't uploaded yet. Never operations will do with them.
		/// When dtoAttributes parameter will has null value. Then we will use single DtoContainerAttribute for it.
		/// </summary>
		public void ByNestedClassesWithAttributes(Type[] dtoAttributes = null, Type[] knownAssemblyTypes = null)
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
					catch (ReflectionTypeLoadException e) { }
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
		/// Also the method find methods described for DtoAttributes (DtoCastAttribute, DtoConvertAttribute, DtoValidateAttribute)
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
					InitGenericConverter(t);
					InitCast(t);
				}
			}
		}

		void InitValidator(MethodInfo method)
		{
			if (method.GetCustomAttribute<DtoValidateAttribute>() == null)
				return;

			if (method.ReturnType != typeof(void))
				throw InvalidSignatureExceptionFactory.Create(method);

			var parameters = method.GetParameters();
			if (parameters.Length < 1 || parameters.Length > 2)
				throw InvalidSignatureExceptionFactory.Create(method);

			var tFactoryType = parameters[0].ParameterType;
			if (!tFactoryType.IsConstructedGenericType || tFactoryType.GetGenericTypeDefinition() != typeof(ValidationRuleFactory<>))
				throw InvalidSignatureExceptionFactory.Create(method);

			var dtoType = tFactoryType.GenericTypeArguments[0];

			ValidationMethodInfo tValidator;

			if (method.IsGenericMethod)
			{
				GenericValidators[dtoType.ToString()] = tValidator = new ValidationMethodInfo()
				{
					Method = method,
				};
			}
			else
			{
				Validators[dtoType] = tValidator = new ValidationMethodInfo()
				{
					Method = method,
				}.InitTypes(dtoType);
			}

			if (parameters.Length == 2)
				tValidator.HasContext = true;
		}

		void InitConverter(MethodInfo method)
		{
			if (method.GetCustomAttribute<DtoConvertAttribute>() == null)
				return;

			if (method.IsGenericMethod)
				return;

			if (method.ReturnType != typeof(void))
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

		void InitGenericConverter(MethodInfo method)
		{
			if (method.GetCustomAttribute<DtoConvertAttribute>() == null)
				return;

			if (!method.IsGenericMethod)
				return;

			if (method.ReturnType != typeof(void))
				throw InvalidSignatureExceptionFactory.Create(method);

			var parameters = method.GetParameters();

			if (parameters.Length < 2 || parameters.Length > 3)
				throw InvalidSignatureExceptionFactory.Create(method);

			if (parameters[0].ParameterType.GUID != parameters[1].ParameterType.GUID)
				throw InvalidSignatureExceptionFactory.Create(method);

			var convertMethod = new GenericConvertMethodInfo()
			{
				Method = method,
				TemplateType = parameters[0].ParameterType
			};

			//var parameterTypeUId = convertMethod.TemplateType.GUID;

			if (parameters.Length == 3)
				convertMethod.HasContext = true;

			var methodPars = method.GetParameters();
			var converterName = GetGenericConverterName(convertMethod.TemplateType.GUID, methodPars[0].ParameterType, methodPars[1].ParameterType);

			if (!GenericConverters.TryGetValue(converterName, out List<GenericConvertMethodInfo> tList))
				GenericConverters[converterName] = new List<GenericConvertMethodInfo>();

			GenericConverters[converterName].Add(convertMethod);
		}

		public static string GetGenericConverterName(Guid templateTypeId, Type dstType, Type srcType)
		{
			const string separator = "$%^#^%$";

			var accum = new StringBuilder();

			accum.Append(templateTypeId).Append(separator);

			if (dstType != null && !dstType.ContainsGenericParameters)
				accum.Append(string.Join("|", dstType.GetGenericArguments().Select(x => x.FullName)));

			accum.Append(separator);

			if (srcType != null && !srcType.ContainsGenericParameters)
				accum.Append(string.Join("|", srcType.GetGenericArguments().Select(x => x.FullName)));

			return accum.ToString();
		}

		void InitCast(MethodInfo method)
		{
			var castAttr = method.GetCustomAttribute<DtoCastAttribute>();
			if (castAttr == null)
				return;

			var error = DtoCopierCastStorage.CheckMethod(method);
			if (error != null)
				throw InvalidSignatureExceptionFactory.Create(method);

			Casts.Append(method, castAttr.AvailableTypes);
		}
	}
}
