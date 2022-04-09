# Копирование объектов

## Cкопируем один объект в другой

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex();

	var cat = new Cat() { Age = 5 };
	var dog = new Dog().CopyFrom(cat, dto);
	// dog.Age == 5;
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

Одна из фич **DtoComplex** - копирование общих частей объектов с помощью функции CopyFrom. Общими частями явлются общие интерфейсы и общие базовые классы. Копирование производится только для get-set свойств.

Я не понимаю, почему Microsoft до сих пор не предложил свой вариант копирования полей интерфейса (сериализация не в счет - это явный overkill). В Build Time все это было бы естественнее и прозрачнее.

## Что если общих интрефейсов нет

Очень часто бывает так, что один из классов находится в чужой библиотеке. Но даже если и в вашей. IAge это явно утилитарный внутренний аспект поведения: хочется его скрыть, а не делиться с клиентами вашего кода:

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex();
	
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
```
  
Сложных решений много. Но мы здесь говорим про **d7k.Dto**
 
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

Описаная схема работы **ByNestedClassesWithAttributes** может приводить к одной неприятной проблеме: что если на момент сканирования нужная сборка еще не загрузилась в домен? В этом случае ее **DtoContainer-ы** не будут учтены при формирвании **DtoComplex**.

Для нейтрализации этого в **DtoContainer** есть параметр **knownTypes**. Здесь вы сможете указать любой тип из этой сборки. При сканировании функция **ByNestedClassesWithAttributes** найдет сначала ваш DtoContainer. Упомянание типа в атрибуте приведет к загрузки всей его сборки и к дальнейшем использовании ее контейнеров при настройке **DtoComplex**. Например, если в тестах вы хотите воспользоваться инструментами **d7k.Dto**, то проще всего в тестовой сборке создать такой класс, наличие которого приведет к успешной загрузки нужных сборок и корректой инициализации **DtoContext**

```csharp
// Класс распологается в вашей Tests сборке

[DtoContainer(typeof(BusinessDto))]
public static class LoadAllDto { }
```

Dto класс, помеченный аттрибутом DtoContainer, должен быть статическим. Об этом написано в комментарии **DtoContainer**. В приведенном примере для интерфейсов описанных в Dto классе ограничений на публичность нет, как и для Cat_Dto и Dog_Dto классов.

В то же время, во избежании ошибок, не рекомендуется использовать не публичные интерфейсы. Дело в том, что библиотека создает, по мере необходимости на основе интерфейсов, классы в Runtime режиме в динамической сборке через наследование этих интерфейсов. От непубличного интерфейса нельзя будет отнаследовать класс в другой сборке и при попытке создания класса произойдет Exception. **Возможно в будущем** эта проблема будет устранена и непубличные интерфейсы будет возможно использовать в полном объеме.

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

В этом случае мы не сможем выдилить общий интерфейс. Но есть решение от **d7k.Dto**:

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

Так же ни кто не запрещает создавать интерфейс для каждого из этих классов и определять конверторы для интерфейсов:


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

	var cat = new Cat() { Age = 3.5 };
	var dog = new Dog().CopyFrom(cat, dto);
	//dog.Age == 3 (int)

	var catOther = new Cat().CopyFrom(dog, dto);
	//catOther.Age == 3.0 (double)
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

Вы скажите, что в анотации к статья говорилось об отсутствии магии, но здесь без нее явно не обошлось. Это да, но нет...

Для случая, когда у классов с генерик интерфейсами отличаются типы полей, **DtoComplex** пытается привести Source значение к Destination через стандартную операцию преобразования типа:

```csharp
int a = (int)3.5;
```

Именно это и имелось в виду под словом "Незначительно" в начале этого раздела - нестыковки, которые решаются приведением типов. Если преобразование невозможно, то во время копирования поднимется InvalidCastException. Это же произойдет при попытки скопировать (int?)null в int свойство.

## Если вы хотите описать общие интерфейсы через Generic-и с "серьезными" различиями

Что если мы все же хотим обрабатывать Null или же у классов такие поля, которые не могут быть трансформированы через стандартное приведение типов:

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

	var cat = new Cat() { Age = null };
	var dog = new Dog().CopyFrom(cat, dto);
	//dog.Age == 10 (смотри логику в Cast методе)
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

Предположим мы определили интерфейс только для валидации структур данных или для приведения объекта к типу (полиморфизм). Копирование для него нам не нужно:

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
