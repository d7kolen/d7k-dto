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

В случае, если объект не реализует нужный интерфейс, то метод **As** вернет NULL. Это можно использовать для проверки, что объект реализует заданный интерфейс (IS операция).

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

После приведения типа мы начинаем иметь дело не с исходным объектом, а с адаптером к нему. Этот адаптер и реализует нужный нам интерфейс. В большинстве случаев это не является проблемой, т.к. если привести тип адаптера (с помощью операции **As**) к другому интерфейсу, то вы получите и его тоже.

Но у вас остается возможность получить и исходный объект. Этого можно добиться с помощью **GetDtoAdapterSource** операции. Она достаточно техническая и редкоиспользуемая в реальной жизни, поэтому и имеет немного странное техническое имя.
