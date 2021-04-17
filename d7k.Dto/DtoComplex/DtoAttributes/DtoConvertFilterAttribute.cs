using System;

namespace d7k.Dto
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class DtoConvertFilterAttribute : Attribute
	{
		public Type[] AvailableTemplateTypes { get; set; }

		public DtoConvertFilterAttribute() { }

		/// <summary>
		/// Empty availableTemplateTypes means any Generic parameter set is available.
		/// </summary>
		public DtoConvertFilterAttribute(params Type[] availableTemplateTypes)
		{
			AvailableTemplateTypes = availableTemplateTypes ?? new Type[0];
		}
	}
}
