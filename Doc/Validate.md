# Валидация

Все данные, приходящие от клиентов, необходимо проверять. **d7k.Dto** предоставляет такую возможность:

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

C помощью **x.NotEmpty()** вызова проверяется что переданный ему объект не является пустым (для классов - это проверка на NULL). Если бы объект cat оказался пустым, то **FixValue** поднял бы исключение ValidateException с текстом: "'cat' cannot be empty.". В этом исключении поле Issues содержало бы экземпляр этой ошибки.

Если бы в примере было бы **x.NotEmpty().NotEmpty()**, то для пустого значения сообщение "'cat' cannot be empty." в исключении содержалось бы дважды. Issues бы содержал бы два экземпляра этой ошибки.

**DtoComplex** старается сделать все предложенные проверки и старается сформировать максимально полный набор проблем переданного объекта. Это особенно полезно, когда переданный объект содержит более одной проблемы в различных полях.

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

Вызов **x.ValidateDto()** самостоятельно выбирает методы валидации объекта из настроек **DtoComplex**. Как и в случае с копированием настройка **DtoComplex** производится через механизм **DtoContainer**.

Методы валидации отдельных типов можно определить через **DtoValidate** атрибут. Вы можете найти все возможные сигнатуры этих методов в комментариях к **DtoValidate** классу.

Все Validate методы в сигнатурах имеют **ValidateRuleFactory** объект. Он предоставляет возможность определять правила для проверки отдельных компонентов проверяемого объекта. Так, в примере определены правила для поля Name. Прежде всего его очищают от невидимых символов в начале и конце строкового значения (если строка после этого пустеет, то значение заменяется на NULL).

Это еще одна особенность валидации с помощью **DtoComplex**. Он не только проверяет значения на валидность, но способен так же корректировать их в соответствии с вашими потребностями.

## Стандартизация пустых значений

Для каждой вашей структуры существует свой ответ, что такое "пустое значение" в том или ином ее поле: null, string.Empty, пустой массив или кастомный NullObject. В одном месте удобен один подход, в другом то же самое решение приводит к странным последствиям. Например к таким проверкам в других местах вашего кода: **if (obj.A is MyNullObject) ...**. Поэтому только вы сами можете сказать, что лучше в конкретном месте.

Выявление и стандартизация пустых значений на ранних этапах исполнения вашего кода исключит появление ненужных **if** конструкиций в дальнейшем. Сделает код более ориентированным на решение бизнес задач, а не "проблем" языка (в нашем случае C#).

К сожалению, заставить пользователя вашей библиотеки или клиента вашего сервиса всегда следовать заданным требованиям - задача противоричивая и приводит к конфликтам внутри команды:
- Почему я должен в поле A передавать пустой массив, а в поле B **null**?
- Ну ... моя ORM будет трактовать это значение как ...

ORM? Серьезно? Какое дело клиентскому вашего кода до выбранной вами ORM? У него есть 50 аналогичных арументов сделать иначе.

**d7k.Dto** считает, что клиент может следовать любому, удобному для него, подходу. Во время валидации входных значений ваш сервис или библиотека легко смогут привести все в "правильный" для них вид. Пусть останются счастливы **клиент** и ваша **"ORM"** одновременно.

Стандартизация пустых значений может быть осуществлена с помощью **NotEmpty** и **FixEmpty** правил валидации. 

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

В данном примере метод валидации смотрит не на класс, а на интерфейс. Это особенно полезно, когда в вашей системе имеется множество классов которые реализуют этот интерфейс (или просто они ассоциированы с ним в **DtoContainer-е**). Вы однажды прописали правила валидации и дальше эти правила применяются автоматически там, где встречается ваш интерфейс.

Маленькая особенность данного примера в том, что IName должен быть публичным из-за особенностей реализации **DtoComplex** (Emit и динамические сборки). Я бы посоветовал делать все такие интерфейсы публичными. Это не должно доставить проблем, так как все подобные интерфейсы собраны внутри класса контейнера.

## Валидация коллекций элементов

Давайте поговорим про массивы и списки. Словари не в счет - для них пока нет коробочной реализации подобного.

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var cat = new Cat() { KittenNames = new[] { "  Snow  ", "  ", " Red " } };

	dto.FixValue(cat, nameof(cat), x => x.ValidateDto());
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

И так, в нашем примере в объекте Cat перечислены имена котят.

Прежде всего в правиле для **x.KittenNames** происходит поиск пустых значений в массиве и избавление от них. Это ответственность метода **SkipEmpty()**.

Если в массиве не осталось не одного элемента, то ссылка на него заменяется на NULL с помощью метода **Trim()** для массива.

Далее, определено правило проверки каждого элемента массива отдельно, с помощью конструкции x.KittenNames.**ScanAll()**.

Можно не беспокоиться о том, что массив в какой-то момент может прийти пустым. Делегаты в **RuleFor** никогда не исполняются. Это даже не и делагаты совсем, а Expression-ы. Их задача определить правила доступа к частям объекта. Метод **ScanAll**, так же не вызывается - он лишь говорит, что от правила ожидается сканирование всех объектов коллекции. Более того, сам по себе вызов **ScanAll** в 100% случаев приводит к исключению NotImplementedException.

Все валидаторы вызываются в том же порядке, что и определены. Поэтому если во время **ScanAll** вдруг какой-то элемент массива решит стать пустым, то для его удаления придется повторно определить правило **SkipEmpty**. Как вариант, можно поменять местами правила для элементов массива и правила для самого массива.

## Валидация вложенных структур данных

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

	dto.FixValue(cat, nameof(cat), x => x.ValidateDto());
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
```

Как было сказано выше, метод **ValidateDto** самостоятельно выбирает нужный валидатор. В этом примере ничего нового, кроме демонстрации возможности достаточно компактно и без дублирования определять валидацию сильно вложенных и рекурсивных структур.

Главное, что бы не возникало зацикливания - это прямой путь к StackOverflow. **На текущий момент** библиотека не отслеживает это.

## Вариативность при валидации

Представим ситуацию, что у вас в системе у запросов на изменение есть поле *Список полей для обновления*. Вы обновляете только их, а остальные поля в запросе вы игнорируете. В таком случае налагать на необновляемые поля какие-то ограничения избыточно:

```csharp
static void Main(string[] args)
{
	var dto = new DtoComplex().ByNestedClassesWithAttributes();

	var updateCat = new UpdateCat
	{
		Age = 1,
		UpdationList = new[] { nameof(UpdateCat.Age) }.ToList()
	};

	dto.FixValue(updateCat, nameof(updateCat), x => x.ValidateDto());

	var newCat = new NewCat
	{
		Age = 1,
		Weight = 2
	};

	dto.FixValue(newCat, nameof(newCat), x => x.ValidateDto());
}

class UpdateCat
{
	public int Age { get; set; }
	public int Weight { get; set; }
	public List<string> UpdationList { get; set; } = new List<string>();
}

class NewCat
{
	public int Age { get; set; }
	public int Weight { get; set; }
}

[DtoContainer]
public static class Dto
{
	public interface ICat
	{
		int Age { get; set; }
		int Weight { get; set; }
	}

	[DtoValidate]
	static void Validate(ValidationRuleFactory<ICat> t)
	{
		var updateCat = t.OriginalValue as UpdateCat;

		if (updateCat == null || updateCat.UpdationList.Contains(nameof(ICat.Age)))
			t.RuleFor(x => x.Age).Greater(0);

		if (updateCat == null || updateCat.UpdationList.Contains(nameof(ICat.Weight)))
			t.RuleFor(x => x.Weight).Greater(0);
	}

	class UpdateCat_Dto : UpdateCat, ICat { }
	class NewCat_Dto : NewCat, ICat { }
}
```

В валидаторе вы имеете доступ к своему оригинальному объекту через свойство **OriginalValue**. Если у вас есть необходимость в вариативности, то у вас есть возможность ее организовать.
