using System;
using System.Collections.Generic;

namespace d7k.Utilities
{
	public class EnumAttributes
	{
		static Dictionary<object, Dictionary<Guid, Attribute>> s_cache = new Dictionary<object, Dictionary<Guid, Attribute>>();

		public static TAttr GetAttribute<TAttr>(object enumItem)
			where TAttr : Attribute
		{
			var res = GetAttributeNoLock<TAttr>(enumItem);
			if (res != null)
				return res;

			lock (s_cache)
			{
				res = GetAttributeNoLock<TAttr>(enumItem);
				if (res != null)
					return res;

				var memInfo = enumItem.GetType().GetMember(enumItem.ToString());
				if (memInfo.Length == 0)
					return SetAttribute<TAttr>(enumItem, null);

				var attributes = memInfo[0].GetCustomAttributes(typeof(TAttr), false);
				if (attributes.Length == 0)
					return SetAttribute<TAttr>(enumItem, null);

				return SetAttribute<TAttr>(enumItem, (TAttr)attributes[0]);
			}
		}
		
		static TAttr GetAttributeNoLock<TAttr>(object enumItem) where TAttr : Attribute
		{
			Dictionary<Guid, Attribute> itemAttrs;

			if (s_cache.TryGetValue(enumItem, out itemAttrs))
			{
				Attribute attr;
				if (itemAttrs.TryGetValue(typeof(TAttr).GUID, out attr))
					return attr as TAttr;
			}

			return null;
		}

		static TAttr SetAttribute<TAttr>(object enumItem, TAttr value) where TAttr : Attribute
		{
			Dictionary<Guid, Attribute> itemAttrs;

			if (!s_cache.TryGetValue(enumItem, out itemAttrs))
				s_cache[enumItem] = itemAttrs = new Dictionary<Guid, Attribute>();

			Attribute attr;
			if (itemAttrs.TryGetValue(typeof(TAttr).GUID, out attr))
				itemAttrs[typeof(TAttr).GUID] = value;

			return value;
		}
	}
}
