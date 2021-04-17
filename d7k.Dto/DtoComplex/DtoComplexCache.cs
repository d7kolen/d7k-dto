using d7k.Dto.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto.Complex
{
	class DtoComplexCache
	{
		DtoComplexState m_state;

		ConcurrentDictionary<string, Type[]> m_copyMap = new ConcurrentDictionary<string, Type[]>();
		ConcurrentDictionary<string, GenericTypePair[]> m_genericsMap = new ConcurrentDictionary<string, GenericTypePair[]>();
		ConcurrentDictionary<string, GenericTypePair[]> m_genericsAllMap = new ConcurrentDictionary<string, GenericTypePair[]>();
		ConcurrentDictionary<string, ConvertMethodInfo[]> m_converterMap = new ConcurrentDictionary<string, ConvertMethodInfo[]>();
		ConcurrentDictionary<string, ConvertMethodInfo[]> m_genericConverterMap = new ConcurrentDictionary<string, ConvertMethodInfo[]>();
		ConcurrentDictionary<Type, HashSet<Type>> m_interfaceTypes = new ConcurrentDictionary<Type, HashSet<Type>>();
		ConcurrentDictionary<Type, HashSet<Type>> m_hierarchyTypes = new ConcurrentDictionary<Type, HashSet<Type>>();
		ConcurrentDictionary<Type, Type[]> m_interfaceAndHierarchyTypes = new ConcurrentDictionary<Type, Type[]>();

		public DtoComplexCache(DtoComplexState state)
		{
			m_state = state;
		}

		public HashSet<Type> GetInterfaces(Type src)
		{
			return m_interfaceTypes.GetOrAdd(src, x =>
			{
				return GetInterfacesInternal(x);
			});
		}

		private HashSet<Type> GetInterfacesInternal(Type x)
		{
			var accum = new HashSet<Type>();

			foreach (var tType in HierarchyTypes(x))
			{
				foreach (var tIt in m_state.Interfaces.Get(tType) ?? Enumerable.Empty<Type>())
					accum.Add(tIt);

				foreach (var tIt in tType.GetInterfaces())
					accum.Add(tIt);
			}

			return accum;
		}

		public Type[] GetInterfacesWithHierarchy(Type srcType)
		{
			return m_interfaceAndHierarchyTypes.GetOrAdd(srcType,
				x => GetInterfacesWithHierarchyInternal(srcType));
		}

		private Type[] GetInterfacesWithHierarchyInternal(Type srcType, bool withIgnore = false)
		{
			var result = GetInterfacesInternal(srcType)
				.Union(HierarchyTypes(srcType));

			if (!withIgnore)
				result = result.Where(t => t.GetCustomAttributes(typeof(DtoNonCopyAttribute), false)?.Any() != true);

			return result.ToArray();
		}

		public ConvertMethodInfo[] GetConverters(object dst, object src)
		{
			var key = CacheKey(dst, src);
			return m_converterMap.GetOrAdd(key, x => GetConvertersInternal(dst, src));
		}

		private ConvertMethodInfo[] GetConvertersInternal(object dst, object src)
		{
			var srcInterfaces = GetInterfacesWithHierarchyInternal(src.GetType(), true);
			var dstInterfaces = GetInterfacesWithHierarchyInternal(dst.GetType(), true);

			var result = new List<ConvertMethodInfo>();

			foreach (var tSrc in srcInterfaces)
			{
				foreach (var tDst in dstInterfaces)
				{
					var dstConverters = m_state.Converters.Get(tDst);
					if (dstConverters == null)
						continue;

					var exactConvert = dstConverters.Get(tSrc);
					if (exactConvert != null)
					{
						result.Add(exactConvert);
						continue;
					}
				}
			}

			return result.ToArray();
		}

		public ConvertMethodInfo[] GetGenericConverters(GenericTypePair par)
		{
			var name = DtoComplexState.GetGenericConverterName(par.DstType.GUID, par.DstType, par.SrcType);
			return m_genericConverterMap.GetOrAdd(name, t =>
			{
				var result = new List<List<GenericConvertMethodInfo>>();
				var emptyName = DtoComplexState.GetGenericConverterName(par.DstType.GUID, null, null);
				result.Add(m_state.GenericConverters.Get(emptyName));

				var nameWithoutDst = DtoComplexState.GetGenericConverterName(par.DstType.GUID, null, par.SrcType);
				result.Add(m_state.GenericConverters.Get(nameWithoutDst));

				var nameWithoutSrc = DtoComplexState.GetGenericConverterName(par.DstType.GUID, par.DstType, null);
				result.Add(m_state.GenericConverters.Get(nameWithoutSrc));

				var tResult = result
					.Where(x => x != null)
					.SelectMany(x => x)
					.Where(x => x.AvailableForFilters(par.DstType, par.SrcType, this))
					.Select(x => x.CreateMethod(par.DstType, par.SrcType)).ToArray();

				return tResult.Any() ? tResult : null;
			});
		}

		private HashSet<Type> HierarchyTypes(Type objType)
		{
			return m_hierarchyTypes.GetOrAdd(objType, x =>
			{
				var result = new HashSet<Type>();

				while (true)
				{
					result.Add(x);
					if (x.BaseType == null)
						break;
					x = x.BaseType;
				}

				return result;
			});
		}

		public class GenericTypePair
		{
			public Type DstType { get; set; }
			public Type SrcType { get; set; }
		}

		public GenericTypePair[] GetActualGenerics(object dst, object src)
		{
			var key = CacheKey(dst, src);
			return m_genericsMap.GetOrAdd(key, x => GetGenericsInternal(dst, src, false).ToArray());
		}

		public GenericTypePair[] GetAllGenerics(object dst, object src)
		{
			var key = CacheKey(dst, src);
			return m_genericsAllMap.GetOrAdd(key, x => GetGenericsInternal(dst, src, true).ToArray());
		}

		private List<GenericTypePair> GetGenericsInternal(object dst, object src, bool withIgnore)
		{
			var srcInterfaces = GetInterfacesWithHierarchyInternal(src.GetType(), withIgnore)
				?.Where(x => x.IsGenericType)
				.Select(x => new { t = x, g = x.GetGenericTypeDefinition() })
				.ToList();

			var dstInterfaces = GetInterfacesWithHierarchyInternal(dst.GetType(), withIgnore)
				?.Where(x => x.IsGenericType)
				.Select(x => new { t = x, g = x.GetGenericTypeDefinition() })
				.ToList();

			if (dstInterfaces == null || srcInterfaces == null)
				return new List<GenericTypePair>();

			var accum = new List<GenericTypePair>();

			foreach (var tSrc in srcInterfaces)
				foreach (var tDst in dstInterfaces.Where(x => x.g == tSrc.g))
					accum.Add(new GenericTypePair() { SrcType = tSrc.t, DstType = tDst.t });

			return accum;
		}

		public Type[] GetCopingInterfaces(object dst, object src)
		{
			var key = CacheKey(dst, src);
			return m_copyMap.GetOrAdd(key, x => GetCopingInterfacesInternal(dst, src));
		}

		private Type[] GetCopingInterfacesInternal(object dst, object src)
		{
			var srcInterfaces = GetInterfacesWithHierarchyInternal(src.GetType());
			var dstInterfaces = GetInterfacesWithHierarchyInternal(dst.GetType());

			if (dstInterfaces == null || srcInterfaces == null)
				return new Type[0];

			return dstInterfaces.Where(t => srcInterfaces.Contains(t)).ToArray();
		}

		private static string CacheKey(object dst, object src)
		{
			return dst.GetType().FullName + "$^&#@#&^$" + src.GetType().FullName;
		}
	}
}
