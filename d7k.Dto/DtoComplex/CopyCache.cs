using d7k.Utilities.Monads;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	class CopyCache
	{
		Dictionary<Type, HashSet<Type>> m_interfaces = new Dictionary<Type, HashSet<Type>>();
		Dictionary<Type, Dictionary<Type, ConvertMethodInfo>> m_converters = new Dictionary<Type, Dictionary<Type, ConvertMethodInfo>>();
		Dictionary<Type, Dictionary<Type, ConvertMethodInfo>> m_srcDstConverters = new Dictionary<Type, Dictionary<Type, ConvertMethodInfo>>();

		ConcurrentDictionary<string, Type[]> m_copyMap = new ConcurrentDictionary<string, Type[]>();
		ConcurrentDictionary<string, ConvertMethodInfo[]> m_converterMap = new ConcurrentDictionary<string, ConvertMethodInfo[]>();
		ConcurrentDictionary<Type, HashSet<Type>> m_factInterfaces = new ConcurrentDictionary<Type, HashSet<Type>>();
		ConcurrentDictionary<Type, HashSet<Type>> m_hierarchyTypes = new ConcurrentDictionary<Type, HashSet<Type>>();

		public CopyCache(Dictionary<Type, HashSet<Type>> interfaces, Dictionary<Type, Dictionary<Type, ConvertMethodInfo>> converters)
		{
			m_interfaces = interfaces;
			m_converters = converters;
		}

		public HashSet<Type> GetInterfaces(Type src)
		{
			return m_factInterfaces.GetOrAdd(src, x =>
			{
				var srcInterfaces = m_interfaces.Get(src);
				if (srcInterfaces != null)
					return srcInterfaces;

				return new HashSet<Type>(src.GetInterfaces());
			});
		}

		public ConvertMethodInfo[] GetConverters(object dst, object src)
		{
			var key = CacheKey(dst, src);
			return m_converterMap.GetOrAdd(key, x => GetConvertersInternal(dst, src));
		}

		public Type[] GetCopingInterfaces(object dst, object src)
		{
			var key = CacheKey(dst, src);
			return m_copyMap.GetOrAdd(key, x => GetCopingInterfacesInternal(dst, src));
		}

		private ConvertMethodInfo[] GetConvertersInternal(object dst, object src)
		{
			var srcInterfaces = GetInterfacesWithHierarchy(src);
			var dstInterfaces = GetInterfacesWithHierarchy(dst);

			var result = new List<ConvertMethodInfo>();

			foreach (var tSrc in srcInterfaces)
			{
				foreach (var tDst in dstInterfaces)
				{
					var dstConverters = m_converters.Get(tDst);
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

		private List<Type> GetInterfacesWithHierarchy(object src)
		{
			return GetInterfaces(src.GetType()).Union(HierarchyTypes(src.GetType())).ToList();
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

		private Type[] GetCopingInterfacesInternal(object dst, object src)
		{
			var srcInterfaces = GetInterfacesWithHierarchy(src);
			var dstInterfaces = GetInterfacesWithHierarchy(dst);

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
