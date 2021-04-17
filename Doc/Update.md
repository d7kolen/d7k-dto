# Копирование части объекта

## Простое копирование

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex();

	var cat = new MaineCoonCat
	{
		Age = 1,
		Weight = 2,
		Height = 3
	};

	var result = dto.Update(new SiameseCat(), cat, new[] { nameof(SiameseCat.Age), nameof(SiameseCat.Height) });
	//result.Age == 1;
	//result.Weight == 0;
	//result.Height == 0;
}

class Cat
{
	public int Age { get; set; }
	public int Weight { get; set; }
}

class MaineCoonCat : Cat
{
	public int Height { get; set; }
}

class SiameseCat : Cat
{
	public int Height { get; set; }
}
```

Метод **Update** работает аналогично **Copy** - он копирует родственные поля из одного объекта в другой. Но до результирующего объекта добираются только изменения от выбранных свойств.

Запутанно? Ну..., шаг за шагом это определение становиться все более и более понятным.

В данном примере мы хотим обновить свойства Age и Height:
* Свойство Age изменится в результирующем объекте. Это свойство находится в общем классе для исходного и результирующего объекта. Поэтому, свойство Age результирующего объекта является родственным для Age исходного.
* Свойство Weight не копируется. Его нет в списке на обновление.
* Свойство Height не копируется. Оно не является родственным для двух объектов, поэтому библиотека не знает откуда для него брать значение.

## Если нет общих интерфейсов

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

Вы все также можете определять общие интерфейсы в **DtoContainer**. И **Update** будет принимать эти определения во внимание при реализации копирования выбранных полей.

## Конверторы и обновление

Что, если копирование интерфейсов определоно через функции конверторы? Проблема в том, что надо скопировать только часть свойств, которыми оперирует конвертор. Нет ни каких проблем:

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat
	{
		Age = 1,
		Weight = 2
	};

	var result = dto.Update(new Dog(), cat, new[] { nameof(Dog.Age) });
	//result.Age == "1";
	//result.Weight == null;
}

class Cat
{
	public int Age { get; set; }
	public int Weight { get; set; }
}

class Dog
{
	public string Age { get; set; }
	public string Weight { get; set; }
}

[DtoContainer]
static class Dto
{
	[DtoConvert]
	static void Convert(Dog dst, Cat src)
	{
		dst.Age = src.Age.ToString();
		dst.Weight = src.Weight.ToString();
	}
}
```

Помните странное замечание в определении в начале этого раздела?

*...до результирующего объекта добираются только изменения от выбранных свойств*

Это оно самое и есть. Хотя из исходного объекта свойства и будут считаны. Хотя по ним будет расчитано значение, которое должно вроде как попасть в результирующий объект. Но оно не попадет, потомоу что оно не "выбранное".

Дело в том, что в конвертере будет находится не оригинальный объект, а объект посредник. После того как конвертирование закончится и поля объекта посредника будут проинициализированы система скопирует из него только "выбранные" поля. Остальные поля (такие как Weight) будут проигнорированы.

## Конвертер определенный для интерфейсов?

По интерфейсам же нельзя создать прокси объект. А результирующий класс может быть связан с интерфейсом в конверторе только через **DtoContainer**. Опять же, вам не о чем беспокоиться:

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat
	{
		Age = 1,
		Weight = 2
	};

	var result = dto.Update(new Dog(), cat, new[] { nameof(Dog.Age) });
	//result.Age == "1";
	//result.Weight == null;
}

class Cat
{
	public int Age { get; set; }
	public int Weight { get; set; }
}

class Dog
{
	public string Age { get; set; }
	public string Weight { get; set; }
}

[DtoContainer]
public static class Dto
{
	public interface ICat
	{
		int Age { get; set; }
		int Weight { get; set; }
	}

	public interface IDog
	{
		string Age { get; set; }
		string Weight { get; set; }
	}

	[DtoConvert]
	static void Convert(IDog dst, ICat src)
	{
		dst.Age = src.Age.ToString();
		dst.Weight = src.Weight.ToString();
	}

	class Cat_Dto : Cat, ICat { }
	class Dog_Dto : Dog, IDog { }
}
```

Библиотека все же умеет создавать прокси объекты по интерфейсам. Поэтому это до сих пор работает.

Главное не забывайте:
1. Интерфесы должны состоять исключительно из свойств. Библиотека не умеет создавать заглушки методов - ее назначение копирование и валидация.
2. Как уже не раз говорилось, интерфейсы должны быть видимыми из вне. Для реализации описанной функциональности создается динамическая сборка, в которой строятся структуры и методы. Они реализуются на основе тех интерфейсов, которые вы определяете в своем коде. Если ваши интерфейсы не будут видимыми из вне, значит не получится построить и инфраструктуру для них.

