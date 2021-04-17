using System;

namespace d7k.Dto.Emit
{
	class LabelId
	{
		Guid m_label;

		public LabelId()
		{
			m_label = Guid.NewGuid();
		}

		public override string ToString()
		{
			return m_label.ToString();
		}
	}
}
