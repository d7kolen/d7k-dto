# Копирование части объекта

## Простое копирование

```csharp
static void Main(string[] args)
{
	var cat = new Cat
	{
		Age = 1,
		Weight = 2
	};

	var result = dto.Update(new Cat(), cat, new[] { nameof(Cat.Age) });
	//result.Age == 1;
	//result.Weight == 0;
}

class Cat
{
	public int Age { get; set; }
	public int Weight { get; set; }
}
```

