# Привидение типа объекта (AS)

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

public interface IName
{
	public string Name { get; set; }
}

[DtoContainer]
public static class Dto
{
	class Cat_Dto : Cat, IName { }
}
```

В примере выше мы создали класс Cat и ассоциировали его с интерфейсом IName. После "приведения" с помощью операции **As** объектом можно оперировать так, будто он действительно реализует этот интерфейс.

Под копотом для объекта создается адаптер, который реализует IName интерфейс. Именно этот адаптер фактически и возвращается методом **As**. Адаптер создается в динамической сборке. Это приводит к необходимости размещать интерфейс IName в публичной области видимости.

## As, для тех кому пожестче

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat
	{
	  Name = "Snow"
	};

	var name = dto.AsStrongly<IName>(cat);
	//name.Name == "Snow";

	name.Name = "Coal";
	//name.Name == "Coal";
}

...
```

Чаще всего мы знаем, что заданный объект реализует нужный интерфейс. В этом случае мы хотим, если наши ожидания ошибочны, то мы ожидаем падения нашего кода с исключение **NotImplementedException**. Этот жесткий вариант реализуется с помощью **AsStrongly**.

Это аналог явного (explicit) приведения типа объекта.

## Исходный объект

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat
	{
	  Name = "Snow"
	};

	var name = dto.As<IName>(cat);
	var tCat = dto.GetDtoAdapterSource(name);

	//object.ReferenceEquals(cat, tCat) == True;
}
    
class Cat
{
	public string Name { get; set; }
}

public interface IName
{
	public string Name { get; set; }
}

[DtoContainer]
public static class Dto
{
	class Cat_Dto : Cat, IName { }
}
```

Как уже говорилось выше, после приведения, с помощью операции **As** мы начинаем иметь дело не с исходным объектом, а с адаптером
