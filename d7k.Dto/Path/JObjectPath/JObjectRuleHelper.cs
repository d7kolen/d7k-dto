using Newtonsoft.Json.Linq;
using System;

namespace d7k.Dto
{
	public static class JObjectRuleHelper
	{
		public static PathValidation<JObject, TProperty> JObjectRuleFor<TProperty>(this ValidationRuleFactory<JObject> factory,
			JObjectFieldsBuilder<TProperty> fields)
		{
			var path = new JObjectPath(fields);
			return factory.RuleForIf<TProperty>(path, null);
		}

		public static PathValidation<JObject, TProperty> JObjectRuleForIf<TProperty>(this ValidationRuleFactory<JObject> factory,
			JObjectFieldsBuilder<TProperty> fields, Func<JObject, bool> condition)
		{
			var path = new JObjectPath(fields);
			return factory.RuleForIf<TProperty>(path, condition);
		}

		public static PathValidation<JObject, TProperty> JObjectRuleFor<TProperty>(this ValidationRuleFactory<JObject> factory,
			Func<JObjectFieldsBuilder, JObjectFieldsBuilder<TProperty>> fields)
		{
			var path = new JObjectPath(fields(new JObjectFieldsBuilder()));
			return factory.RuleForIf<TProperty>(path, null);
		}

		public static PathValidation<JObject, TProperty> JObjectRuleForIf<TProperty>(this ValidationRuleFactory<JObject> factory,
			Func<JObjectFieldsBuilder, JObjectFieldsBuilder<TProperty>> fields,
			Func<JObject, bool> condition)
		{
			var path = new JObjectPath(fields(new JObjectFieldsBuilder()));
			return factory.RuleForIf<TProperty>(path, condition);
		}
	}
}
