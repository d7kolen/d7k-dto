using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace d7k.Emit
{
	class ExecBldItem
	{
		OpCode m_code;
		object m_arg;
		public LabelId Label { get; set; }

		public ExecBldItem(OpCode code)
			: this(code, null)
		{
		}

		public ExecBldItem(OpCode code, object arg)
		{
			m_code = code;
			m_arg = arg;
		}

		public void Emit(ILGenerator il, Dictionary<LabelId, Label> m_labels)
		{
			if (Label != null)
				il.MarkLabel(CreateLabel(il, m_labels, Label));

			if (m_arg == null)
				il.Emit(m_code);
			else if (m_arg is int)
				il.Emit(m_code, (int)m_arg);
			else if (m_arg is FieldInfo)
				il.Emit(m_code, (FieldInfo)m_arg);
			else if (m_arg is FieldBuilder)
				il.Emit(m_code, (FieldBuilder)m_arg);
			else if (m_arg is MethodInfo)
				il.Emit(m_code, (MethodInfo)m_arg);
			else if (m_arg is Type)
				il.Emit(m_code, (Type)m_arg);
			else if (m_arg is LabelId)
				il.Emit(m_code, CreateLabel(il, m_labels, (LabelId)m_arg));
			else
				throw new NotImplementedException();
		}

		Label CreateLabel(ILGenerator il, Dictionary<LabelId, Label> m_labels, LabelId label)
		{
			Label lbl;
			if (!m_labels.TryGetValue(label, out lbl))
				m_labels[label] = lbl = il.DefineLabel();
			return lbl;
		}

		public override string ToString()
		{
			return string.Format(
				"{0}{1}-{2}",
				Label != null ? Label.ToString() + ":" : "",
				m_code,
				m_arg != null ? m_arg.ToString() : "");
		}
	}
}
