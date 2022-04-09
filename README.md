# d7k.Dto

Библиотека декларативного типизированного управления структурами данных.

Ее рабочая версия может быть найдена в Nuget: https://www.nuget.org/packages/d7k.Dto

## Почему я ее использую:
1. Она предоставляет возможность определять декларативные правила копирования, валидации и приведения типов
2. Типизированность. Ни какой магии - если два класса реализуют один интерфейс, то с ними можно делать все что допустимо для этого интерфейса (например, копировать поля одного в другой). Ни каких "возможно ты имел ввиду ...".
3. Классы структур данных (DTO) не обязаны напрямую реализовывать какие-либо интерфейсы. Прощай дилемма: "это внутренняя реализация - ей место в private зоне" vs "в public зоне реализовать проще".
4. Вы больше не обязаны впихивать, с недовольной гримасой на лице, интерфейсы внешних сервисов в свои классы, успакаивая себя низкой ценой данного решения.
5. Защита от ошибок именования полей в разных классах, в случае когда они означют одно и тоже. Так, имя пользователя всегда остается именем пользователя в независимости от того встречается оно в методе создания пользователя или форме клиентского заказа. Единожды выявленная абстракция будет побуждать разработчиков реиспользовать ее снова и снова. Просто потому что так удобнее.
6. Хотите переименовывать поля сразу в нескольких класах одновременно? Разработчик должен оставаться спокоен даже тогда, когда "клиент" вдруг превращается в "пользователя", а "прибыль" в "сумму".
7. А как вам легкое реиспользование сложных правил валидации для ваших сущностей? Единожды определив правила, вы сможете переиспользовать их для новых классов, приложив минимум услилий.
8. А как вам гибкое комплексное копирование одного объекта в другой?

Если вы все еще здесь, то давайте рассмотрим базовые примеры использования библиотеки.
Постараюсь не углубляться в детали - все же это **Quickstart Start Guid**. Но если у вас возникнут вопросы, постараюсь расширить этот документ новыми разделами, комментариями и гиперссылками.

## [Копирование объектов](Doc/Copy.md)

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();
	
	var cat = new Cat() { Age = 5 };
	var dog = new Dog().CopyFrom(cat, dto);
	//dog.Age == 5
}

class Cat
{
	public int Age { get; set; }
}

class Dog
{
	public int Age { get; set; }
}

[DtoContainer]
static class Dto
{
	interface IAge
	{
		int Age { get; set; }
	}

	class Cat_Dto : Cat, IAge { }
	class Dog_Dto : Dog, IAge { }
}
```

[Интересно?](Doc/Copy.md)

## [Копирование части объекта](Doc/Update.md)

```csharp
class Program
{
	static void Main(string[] args)
	{
		var dto = new DtoComplex().ByNestedClassesWithAttributes();

		var cat = new Cat
		{
			Age = 1,
			Weight = 2
		};

		var result = dto.Update(new Dog(), cat, new[] { nameof(Dog.Age) });
		//result.Age == 1;
		//result.Weight == 0;
	}
}

class Cat
{
	public int Age { get; set; }
	public int Weight { get; set; }
}

class Dog
{
	public int Age { get; set; }
	public int Weight { get; set; }
}

public interface IPet
{
	int Age { get; set; }
	int Weight { get; set; }
}

[DtoContainer]
static class Dto
{
	class Cat_Dto : Cat, IPet{ }
	class Dog_Dto : Dog, IPet{ }
}
```
[Интересно?](Doc/Update.md)

## [Валидация](Doc/Validate.md)

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat() { Name = "  Snow  " };
	
	dto.FixValue(cat, nameof(cat), x => x.ValidateDto());
	//cat.Name == "Snow"
}

class Cat
{
	public string Name { get; set; }
}

[DtoContainer]
public static class Dto
{
	public interface IName
	{
		string Name { get; set; }
	}

	[DtoValidate]
	static void Validate(ValidationRuleFactory<IName> t)
	{
		t.RuleFor(x => x.Name).Trim();
	}

	class Cat_Dto : Cat, IName { }
}
```

[Интересно?](Doc/Validate.md)

## [Функции валидации](Doc/ValidateFunctions.md)

## [Приведение типа (AS)](Doc/As.md)

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

[Интересно?](Doc/As.md)

## [Философия](Doc/Philosophy.md)

Здесь немного занудства. Правда, не стоит
