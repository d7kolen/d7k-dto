using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace d7k.Dto.Emit
{
	public class ExecBld
	{
		IEnumerable<ExecBldItem> m_items;

		ExecBld(ExecBldItem item)
		{
			m_items = new[] { item };
		}

		ExecBld(ExecBldItem item, IEnumerable<ExecBldItem> items)
		{
			var data = items.ToList();
			data.Add(item);
			m_items = data;
		}

		ExecBld(IEnumerable<ExecBldItem> items)
		{
			m_items = items.ToList();
		}

		public static ExecBld Return()
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ret));
		}

		public static ExecBld Return(ExecBld src)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ret), src.m_items);
		}

		public static ExecBld GetNull()
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ldnull));
		}

		public static ExecBld GetArg(int index)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ldarg, index));
		}

		public static ExecBld GetThis()
		{
			return GetArg(0);
		}

		public static ExecBld GetConst(int value)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ldc_I4, value));
		}

		public static ExecBld GetConst(bool value)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ldc_I4, value ? 1 : 0));
		}

		public static ExecBld GetFld(FieldInfo field, ExecBld src)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ldfld, field), src.m_items);
		}

		public static ExecBld GetFld(FieldBuilder field, ExecBld src)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ldfld, field), src.m_items);
		}

		public static ExecBld SetFld(FieldInfo field, ExecBld src, ExecBld value)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Stfld, field), src.m_items.Union(value.m_items));
		}

		public static ExecBld SetFld(FieldBuilder field, ExecBld src, ExecBld value)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Stfld, field), src.m_items.Union(value.m_items));
		}

		public static ExecBld GetProp(PropertyInfo prop, ExecBld src)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Callvirt, prop.GetGetMethod(true)), src.m_items);
		}

		public static ExecBld GetProp(PropertyBuilder prop, ExecBld src)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Callvirt, prop.GetGetMethod(true)), src.m_items);
		}

		public static ExecBld SetProp(PropertyInfo prop, ExecBld src, ExecBld value)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Callvirt, prop.GetSetMethod(true)), src.m_items.Union(value.m_items));
		}

		public static ExecBld SetProp(PropertyBuilder prop, ExecBld src, ExecBld value)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Callvirt, prop.GetSetMethod(true)), src.m_items.Union(value.m_items));
		}

		public static ExecBld GetLoc(int index)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ldloc, index));
		}

		public static ExecBld SetLoc(int index, ExecBld value)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Stloc, index), value.m_items);
		}

		public static ExecBld GetItem<T>(ExecBld src, ExecBld index)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Ldelem, typeof(T)), src.m_items.Union(index.m_items));
		}

		public static ExecBld SetItem<T>(ExecBld src, ExecBld index, ExecBld value)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Stelem, typeof(T)), src.m_items.Union(index.m_items).Union(value.m_items));
		}

		public static ExecBld Block(params ExecBld[] bldrs)
		{
			return new ExecBld(bldrs.SelectMany(x => x.m_items));
		}

		public static ExecBld Call(MethodInfo method, params ExecBld[] bldrs)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Callvirt, method), bldrs.SelectMany(x => x.m_items));
		}

		public static ExecBld CallStat(MethodInfo method, params ExecBld[] bldrs)
		{
			return new ExecBld(new ExecBldItem(OpCodes.Call, method), bldrs.SelectMany(x => x.m_items));
		}

		public static ExecBld Cast(ExecBld src, Type tSrc, Type tDst)
		{
			if (!tSrc.IsSubclassOf(typeof(ValueType)) && tDst.IsSubclassOf(typeof(ValueType)))
				return new ExecBld(new ExecBldItem(OpCodes.Unbox_Any, tDst), src.m_items);
			else if (!tSrc.IsSubclassOf(typeof(ValueType)) && !tDst.IsSubclassOf(typeof(ValueType)))
				return new ExecBld(new ExecBldItem(OpCodes.Castclass, tDst), src.m_items);
			else if (tSrc.IsSubclassOf(typeof(ValueType)) && !tDst.IsSubclassOf(typeof(ValueType)))
				return new ExecBld(new ExecBldItem(OpCodes.Box, tSrc), src.m_items);
			else
				return Cast(Cast(src, tSrc, typeof(object)), typeof(object), tDst);
		}

		public void Build(ILGenerator il)
		{
			var labels = new Dictionary<LabelId, Label>();

			foreach (var t in m_items)
				t.Emit(il, labels);
		}
	}
}
