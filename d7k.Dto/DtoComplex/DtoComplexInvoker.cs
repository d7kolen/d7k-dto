using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using d7k.Dto.Complex;
using d7k.Dto.Utilities;

namespace d7k.Dto
{
	class DtoComplexInvoker
	{
		DtoComplexState m_state = new DtoComplexState();
		DtoComplex m_complex;
		DtoComplexCache m_copyCache;
		ConcurrentDictionary<Type, ValidationMethodInfo[]> m_validatorsMap = new ConcurrentDictionary<Type, ValidationMethodInfo[]>();
		DtoCopier m_copier;
		public DtoComplexCastInvoker Cast { get; }

		public DtoComplexInvoker(DtoComplexCache copyCache, DtoComplexState state, DtoComplex complex)
		{
			m_copyCache = copyCache;
			m_state = state;
			m_complex = complex;

			m_copier = new DtoCopier(m_state.Casts);
			Cast = new DtoComplexCastInvoker(m_copier);
		}

		public void ForConverters(object dst, object src)
		{
			var converters = m_copyCache.GetConverters(dst, src);

			foreach (var t in converters)
				t.Invoke(dst, src, m_complex, null);
		}

		public void ForGenericsConverters(object dst, object src, HashSet<string> updationList)
		{
			var generics = m_copyCache.GetAllGenerics(dst, src);

			var updationListWriter = m_copier.Updater(updationList);

			foreach (var tGen in generics)
			{
				var converters = m_copyCache.GetGenericConverters(tGen);
				if (converters == null)
					continue;

				foreach (var tConv in converters)
					tConv.Invoke(dst, src, m_complex, updationListWriter);
			}
		}

		public void ForConverters(object dst, object src, HashSet<string> updationList)
		{
			var updationListWriter = m_copier.Updater(updationList);

			var converters = m_copyCache.GetConverters(dst, src);
			foreach (var t in converters)
				t.Invoke(dst, src, m_complex, updationListWriter);
		}

		public void ForGenerics(object dst, object src, HashSet<string> updationList = null)
		{
			var updationListWriter = m_copier.Updater(updationList);

			var generics = m_copyCache.GetActualGenerics(dst, src);
			foreach (var t in generics)
				updationListWriter.Copy(dst, t.DstType, src, t.SrcType);
		}

		public void ForCopiers(object dst, object src, HashSet<string> updationList = null)
		{
			var updationListWriter = m_copier.Updater(updationList);

			var copiers = m_copyCache.GetCopingInterfaces(dst, src);

			foreach (var t in copiers)
				updationListWriter.Copy(dst, src, t);
		}

		public void ForSame(object dst, object src)
		{
			m_copier.Copy(dst, src, src.GetType(), false, null);
		}

		public ValidationMethodInfo[] GetValidators(Type sourceType)
		{
			return m_validatorsMap.GetOrAdd(sourceType, tType =>
			{
				var validators = new List<ValidationMethodInfo>();

				var interfaces = m_copyCache.GetInterfacesWithHierarchy(tType);

				var interfValidators = interfaces
					.Select(x => m_state.Validators.Get(x))
					.Where(x => x != null);

				var generics = new List<ValidationMethodInfo>();
				foreach (var t in interfaces)
				{
					if (!t.IsGenericType)
						continue;

					var tValidator = m_state.GenericValidators.Get(t.GetGenericTypeDefinition().ToString());
					if (tValidator == null)
						continue;

					tValidator = tValidator.Clone().InitTypes(t);
					tValidator.Method = tValidator.Method.MakeGenericMethod(t.GenericTypeArguments);
					generics.Add(tValidator);
				}

				return interfValidators.Union(generics).ToArray();
			});
		}
	}
}
