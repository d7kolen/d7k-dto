using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto
{
	public abstract class PathValidation<TSource>
	{
		public PathValueIndexer<TSource> Indexer { get; private set; }

		public List<BaseValidationRule> Validators { get; set; } = new List<BaseValidationRule>();

		public ValidationRuleFactory<TSource> Factory { get; private set; }

		public string Path { get; set; }

		public PathValidation(PathValueIndexer<TSource> indexer, ValidationRuleFactory<TSource> factory)
		{
			Indexer = indexer;
			Path = indexer?.GeneralPath;
			Factory = factory;
		}

		public void AddValidator(BaseValidationRule validator)
		{
			Validators.Add(validator);
		}

		public abstract PathValidation<TSource> Clone();

		public override string ToString()
		{
			if (Path == null)
				return $"{typeof(TSource).Name} ~{Validators.Count}";
			return $"{typeof(TSource).Name}{Path} ~{Validators.Count}";
		}
	}

	public class PathValidation<TSource, TProperty> : PathValidation<TSource>
	{
		public PathValidation(PathValueIndexer<TSource> indexer, ValidationRuleFactory<TSource> factory)
			: base(indexer, factory)
		{
		}

		public override PathValidation<TSource> Clone()
		{
			return new PathValidation<TSource, TProperty>(Indexer, Factory)
			{
				Validators = Validators.ToList()
			};
		}
	}
}
