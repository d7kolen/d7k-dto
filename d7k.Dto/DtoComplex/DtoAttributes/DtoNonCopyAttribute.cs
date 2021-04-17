using System;

namespace d7k.Dto
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	[Obsolete("Please use " + nameof(DtoNonCopyAttribute))]
	public class DtoIgnoreAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class DtoNonCopyAttribute : Attribute { }
}
