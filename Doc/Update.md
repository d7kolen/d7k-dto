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

## Если объекты не имеют общих интерфейсов
