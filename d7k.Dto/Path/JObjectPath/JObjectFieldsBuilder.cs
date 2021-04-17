using System.Collections.Generic;

namespace d7k.Dto
{
	public class JObjectFieldsBuilder
	{
		public List<string> Accum { get; private set; } = new List<string>();

		public JObjectFieldsBuilder this[string index]
		{
			get
			{
				Accum.Add(index);
				return this;
			}
		}

		public JObjectFieldsBuilder ScanArr
		{
			get
			{
				Accum.Add(JObjectPath.c_scanArrayIndex);
				return this;
			}
		}

		public JObjectFieldsBuilder<TProperty> Type<TProperty>()
		{
			return new JObjectFieldsBuilder<TProperty>() { Accum = Accum };
		}
	}

	public class JObjectFieldsBuilder<TProperty> : JObjectFieldsBuilder
	{
	}
}