namespace d7k.Dto
{
	public static class ValidationRuleFactoryHelper
	{
		public static TSource Update<TSource>(this ValidationRuleFactory<TSource> factory, TSource source)
		{
			var context = new ValidationContext() { WithUpdate = true };
			factory.ValidateObject(source, context);
			return source;
		}

		public static ValidationResult Validate<TSource>(this ValidationRuleFactory<TSource> factory, TSource source)
		{
			return factory.ValidateObject(source, new ValidationContext());
		}

		public static TSource Fix<TSource>(this ValidationRuleFactory<TSource> factory, TSource sources)
		{
			factory.ValidateObject(sources, new ValidationContext()).ThrowIssues();
			factory.Update(sources);

			return sources;
		}

		/// <summary>
		/// The method return example of TSource type. Be carefull, it has NULL or other empty value (i.e. 0).
		/// The method usefull when Validation signature need to have type but C# compiler cannot calculate it automatically.
		/// So several method request example parameters which use only as C# type hints (without any using).
		/// </summary>
		public static TSource Example<TSource>(this ValidationRuleFactory<TSource> factory)
		{
			return default(TSource);
		}
	}
}
