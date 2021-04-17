using System;

namespace d7k.Dto
{
	/// <summary>
	/// The cast method will be used for Generic Type convertions.<para/>
	/// Available signatures:<para/>
	/// static TDst Cast(TSrc src)
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DtoCastAttribute : Attribute
	{
		public Type[] AvailableTypes { get; set; } = new Type[0];

		public DtoCastAttribute() { }

		public DtoCastAttribute(params Type[] availableTypes)
		{
			AvailableTypes = availableTypes;
		}
	}
}
