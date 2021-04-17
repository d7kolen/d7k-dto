using System;
using System.Linq.Expressions;

namespace d7k.Dto
{
	public static class ObjectFieldsRuleHelper
	{	
		public static PathValidation<TSource, TProperty> RuleFor<TSource, TProperty>(this ValidationRuleFactory<TSource> factory, 
			Expression<Func<TSource, TProperty>> expression)
		{
			var path = PathValueIndexer<TSource>.m_factory.GetPathItems(expression);
			return factory.RuleForIf<TProperty>(path, null);
		}

		public static PathValidation<TSource, TProperty> RuleForIf<TSource, TProperty>(this ValidationRuleFactory<TSource> factory, 
			Expression<Func<TSource, TProperty>> expression, Func<TSource, bool> condition)
		{
			var path = PathValueIndexer<TSource>.m_factory.GetPathItems(expression);
			return factory.RuleForIf<TProperty>(path, condition);
		}
	}
}
