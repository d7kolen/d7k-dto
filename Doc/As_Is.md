# AS и IS

## Привидение типа объекта (AS)

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat
	{
	  Name = "Snow"
	};

	var name = dto.As<IName>(cat);
	//name.Name == "Snow";

	name.Name = "Coal";
	//name.Name == "Coal";
}
    
class Cat
{
	public string Name { get; set; }
}

interface IName
{
	public string Name { get; set; }
}

[DtoContainer]
public static class Dto
{
	class Cat_Dto : Cat, IName { }
}
```

В примере выше мы создали класс Cat и ассоциировали его с интерфейсом IName.
