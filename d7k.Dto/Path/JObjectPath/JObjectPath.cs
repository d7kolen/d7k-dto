using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace d7k.Dto
{
	public class JObjectPath : IPath<JObject>
	{
		public const string c_scanArrayIndex = ":#$%scanArrayIndex%$#";

		string[] m_fields;

		public JObjectPath(JObjectFieldsBuilder fields)
		{
			m_fields = fields.Accum.ToArray();
		}

		public JObjectPath(Func<JObjectFieldsBuilder, JObjectFieldsBuilder> fields)
		{
			m_fields = fields(new JObjectFieldsBuilder()).Accum.ToArray();
		}

		public void PrepareForIndexing()
		{
		}

		public object Get(JObject source, object[] index)
		{
			JToken current = source;

			foreach (var t in index)
				current = current[t];

			if (current is JArray arrCurrent)
			{
				var accum = new object[arrCurrent.Count];
				for (int i = 0; i < arrCurrent.Count; i++)
				{
					var t = arrCurrent[i];
					accum[i] = t is JValue ? (t as JValue).Value : t;
				}
				return accum;
			}

			return current is JValue ? (current as JValue).Value : current;
		}

		public void Set(JObject source, object[] index, object value)
		{
			JToken current = source;

			foreach (var t in index.Take(index.Length - 1))
				current = current[t];

			JToken jValue;
			if (value is JObject)
				jValue = (JObject)value;
			else if (value is JArray)
				jValue = (JArray)value;
			else if (value is Array)
				jValue = JArray.FromObject(value);
			else
				jValue = new JValue(value);

			var lastIndex = index[index.Length - 1];

			if (current is JObject objCurrent)
				objCurrent[lastIndex] = jValue;

			else if (current is JArray arrCurrent)
				arrCurrent[lastIndex] = jValue;

			else
				throw new NotImplementedException();
		}

		public IEnumerable<object[]> GetAllIndexes(JObject source)
		{
			return GetIndexesForPrefix(source, 0);
		}

		IEnumerable<object[]> GetIndexesForPrefix(JToken source, int nextFieldIndex)
		{
			if (m_fields.Length == nextFieldIndex)
			{
				yield return new object[0];
				yield break;
			}

			var nextFieldName = m_fields[nextFieldIndex];

			if (source is JArray sourceArr && nextFieldName == c_scanArrayIndex)
			{
				for (int i = 0; i < sourceArr.Count; i++)
				{
					var iArr = new object[] { i };

					var result = GetIndexesForPrefix(sourceArr[i], nextFieldIndex + 1);
					foreach (var t in result)
						yield return new[] { iArr, t }.SelectMany(x => x).ToArray();
				}

				yield break;
			}

			if (source is JObject && nextFieldName != c_scanArrayIndex)
			{
				var fieldArr = new[] { nextFieldName };

				var result = GetIndexesForPrefix(source[nextFieldName], nextFieldIndex + 1);
				foreach (var t in result)
					yield return new[] { fieldArr, t }.SelectMany(x => x).ToArray();

				yield break;
			}
		}

		public string PathName()
		{
			if (!m_fields.Any())
				return nameof(JObject);

			var accum = new StringBuilder(nameof(JObject)).Append(" : ");
			foreach (var t in m_fields)
				if (t == c_scanArrayIndex)
					accum.Append("[]").Append(".");
				else
					accum.Append(t).Append(".");

			if (accum.Length > 0)
				accum.Length--;

			return accum.ToString();
		}

		public string IndexedName(object[] index)
		{
			if (!m_fields.Any())
				return "";

			var accum = new StringBuilder("");
			for (int i = 0; i < m_fields.Length; i++)
			{
				var tField = m_fields[i];
				if (tField == c_scanArrayIndex)
					accum.Append("[").Append(index[i]).Append("]").Append(".");
				else
					accum.Append(tField).Append(".");
			}

			if (accum.Length > 0)
				accum.Length--;

			return accum.ToString();
		}
	}
}
