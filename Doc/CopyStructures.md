# Копирование объектов

## Cкопируем один объект в другой

```csharp
static void Main(string[] args)
{
	var tCat = new Cat() { Name = "Snow" };
	var tDog = new Dog().CopyFrom(tCat);
}

public interface IName
{
	string Name { get; set; }
}

class Cat: IName
{
	public string Name { get; set; }
}

class Dog: IName
{
	public string Name { get; set; }
}
```
  
Вопрос лишь в том, что такое функция CopyFrom. Вы могли бы определить ее в классе Dog или как метод расширения. **d7k.Dto** предлагает следующее решение:

```csharp
public static class DtoComplexHelper
{
	static DtoComplex m_dto = new DtoComplex();

	public static TDst CopyFrom<TDst, TSrc>(this TDst dst, TSrc src)
	{
		return m_dto.Copy(dst, src);
	}
}
```
  
Одна из фич **DtoComplex** класса - копирование общих частей классов (общие интерфейсы и общие базовые классы). Я не понимаю, почему Microsoft до сих пор не предложил свой вариант копирования интерфейсов (сериализация не в счет - это явный overkill). В Build Time все это было бы естественнее и прозрачнее.

Может возникнуть вопрос: почему DtoComplexHelper отсутствует в библиотеке **d7k.Dto**? Ответ: этот класс отвратителен - это статический контекст; у него есть методы с очень абстрактными именами, которые постаянно находится на виду и навязывают использовать именно их. Это неплохо пока парадигма вашего кода полностью (на 100%) соответствует той, которая предлагает **DtoComplexHelper**. Но если вдруг, например, вам захочется иметь два (или более) подобных контекстов, то эта реализация тут же начнет очень дурно пахнуть. Наиболее правильным кажется выделить этот класс в отдельный NuGet пакет, но на текущий момент это выглядит черезвычайно тяжеловестным и преждевременным.

## Что если общих интрефейсов нет

Так может получиться, если один из классов находится в чужой библиотеке. Но даже если в вашей. IName это явно утилитарный внутренний аспект поведения: хочется его скрыть, а не делиться с клиентами вашего кода:

```csharp
static void Main(string[] args)
{
	var tCat = new Cat() { Name = "Snow" };
	var tDog = new Dog().CopyFrom(tCat);
}

class Cat
{
	public string Name { get; set; }
}

class Dog
{
	public string Name { get; set; }
}
```
  
Сложных решений масса. Но мы же собрались поговорить про **d7k.Dto**
 
```csharp
[DtoContainer]
public static class Dto
{
	public interface IName
	{
		string Name { get; set; }
	}

	class Cat_Dto : Cat, IName { }
	class Dog_Dto : Dog, IName { }
}
```

Скорректируем инициализацию **DtoContext**

```csharp
public static class DtoComplexHelper
{
  static DtoComplex m_dto = new DtoComplex().ByNestedClassesWithAttributes();
  ...
}
```

Функция **ByNestedClassesWithAttributes** сканирует все загруженные в AppDomain сборки и ищет в них DtoContainer классы. С их помощью она конфигурирует DtoComplex таким образом, что для него **Cat** и **Dog** вдруг начинает реализовывать интерфейс **IName**.

Описаная схема работы **ByNestedClassesWithAttributes** может приводит к одной неприятной проблеме: если на момент сканирования какая-то сборка не загрузилась, то ее DtoContainer-ы не будут учтены при формирвании DtoComplex. Для нейтрализации этого в **DtoContainer** есть параметр **knownTypes**. Здесь вы можете указать любой тип из этой сборки. При сканировании **ByNestedClassesWithAttributes** найдет сначала ваш DtoContainer, а упомянание проблемного типа приведет к загрузке сборки и дальнейшей загрузки ее контейнеров в **DtoContext**. Например, если в тестах вы хотите воспользоваться инструментами **d7k.Dto** до вызова первого метода сборки с логикой, то проще всего в тестовой сборке создать такой класс, наличие которого приведет к корректой инициализации **DtoContext**

```csharp
// Класс распологается в вашей Tests сборке

[DtoContainer(typeof(BusinessDto))]
public static class LoadAllDto { }
```

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
public static class Dto
{
	[DtoConvert]
	static void Convert(Dog dst, Cat src)
	{
		dst.Age = src.Age;
	}
}
```

Ни кто вам не запрещает создавать интерфейс для каждого класса и определять конверторы для интерфейсов. Сигнатуру метода **Convert** вы можете найти в комментариях к **DtoConvertAttribute**.
