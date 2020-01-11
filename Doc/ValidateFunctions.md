## Функции валидации

#### Empty
| Функция | Назначение | Комментарий |
| - | - | - |
| NotEmpty | Проверяет, что объект не является пустым значение | Null, пустая строка (IsNullOrWhiteSpace), пустой массив или IList - это пустые Empty значения; 0 любого типа - нормальное не пустое значение, ничем не хуже единицы |
| FixEmpty | В случае, если значение пустое заменяет его на значение по умолчанию | Для массивов - это пустой массив; для строк - пустая строка; для объектов - объект без дополнительной инициализации |
| Trim | Подчищает объект, приводя к некоторому стандартному виду | Тримит строки и если они становятся пустыми превращает их в NULL; проверяет коллекции, что они не пустые - пустые превращает в NULL |
| SkipEmpty | Выкидывает из набора пустые значения |  |

#### Value
| Функция | Назначение | Комментарий |
| - | - | - |
| Greater | Проверяет, что объект строго больше заданой границы | Сравнение производится с помощью IComparable реализации границы |
| NotLesser | Проверяет, что объект не меньше границы (больше или равен) | Сравнение производится с помощью IComparable реализации границы |
| Lesser | Проверяет, что объект не строго больше границы | Сравнение производится с помощью IComparable реализации границы |
| NotGreater | Проверяет, что объект не больше границы (меньше или равен) | Сравнение производится с помощью IComparable реализации границы |
| FixLesser | Корректирует значение в значение границы, если оно меньше границы | Сравнение производится с помощью IComparable реализации границы |
| FixGreater | Корректирует значение в значение границы, если оно больше границы | Сравнение производится с помощью IComparable реализации границы |
| Enum | Проверяет, что объект определен в перечислении | |
| Forbidden | Запрещает значения из переданного набора | |
| Available | Разрешает только значения из переданного набора | |

#### String (and sometime arrays)
| Функция | Назначение | Комментарий |
| - | - | - |
| LengthBetween | Ограничивает длину строк строк и коллекций | Можно ограничить длину сверху, снизу, сверху и снизу; значение NULL у границы приводит к ее игнорированию |
| Length | Требует, чтобы строка или коллекция были заданной длины |  |
| FixToLower | Опускает регистр всех символов в строке | "aBcD" -> "abcd" |
| FixToUpper | Поднимает регистр всех символов в строке | "aBcD" -> "ABCD" |
| Email | Проверяет, что строка - это корректный email | |
| Url | Проверяет, что строка - это URL | |
| FixFileName | Превращает строку в корректное имя файла | "aB$#cd" -> "aB__cd" |
| FixLatinFileName | Превращает строку в корректное имя файла, состоящее из латинских букв | "aФ$#cШ" -> "a___c_" |
| Base64 | Проверяет формат Base64 строки | |

#### Date
| Функция | Назначение | Комментарий |
| - | - | - |
| NewEra | Проверяет дату, что она больше 1900-01-01 | |
| EarlierNow | Проверяет что значение даты более раннее чем Now | |
| LaterNow | Проверяет что значение даты более позднее чем Now | |
| FixEarlierNow | Корректирует значение на Now если оно более раннее чем Now | |
| FixLaterNow | Корректирует значение на Now если оно более позднее чем Now | |
| ZeroOffset | Проверяет что смещение зоны у переданного значение даты нулевое | |

#### Custom
| Функция | Назначение | Комментарий |
| - | - | - |
| Complex | Позволяет тонко настроить валидацию для значения |  |
| Check | Производит пользовательскую (не стандартную) проверку значения |  |
| Fix | Корректирует значение, используюя пользовательскую логику |  |
| Cast | Трансформирует тип значения в другой тип | Будьте внимательны. По завершению валидации произойдет запись значения в структуру данных. Поэтому тип при транформации необходимо по завершению проверки повторно привести к совместимому со структурой |
| ValidateDto | Валидирует значение с учетом всех имеющихся у DtoComplex правил валидации | С помощью этого метода появлятся "полиморфный" эффект: не важно какого типа значение (не поля), мы сделаем для него все возможное |