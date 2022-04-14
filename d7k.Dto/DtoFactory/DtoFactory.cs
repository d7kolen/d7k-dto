using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using d7k.Dto.Emit;

namespace d7k.Dto
{
	public static class DtoFactory
	{
		static readonly EmitTypeFactory s_factory = new EmitTypeFactory(EmitTypeFactory.DynamicModule);

		static readonly ConcurrentDictionary<string, IDtoAdapterFactory> s_factories = new ConcurrentDictionary<string, IDtoAdapterFactory>();
		static readonly ConcurrentDictionary<string, PureDtoFactory> s_dtoFactories = new ConcurrentDictionary<string, PureDtoFactory>();

		public static T Dto<T>()
		{
			return (T)Dto(typeof(T));
		}

		public static object Dto(Type interfType)
		{
			var factory = s_dtoFactories.GetOrAdd(interfType.FullName, x =>
			{
				PureDtoFactory res;

				try
				{
				}
				finally
				{
					AssertInterface(interfType);
					res = new PureDtoFactory(s_factory, interfType);
				}

				return res;
			});

			return factory.Create();
		}

		public static T DtoAdapter<T>(this object source)
		{
			return (T)DtoAdapter(source, typeof(T));
		}

		public static object DtoAdapter(this object source, Type interfType)
		{
			if (source.GetType().GetInterfaces().Contains(interfType))
				return source;

			if (source is IDtoAdapter)
				source = ((IDtoAdapter)source).GetSource();

			var sourceType = source.GetType();
			var key = interfType.FullName + "#$%^%$#" + sourceType.FullName;

			var fact = s_factories.GetOrAdd(key, x =>
			{
				IDtoAdapterFactory res = null;

				try
				{
				}
				finally
				{
					//ignore ThreadAbortException

					AssertInterface(interfType);
					AssertMapping(interfType, sourceType);

					if (sourceType.IsPublic)
						res = new PublicDtoAdapterFactory(s_factory, sourceType, interfType);
					if (sourceType.IsSealed && sourceType.Name.Contains("f__AnonymousType"))
						res = new AnonymousDtoAdapterFactory(s_factory, sourceType, interfType);
					else
						res = new InternalDtoAdapterFactory(s_factory, sourceType, interfType);
				}

				return res;
			});

			return fact.Create(source);
		}

		static void AssertInterface(Type interf)
		{
			if (!interf.IsInterface)
				throw new InvalidOperationException($"{interf.FullName} type should be interface.");

			if (!interf.IsNested && !interf.IsPublic)
				throw new InvalidOperationException($"Interface {interf.FullName} type should be public.");

			if (interf.IsNested && (!interf.IsNestedPublic || !interf.ReflectedType.IsPublic))
				throw new InvalidOperationException($"Interface {interf.FullName} type should be public.");

			if (interf.GetMethods().Where(x => !x.IsSpecialName).Any())
				throw new InvalidOperationException($"Interface {interf.FullName} cannot have any methods.");
		}

		static void AssertMapping(Type interf, Type sourceType)
		{
			foreach (var t in interf.GetProperties())
			{
				var tSrcProp = sourceType.GetProperty(t.Name);//, t.PropertyType);
				if (tSrcProp == null)
				{
					throw new InvalidOperationException($"Source property {t.Name} doesn't exist.");
					//throw new InvalidOperationException($"Source property {t.Name} with type {t.PropertyType.FullName} is not exist in {sourceType.GetType().FullName}.");
				}
			}
		}

		public static IEnumerable<TDst> Adapters<TDst>(this System.Collections.IEnumerable source)
		{
			foreach (var t in source)
				yield return DtoAdapter<TDst>(t);
		}
	}
}
