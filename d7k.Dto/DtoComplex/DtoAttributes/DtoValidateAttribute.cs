using System;

namespace d7k.Dto
{
	/// <summary>
	/// Available signatures:<para/>
	/// static void Validate(ValidationRuleFactory&lt;TSrc&gt; t)<para/>
	/// static void Validate(ValidationRuleFactory&lt;TSrc&gt; t, DtoComplex dto)<para/>
	/// Can have generic signature:<para/>
	/// static void Validate&lt;T&gt;(ValidationRuleFactory&lt;TSrc&lt;T&gt;&gt; t)<para/>
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class DtoValidateAttribute : Attribute { }
}
