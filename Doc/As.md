# Приведение типа объекта (AS)

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
	//cat.Name == "Coal";
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

Под копотом для объекта создается адаптер, который реализует IName интерфейс. Именно этот адаптер фактически и возвращается методом **As**. Адаптер создается в динамической сборке. Это приводит к необходимости размещать интерфейс IName в публичной области видимости. Это техническое ограничение текущей реализации.

В случае, если объект не реализует нужный интерфейс, то метод **As** вернет NULL. Такое поведение можно использовать для проверки, что объект реализует заданный интерфейс (аналогично IS операции).

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
	//cat.Name == "Coal";
}

...
```

Чаще всего мы знаем, что заданный объект реализует нужный интерфейс. И если наши ожидания ошибочны, то мы хотим падения нашего кода с исключение **NotImplementedException**. "Жесткий" вариант реализуется с помощью **AsStrongly**.

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

После приведения типа мы имеем дело не с исходным объектом, а с адаптером к нему. Этот адаптер и реализует нужный нам интерфейс. В большинстве случаев это не является проблемой. Если привести тип адаптера (с помощью операции **As**) к другому интерфейсу, то мы его получим так же как после приведения исходного объекта. Под копотом будет найден исходный объект и приведен к нужному интерфейсу именно он.

У вас все еще остается возможность получить исходный объект самостоятельно. Для этого существует метод **GetDtoAdapterSource**. Он достаточно технический и редкоиспользуемый в реальной жизни, поэтому и имеет такое странное техническое имя. Но ничего противозаконного в его использовании нет.
