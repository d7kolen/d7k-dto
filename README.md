# Dto

Библиотека декларативного типизированного управления структурами данных

## Почему я ее использую:
1. Декларативные определения правил копирования, валидации и приведения типов
2. Типизированность. Ни какой магии - если два класса реализуют один интерфейс, то с ними можно делать все что допустимо для этого интерфейса (например, копировать поля одного в другой). Ни каких "возможно ты имел ввиду ...".
3. Классы структур данных (DTO) не обязаны напрямую реализовывать какие-либо интерфейсы. Прощай делема: "это внутренняя реализация - ей место в private зоне" vs "в public зоне реализовать проще".
4. Вы больше не обязаны впихивать, с недовольной гримасой на лице, интерфейсы внешних сервисов в свои классы, успакаивая себя низкой ценой данного решения.
5. Защита от ошибок именования полей в разных классах, в случае когда они значат одно и тоже. Так имя пользователя, всегда остается именем пользователя в независимости от того встречается оно в методе создания пользователя или форме клиентского заказа. Единожды выявленная абстракция будет побуждать разработчиков реиспользовать ее снова и снова, просто потому что так удобнее.
6. Хотите переименовывать поля сразу в нескольких класах одновременно? Разработчик должен оставаться спокоен даже тогда, когда "клиент" вдруг превращается в "пользователя", а "прибыль" в "сумму".
7. А как вам легкое реиспользование сложных правил валидации для ваших сущьностей? Единожды определив правила, вы сможете переиспользовать их для новых классов, приложив минимум услилий.
8. А как вам гибкое комплексное копирование?

Если вы все еще здесь, то давайте рассмотрим базовые примеры использования библиотеки.
Постараюсь не углубляться в детали - все же это **Quickstart start guid**. Но если у кого возникнут вопросы, постараюсь расширить этот документ новыми разделами, комментариями и гиперссылками.

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
  
Вопрос лишь в том, что такое функция CopyFrom. Вы могли бы определить ее в классе Dog или как метод расширения. **Dto** предлагает следующее решение:

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

Может возникнуть вопрос: почему DtoComplexHelper отсутствует в библиотеки **Dto**? Ответ: этот класс отвратителен - это статический контекст; у него есть методы с очень абстарктными именами, которые постаянно находится на виду и навязывают использовать именно их. Это неплохо пока ваша парадигма полностью (на 100%) соответствует той, которая предлагает **DtoComplexHelper**. Но если вдруг, например, вам захочется иметь два (или более) подобных контекстов, то эта реализация тут же начнет очень дурно пахнуть. Наиболее правильным кажется выделить этот класс в отдельный NuGet пакет, но на текущий момент это выглядит черезвычайно тяжеловестным и преждевременным.

## Что если общих интрефейсов нет

Так может получиться, если один из классов находится в чужой библиотеке. Но даже если в вашей - IName это явно утилитарный внутренний аспект поведения: чаще, по истетическим соображениям, хочется его скрыть чем делиться друими:

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
  
Сложных решений масса. Но мы же собрались поговорить про **Dto**
 
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

И скорректируем инициализацию **DtoContex**

```csharp
public static class DtoComplexHelper
{
  static DtoComplex m_dto = new DtoComplex().ByNestedClassesWithAttributes();
  ...
}
```

Функция **ByNestedClassesWithAttributes** сканирует все загруженные в AppDomain сборки и ищет в них DtoContainer классы. С их помощью она конфигурирует DtoComplex таким образом, что для него **Cat** и **Dog** вдруг начинает реализовывать интерфейс **IName**.

Описаная схема работы **ByNestedClassesWithAttributes** может приводит к одной неприятной проблеме: если на момент сканирования какая-то сборка не загрузилась, то ее DtoContainer-ы не будут учтены при формирвании DtoComplex. Для нейтрализации этого в **DtoContainer** есть параметр **knownTypes**. Здесь вы можете указать любой тип из этой сборки. При сканировании **ByNestedClassesWithAttributes** найдет сначала ваш DtoContainer, а упомянание проблемного типа приведет к загрузке сборки и дальнейшей загрузки ее контейнеров в **DtoContext**. Например, если в тестах вы хотите воспользоваться инструментами **Dto** до вызова первого метода сборки с логикой, то проще всего в тестовой сборке создать такой класс, наличие которого приведет к корректой инициализации **DtoContext**

```csharp
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

В этом случае мы не сможемим выделить общий интерфейс. **Dto** предлагает такое решение:

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

## Копирование части объекта
Только root поля

## Валидация

Все данные, приходящие от клиентов, необходимо проверять. **Dto** предоставляет такую возможность

```csharp
static void Main(string[] args)
{
	var tCat = new Cat() { Name = " Snow " };
	var validator = new ValidationRepository();
	tCat = validator.FixValue(tCat, nameof(tCat), x => x.NotEmpty().ValidateDto());
}

class Cat
{
	public string Name { get; set; }
}

[DtoContainer]
public static class Dto
{
	[DtoValidate]
	static void Validate(ValidationRuleFactory<Cat> t)
	{
		t.RuleFor(x => x.Name).Trim().NotEmpty();
	}
}
```

**ValidateDto** это еще один метод расширения в нашем DtoComplexHelper:

```csharp
public static class DtoComplexHelper
{
...
	public static PathValidation<TSrc, TProperty> ValidateDto<TSrc, TProperty>(this PathValidation<TSrc, TProperty> validation)
	{
		return validation.ValidateDto(m_dto);
	}
}
```

Механизм валидации может не только проверить корректность присланных данных, но и скорректировать какие-то простые случае (в нашем примере - убрать пустые символы в начале и в конце строки).

В качестве валидируемой сущности может выступать интерфейс. Поэтому, классы могут быть разделены на интерфейсы и валидация может производиться для каждого интерфейса в отдельности. Другие классы для которых определен этот интерфейс автоматически будут иметь эти валидаторы

## Валидация вложенных структур
## Валидация массивов и списков
## Вложенная комплексная валидация
## Дополнительные функции валидации
## Кастомная валидация и ValidationRule
## DtoAdapter
## DtoObject
