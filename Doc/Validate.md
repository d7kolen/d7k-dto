# Валидация

Все данные, приходящие от клиентов, необходимо проверять. **d7k.Dto** предоставляет такую возможность

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex();

	var cat = new Cat();
	
	dto.FixValue(cat, nameof(cat), x => x.NotEmpty());
	//cat.Name == "Snow"
}

class Cat
{
	public string Name { get; set; }
}
```

В данном примере метод **FixValue** запускает процесс валидации переданного ему объекта.

C помощью **x.NotEmpty()** вызова проверяется что переданный ему объект не является пустым (для классов - это проверка на NULL). Если бы объект cat оказался бы пустым, то **FixValue** поднял бы исключение ValidateException с текстом: "'cat' cannot be empty.". Поле Issues содержало бы экземпляр этой ошибки.

Если бы в примере было бы **x.NotEmpty().NotEmpty()**, при условии что мы передадим пустое значение на вход, то сообщение "'cat' cannot be empty." в исключении содержалось бы дважды. Issues бы содержал бы два экземпляра этой ошибки.

Дело в том, что **DtoComplex** старается сделать все предложенные проверки и старается сформировать максимально полный набор проблем переданного объекта. Это особенно полезно, когда переданный объект содержит более одной проблемы в различных полях.

## Валидация в зависимости от типа объекта

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
	[DtoValidate]
	static void Validate(ValidationRuleFactory<Cat> t)
	{
		t.RuleFor(x => x.Name).Trim();
	}
}
```

Вызов **x.ValidateDto()** самостоятельно выбирает методы валидации объекта в зависимости от текущих настроек валидации **DtoComplex**. Как и в случае с копирование настройка **DtoComplex** производится через механизм **DtoContainer**.

Методы валидации отдельных типов можно определить через **DtoValidate** атрибут. Вы можете найти все возможные сигнатуры этих методов в комментариях к **DtoValidate** классу.

Все Validate методы в сигнатурах имеют **ValidateRuleFactory** объект. Он предоставляет возможность определять правила для проверки отдельных компонентов проверяемого объекта. Так, в примере определены правила для поля Name. Прежде всего его очищают от невидимых символов в начале и конце строкового значения (если строка после этого пустеет, то значение заменяется на NULL).

Это еще одна особенность валидации с помощью **DtoComplex**. Он не только проверяет значения на валидность, но способен так же корректировать их в соответствии с вашими потребностями.

## Стандартизация пустых значений

Для каждой структуры существует свой ответ, что такое "пустое значение" в том или ином ее поле: null, string.Empty, пустой массив или кастомный NullObject. В одном месте удобен один подход, в другом то же самое решение приводит к странным последствиям. Например к таким проверкам: **if (obj.A is MyNullObject) ...**.

Решение и стандартизация данного вопроса на ранних этапах исполнения запросов исключит появление ненужных **if** конструкиций в дальнейшем, сделав код более ориентированным на решение бизнес задач, а не "проблем" языка (в нашем случае C#).

К сожалению, заставить клиента всегда следовать заданным требованиям - задача противоричивая и приводит к конфликтам внутри команды:
- Почему я должен в поле A передавать пустой массив, а в поле B null?
- Ну ... моя ORM будет трактовать это значение ...

**ORM**? Серьезно? Какое дело клиентскому коду до твоей ORM? У него есть 50 аналогичных арументов сделать иначе.

**d7k.Dto** считает, что клиент может следовать любому, удобному для него, подходу. Во время валидации входных значений сервер легко сможет привести все в "правильный" для него вид. Счастливы **клиент** и **ORM** одновременно.

## Валидаторы интерфейсов

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

В данном примере метод валидации смотрит не на класс, а на интерфейс. Это особенно полезно, когда в вашей системе имеется множество классов которые реализуют этот интерфейс (или просто ассоциированы в **DtoContainer-е**). Вы однажды прописали правила валидации и дальше они применяются самостоятельно там, где встречается ваш интерфейс.

Маленькая особенность данного примера в том, что IName должен быть публичным из-за реализации **DtoComplex** (Emit и динамические сборки). Я бы посоветовал делать все такие интерфейсы публичными. Это не должно доставить проблем, так как все подобные интерфейсы собраны внутри класса контейнера. Надо приложить дополнительные усилия, что бы до них добраться - это защита от того, что лишние интерфейсы будут постоянно путаться под ногами. Кроме того, вам это все равно потребуется если вы решите использовать **DtoComplex.As** о котором рассказывается в другом разделе.

## Валидация коллекций элементов

Давайте поговорим про массивы и списки. Словари не в счет - для них пока нет коробочной реализации подобного.

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat() { KittenNames = new[] { "  Snow  ", "  ", " Red " } };

	dto.FixValue(cat, nameof(cat), x => x.NotEmpty().NotEmpty().ValidateDto());
	//cat.Name == ["Snow", "Red"]
}
		
class Cat
{
	public string[] KittenNames { get; set; }
}

[DtoContainer]
public static class Dto
{
	[DtoValidate]
	static void Validate(ValidationRuleFactory<Cat> t)
	{
		t.RuleFor(x => x.KittenNames).SkipEmpty().Trim();
		t.RuleFor(x => x.KittenNames.ScanAll()).Trim();
	}
}
```

И так, в нашем примере в объекте Cat перечислины имена ее котят.

Прежде всего в правиле для **x.KittenNames** происходит поиск пустых значений массива (элементов которые являются Empty для DtoComplex) и избавление от них с помощью метода **SkipEmpty()**. Далее, если в массиве не осталось не одного элемента, то ссылка на него заменяется на NULL с помощью метода **Trim()**.

Далее, определено правило проверки каждого элемента массива отдельно, с помощью конструкции x.KittenNames.**ScanAll()**.

Можно не беспокоиться о том, что массив в какой-то момент приходит пустым. Делегаты в **RuleFor** никогда не исполняются. Их задача выявить правила доступа к части объекта. Метод **ScanAll**, тоже не вызывается - он лишь говорит системе о необходимости просканировать все объекты коллекции. Более того, вызов **ScanAll** всегда поднимает исключение NotImplementedException.

Все валидаторы вызываются в том же порядке, что и определены. Поэтому если во время **ScanAll** вдруг какой-то элемент массива решит стать пустым, то для удаления его из коллекции правило со **SkipEmpty** лучше опустить ниже правила со **ScanAll**.

## "External polymorphism"

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat
	{
		Name = "  Snow  ",
		Kitten = new Cat
		{
			Name = "  Red  ",
			Kitten = new Cat
			{
				Name = " Bully "
			}
		}
	};

	dto.FixValue(cat, nameof(cat), x => x.NotEmpty().NotEmpty().ValidateDto());
	//cat.Name == "Snow"
	//cat.Kitten.Name == "Red"
	//cat.Kitten.Kitten.Name == "Bully"
}

class Cat
{
	public string Name { get; set; }
	public Cat Kitten { get; set; }
}

[DtoContainer]
public static class Dto
{
	[DtoValidate]
	static void Validate(ValidationRuleFactory<Cat> t)
	{
		t.RuleFor(x => x.Name).Trim();
		t.RuleFor(x => x.Kitten).ValidateDto();
	}
}
