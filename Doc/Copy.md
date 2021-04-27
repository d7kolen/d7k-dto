# Копирование объектов

## Cкопируем один объект в другой

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex();

	var tCat = new Cat() { Age = 5 };
	var tDog = new Dog().CopyFrom(tCat, dto);
	// tDog.Age == 5;
}

public interface IAge
{
	int Age { get; set; }
}

class Cat: IAge
{
	public int Age { get; set; }
}

class Dog: IAge
{
	public int Age { get; set; }
}
```

Одна из фич **DtoComplex** класса - копирование общих частей объектов с помощью функции CopyFrom. Общими частями явлются общие интерфейсы и общие базовые классы. Копирование производится только для get-set свойств.

Я не понимаю, почему Microsoft до сих пор не предложил свой вариант копирования интерфейсов (сериализация не в счет - это явный overkill). В Build Time все это было бы естественнее и прозрачнее.

## Что если общих интрефейсов нет

Так может получиться, если один из классов находится в чужой библиотеке. Но даже если в вашей. IAge это явно утилитарный внутренний аспект поведения: хочется его скрыть, а не делиться с клиентами вашего кода:

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex();
	
	var tCat = new Cat() { Age = 5 };
	var tDog = new Dog().CopyFrom(tCat, dto);
	//tDog.Age == 5
}

class Cat
{
	public int Age { get; set; }
}

class Dog
{
	public int Age { get; set; }
}
```
  
Сложных решений масса. Но мы же собрались поговорить про **d7k.Dto**
 
```csharp
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

Скорректируем инициализацию **DtoComplex**

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();
	...
}
```

Функция **ByNestedClassesWithAttributes** сканирует все загруженные в AppDomain сборки и ищет в них **DtoContainer** классы. С их помощью она конфигурирует **DtoComplex** таким образом, что для него **Cat** и **Dog** вдруг начинает реализовывать интерфейс **IAge**.

Описаная схема работы **ByNestedClassesWithAttributes** может приводить к одной неприятной проблеме: если на момент сканирования какая-то сборка не загрузилась в домен, то ее **DtoContainer-ы** не будут учтены при формирвании **DtoComplex**. Для нейтрализации этого в **DtoContainer** есть параметр **knownTypes**. Здесь вы можете указать любой тип из этой сборки. При сканировании **ByNestedClassesWithAttributes** найдет сначала ваш DtoContainer. Упомянание типа в атрибуте приведет к загрузке всей его проблемной сборки и дальнейшей загрузки ее контейнеров в **DtoComplex**. Например, если в тестах вы хотите воспользоваться инструментами **d7k.Dto** до вызова первого метода сборки с логикой, то проще всего в тестовой сборке создать такой класс, наличие которого приведет к корректой инициализации **DtoContext**

```csharp
// Класс распологается в вашей Tests сборке

[DtoContainer(typeof(BusinessDto))]
public static class LoadAllDto { }
```

Dto класс, помеченный аттрибутом DtoContainer, должен быть статическим. Об этом написано в комментарии **DtoContainer**.
В приведенном примере для интерфейсов описанных в Dto классе ограничений на публичность нет, как и для Cat_Dto и Dog_Dto классов.
В то же время не публичные интерфейсы использовать не рекомендуется. Дело в том, что библиотека создает по мере необходимости на основе интерфейсов классы в динамической сборке через их наследование. От непубличного интерфейса нельзя будет отнаследоваться. Возможно в будущем эта проблема будет устранена.

## Что если сигнатуры Dto различаются

Допустим Dto классы не совсем эквавалентны: различаются имена полей или различаются их типы.

```csharp
class Cat
{
	public int Age { get; set; }
}

class Dog
{
	public int? Age { get; set; }
}
```

В этом случае мы не сможем выдилить общий интерфейс. Решение от **d7k.Dto**:

```csharp
[DtoContainer]
static class Dto
{
	[DtoConvert]
	static void Convert(Dog dst, Cat src)
	{
		dst.Age = src.Age;
	}
}
```

Ни кто вам не запрещает создавать интерфейс для каждого класса и определять конверторы для интерфейсов:


```csharp
[DtoContainer]
static class Dto
{
	interface IAge_Int
	{
		int Age { get; set; }
	}
	
	interface IAge_String
	{
		string Age { get; set; }
	}

	[DtoConvert]
	static void Convert(IId_String dst, IId_Int src)
	{
		dst.Age = src.Age == null ? null : src.Age.ToString();
	}
}
```

Все возможные сигнатуру метода **Convert** вы можете найти в комментариях к **DtoConvertAttribute**.

## Если вы хотите описать общие интерфейсы через Generic-и

Допустим Dto классы почти эквивалентны. "Незначительно" различаются их типы:

```csharp
class Cat
{
	public double Age { get; set; }
}

class Dog
{
	public int Age { get; set; }
}

static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var tCat = new Cat() { Age = 3.5 };
	var tDog = new Dog().CopyFrom(tCat, dto);
	//tDog.Age == 3 (int)

	var tCatOther = new Cat().CopyFrom(tDog, dto);
	//tCatOther.Age == 3.0 (double)
}

[DtoContainer]
static class Dto
{
	interface IAge<T>
	{
		T Age { get; set; }
	}

	class Cat_Dto : Cat, IAge<double> { }
	class Dog_Dto : Dog, IAge<int> { }
}
```

В этом примере у класса Cat поле Age имеет тип **double**, а у Dog тоже поле имеет тип **int**. В примере показывается что возможно копирование и в одну и в другую сторону (но с очевидной потерей точности).

Вы скажите, что в анотации к статья говорилось об отсутствии магии, но здесь без нее явно не обошлось. Да, но нет...

Дело в том, что в случае, когда типы полей разные у классов для случая с генериками, **DtoComplex** пытается привести Source значение к Destination через стандартную операцию преобразования типа: 

```csharp
int a = (int)3.5;
```

Именно это и означает слово "Незначительно". Если преобразование невозможно, то во время копирования поднимется InvalidCastException. Это же произойдет при попытки скопировать (int?)null в int свойство.

## Если вы хотите описать общие интерфейсы через Generic-и с серьезными различиями

Что если мы все же хотим обрабатывать Null или же у классов такие поля, которые не могут быть трансформированы через стандартное преобразование:

```csharp
class Cat
{
	public int? Age { get; set; }
}

class Dog
{
	public int Age { get; set; }
}

static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var tCat = new Cat() { Age = null };
	var tDog = new Dog().CopyFrom(tCat, dto);
	//tDog.Age == 10
}

[DtoContainer]
static class Dto
{
	interface IAge<T>
	{
		T Age { get; set; }
	}

	[DtoCast]
	static int Cast(int? src)
	{
		return src ?? 10;
	}

	class Cat_Dto : Cat, IAge<int?> { }
	class Dog_Dto : Dog, IAge<int> { }
}
```

**DtoCast** метод определяет то, как **DtoComplex** должен выполнять трансформацию типов для generic свойств. В случае если ничего подходящего не найдено, то делается попытка использовать трансформацию по умолчанию.

## Если какой-то интерфейс не должен участвовать в копировании

Предположим мы определили интерфейс только для валидации структур данных или для приведения объекта к типу (для полиморфизма). Копирование для него нам не нужно:

```csharp
class Cat
{
	public int Age { get; set; }
}

class Dog
{
	public int Age { get; set; }
}

static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var tCat = new Cat() { Age = 5 };
	var tDog = new Dog().CopyFrom(tCat, dto);
	//tDog.Age == 0
}

[DtoContainer]
static class Dto
{
	[DtoNonCopy]
	interface IAge
	{
		int Age { get; set; }
	}

	class Cat_Dto : Cat, IAge { }
	class Dog_Dto : Dog, IAge { }
}
```

Вы можете промаркеровать такой интерфейс **DtoNonCopy** аттрибутом. В этом случае **DtoComplex** будет игнорировать такой интерфейс - он не будет участвовать в копировании.

О приведении типа и о валидации будет рассказывается в других разделах.
