using System;
using System.Reflection;

namespace d7k.Dto
{
	public class PathItem
	{
		public MemberInfo Property { get; set; }
		public Type Array { get; set; }
	}
}
