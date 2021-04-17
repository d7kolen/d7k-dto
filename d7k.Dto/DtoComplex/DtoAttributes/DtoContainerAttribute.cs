using System;

namespace d7k.Dto
{
	/// <summary>
	/// Default DTO container. Owner class should be STATIC.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DtoContainerAttribute : Attribute
	{
		Type[] m_knownTypes = new Type[0];

		public DtoContainerAttribute()
		{
		}

		public DtoContainerAttribute(params Type[] knownTypes)
		{
			m_knownTypes = knownTypes;
		}
	}
}
