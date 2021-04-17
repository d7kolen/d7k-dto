using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Linq;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace d7k.Dto.Tests
{
	[TestClass]
	public class ValidationRuleTests
	{
		[TestMethod]
		public void RequiredTest()
		{
			var lot = new SaveLotDtoTemp();

			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(lot);

			validation.RuleFor(x => x.BaseExchangeHousePriceRubGram).AddValidator(new NotEmptyRule(typeof(decimal)));
			validation.RuleFor(x => x.ExchangeHousePriceRubGram).AddValidator(new NotEmptyRule(typeof(decimal)));
			validation.RuleFor(x => x.WeightGram).AddValidator(new NotEmptyRule(typeof(decimal)));
			validation.RuleFor(x => x.PriceRubGram).AddValidator(new NotEmptyRule(typeof(decimal)));
			validation.RuleFor(x => x.Sum).AddValidator(new NotEmptyRule(typeof(decimal)));

			validation.Validate(lot).Issues.Should().HaveCount(5);
		}

		[TestMethod]
		public void RequiredArrayTest()
		{
			var value = new { arr = new[] { 1, 2, 3 } };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.arr).NotEmpty();
			});

			validator.Validate(value).Issues.Should().HaveCount(0);
			validator.Validate(new { arr = (int[])null }).Issues.Should().HaveCount(1);
			validator.Validate(new { arr = new int[0] }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void RequiredBoolTest()
		{
			var value = new { a = false };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.a).NotEmpty();
			});

			validator.Validate(value).Issues.Should().HaveCount(0);
			validator.Validate(new { a = false }).Issues.Should().HaveCount(0);
			validator.Validate(new { a = true }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void RequiredIntTest()
		{
			var value = new { a = (int)0 };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.a).NotEmpty();
			});

			validator.Validate(new { a = 0 }).Issues.Should().HaveCount(0);
			validator.Validate(new { a = 1 }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void RequiredDecimalTest()
		{
			var value = new { a = (decimal)0 };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.a).NotEmpty();
			});

			validator.Validate(new { a = 0.0m }).Issues.Should().HaveCount(0);
			validator.Validate(new { a = 1.0m }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void RequiredDoubleTest()
		{
			var value = new { a = (double)0 };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.a).NotEmpty();
			});

			validator.Validate(new { a = 0.0 }).Issues.Should().HaveCount(0);
			validator.Validate(new { a = 1.0 }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void RequiredDateTimeTest()
		{
			var value = new { a = DateTime.MinValue };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.a).NotEmpty();
			});

			validator.Validate(new { a = DateTime.MinValue }).Issues.Should().HaveCount(1);
			validator.Validate(new { a = DateTime.UtcNow }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void RequiredListTest()
		{
			var value = new { arr = new[] { 1, 2, 3 }.ToList() };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.arr).NotEmpty();
			});

			validator.Validate(value).Issues.Should().HaveCount(0);
			validator.Validate(new { arr = (List<int>)null }).Issues.Should().HaveCount(1);
			validator.Validate(new { arr = new List<int>() }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void LengthBetweenArrayTest()
		{
			var value = new { arr = new[] { 1, 2, 3 } };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.arr).LengthBetween(2, 3);
			});

			validator.Validate(value).Issues.Should().HaveCount(0);
			validator.Validate(new { arr = (int[])null }).Issues.Should().HaveCount(0);
			validator.Validate(new { arr = new[] { 1 } }).Issues.Should().HaveCount(1);
			validator.Validate(new { arr = new[] { 1, 2, 3, 4 } }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void SelfRequiredTest()
		{
			var lot = new SaveLotDtoTemp();

			var value = new { lot };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.lot).NotEmpty();
			});

			var result = validator.Validate(value);
			result.Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void SelfRequiredForIntTest()
		{
			var value = new { t = 2 };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.t).Greater(2);
			});

			var result = validator.Validate(value);
			result.Issues.Should().HaveCount(1);
			result.Issues[0].Message.Should().Be("'.t' is not greater than 2.");

			validator.Validate(new { t = 3 }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void ValidationObjTest()
		{
			var obj = new ValidationObj()
			{
				Obj = new ValidationObj()
			};

			var validation = new ValidationObjValidation1(new ValidationRepository());

			var result = validation.Validate(obj);
			result.Issues.Should().HaveCount(2);
		}

		[TestMethod]
		public void ValidationObjWithSecondRequiredTest()
		{
			var obj = new ValidationObj()
			{
				Obj = new ValidationObj()
				{
					Obj = new ValidationObj()
				}
			};

			var validation = new ValidationObjValidation1(new ValidationRepository());
			validation.RuleFor(x => x.Obj.Obj.A).AddValidator(new NotEmptyRule(null));

			var result = validation.Validate(obj);
			result.Issues.Should().HaveCount(2);
		}

		[TestMethod]
		public void GreaterThanTest()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).AddValidator(new CompareRule.Greater() { Value = 1 });

			validation.Validate(new SaveLotDtoTemp() { Sum = 2 }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 1 }).Issues.Should().HaveCount(1);
			validation.Validate(new SaveLotDtoTemp() { Sum = 0.5m }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void GreaterThan_WithInit_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).Greater(1);

			validation.Validate(new SaveLotDtoTemp() { Sum = 2 }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 1 }).Issues.Should().HaveCount(1);
			validation.Validate(new SaveLotDtoTemp() { Sum = 0.5m }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void NotLesserThanTest()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).AddValidator(new CompareRule.NotLesser() { Value = 1 });

			validation.Validate(new SaveLotDtoTemp() { Sum = 2 }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 1 }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 0.5m }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void NotLesserThan_WithInit_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).NotLesser(1);

			validation.Validate(new SaveLotDtoTemp() { Sum = 2 }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 1 }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 0.5m }).Issues.Should().HaveCount(1);
		}


		[TestMethod]
		public void FixLesser_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).FixLesser(1);

			var obj = new SaveLotDtoTemp() { Sum = 2 };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Sum.Should().Be(2);

			obj = new SaveLotDtoTemp() { Sum = 0.5m };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Sum.Should().Be(1);
		}

		[TestMethod]
		public void FixLength_Test0()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Comment).FixLength(2);

			var obj = new SaveLotDtoTemp() { Comment = "123" };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Comment.Should().Be("12");

			obj = new SaveLotDtoTemp() { Comment = "1" };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Comment.Should().Be("1");
		}

		[TestMethod]
		public void FixLength_Test1()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Timestamp).FixLength(2);
			validation.RuleFor(x => x.List).FixLength(2);

			var obj = new SaveLotDtoTemp() { Timestamp = new byte[] { 1, 2, 3 }, List = new List<byte> { 1, 2, 3 } };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Timestamp.Should().HaveCount(2);
			obj.Timestamp[0].Should().Be(1);
			obj.Timestamp[1].Should().Be(2);
			obj.List.Should().HaveCount(2);
			obj.List[0].Should().Be(1);
			obj.List[1].Should().Be(2);

			obj = new SaveLotDtoTemp() { Timestamp = new byte[] { 1 }, List = new List<byte> { 1 } };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Timestamp.Should().HaveCount(1);
			obj.Timestamp[0].Should().Be(1);
			obj.List.Should().HaveCount(1);
			obj.List[0].Should().Be(1);
		}

		[TestMethod]
		public void FixLength_Test2()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Timestamp).FixLength(0);
			validation.RuleFor(x => x.List).FixLength(0);

			var obj = new SaveLotDtoTemp() { Timestamp = new byte[] { 1, 2, 3 }, List = new List<byte> { 1, 2, 3 } };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Timestamp.Should().HaveCount(0);
			obj.List.Should().HaveCount(0);
		}

		[TestMethod]
		public void FixMantissa_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).RoundMantissa(2);
			validation.RuleFor(x => x.NotNullableSum).RoundMantissa(2);

			var obj = new SaveLotDtoTemp() { Sum = 4.11111M, NotNullableSum = 4.999999M };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Sum.Should().Be(4.11M);
			obj.NotNullableSum.Should().Be(5M);
		}

		[TestMethod]
		public void LesserThanTest()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).AddValidator(new CompareRule.Lesser() { Value = 1 });

			validation.Validate(new SaveLotDtoTemp() { Sum = 0.5m }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 1 }).Issues.Should().HaveCount(1);
			validation.Validate(new SaveLotDtoTemp() { Sum = 2 }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void LesserThan_WithInit_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).Lesser(1);

			validation.Validate(new SaveLotDtoTemp() { Sum = 0.5m }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 1 }).Issues.Should().HaveCount(1);
			validation.Validate(new SaveLotDtoTemp() { Sum = 2 }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void NotGreaterThanTest()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).AddValidator(new CompareRule.NotGreater() { Value = 1 });

			validation.Validate(new SaveLotDtoTemp() { Sum = 0.5m }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 1 }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 2 }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void NotGreaterThan_WithInit_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).NotGreater(1);

			validation.Validate(new SaveLotDtoTemp() { Sum = 0.5m }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 1 }).Issues.Should().HaveCount(0);
			validation.Validate(new SaveLotDtoTemp() { Sum = 2 }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void FixGreater_Test()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Sum).FixGreater(1);

			var obj = new SaveLotDtoTemp() { Sum = 0.5m };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Sum.Should().Be(0.5m);

			obj = new SaveLotDtoTemp() { Sum = 2m };
			validation.Validate(obj).Issues.Should().HaveCount(0);
			validation.Validate(obj).Update();

			obj.Sum.Should().Be(1m);
		}

		[TestMethod]
		public void ZeroOffsetRuleTest()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.FixDate).AddValidator(new ZeroOffsetRule());

			validation.Validate(new SaveLotDtoTemp() { FixDate = new DateTimeOffset(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified), TimeSpan.FromHours(3)) }).Issues.Should().HaveCount(1);
			validation.Validate(new SaveLotDtoTemp() { FixDate = new DateTimeOffset(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified), TimeSpan.FromHours(0)) }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void ZeroOffsetRule_WithInitTest()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.FixDate).ZeroOffset();

			validation.Validate(new SaveLotDtoTemp() { FixDate = new DateTimeOffset(DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified), TimeSpan.FromHours(3)) }).Issues.Should().HaveCount(1);
			validation.Validate(new SaveLotDtoTemp() { FixDate = new DateTimeOffset(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Unspecified), TimeSpan.FromHours(0)) }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void TrimRuleTest()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Comment).AddValidator(new TrimRule());

			var t = new SaveLotDtoTemp() { Comment = "1" };
			validation.Fix(t);
			t.Comment.Should().Be("1");

			t.Comment = " 1 ";
			validation.Fix(t);
			t.Comment.Should().Be("1");
		}

		[TestMethod]
		public void TrimRule_WithInitTest()
		{
			var validation = new ValidationRepository().Create<SaveLotDtoTemp>(null);
			validation.RuleFor(x => x.Comment).Trim();

			var t = new SaveLotDtoTemp() { Comment = "1" };
			validation.Fix(t);
			t.Comment.Should().Be("1");

			t.Comment = " 1 ";
			validation.Fix(t);
			t.Comment.Should().Be("1");
		}

		[TestMethod]
		public void TrimRule_WithValidationRulesTest()
		{
			var value = new { t = " 1 " };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.t).Trim();
			});

			validator.Fix(value);
			value.t.Should().Be("1");
		}

		[TestMethod]
		public void TrimRule_WithValidationRulesTest1()
		{
			var value = new { t = new { a = " 1 " } };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.t.a).Trim();
			});

			validator.Fix(value);
			value.t.a.Should().Be("1");
		}

		[TestMethod]
		public void TrimRule_WithValidationRulesTest2()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = " 1 " } };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.Obj.B).Trim();
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("1");
		}

		[TestMethod]
		public void TrimRule_ForEmptyString_Test1()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "" } };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.Obj.B).Trim();
			});

			validator.Fix(value);
			value.Obj.B.Should().Be(null);
		}

		[TestMethod]
		public void TrimRule_ForEmptyString_Test2()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = " " } };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.Obj.B).Trim();
			});

			validator.Fix(value);
			value.Obj.B.Should().Be(null);
		}

		[TestMethod]
		public void TrimRuleForArrays_EmptyArray_Test()
		{
			var value = new { Obj = new { B = new string[0] } };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.Obj.B).Trim();
			});

			validator.Fix(value);
			value.Obj.B.Should().BeNull();
		}

		[TestMethod]
		public void TrimRuleForArrays_ForFullArray_Test()
		{
			var value = new { Obj = new { B = new string[] { "abc" } } };
			var validator = new ValidationRepository().Create(value, x =>
			{
				x.RuleFor(t => t.Obj.B).Trim();
			});

			validator.Fix(value);
			value.Obj.B.Should().BeEquivalentTo("abc");
		}

		[TestMethod]
		public void ToLowerRuleTest()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "A" } };
			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.Obj.B).FixToLower();
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("a");
		}

		[TestMethod]
		public void ToLowerRule_WithoutChange_Test()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "a" } };
			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.Obj.B).FixToLower();
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("a");
		}

		[TestMethod]
		public void ToUpperRuleTest()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "A" } };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.Obj.B).FixToUpper();
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("A");
		}

		[TestMethod]
		public void MaxLengthRuleTest()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "AAAA" } };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.Obj.B).FixLength(2);
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("AA");
		}

		[TestMethod]
		public void MaxLengthRule_WithoutChange_Test()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "AAAA" } };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.Obj.B).FixLength(6);
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("AAAA");
		}

		[TestMethod]
		public void MaxLengthRule_Exactly_Test()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "AAAA" } };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.Obj.B).FixLength(4);
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("AAAA");
		}

		[TestMethod]
		public void MaxLengthRule_Zero_Test()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "AAAA" } };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.Obj.B).FixLength(0);
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("");
		}

		[TestMethod]
		public void ToUpperRule_WithoutChange_Test()
		{
			var value = new ValidationObj() { Obj = new ValidationObj() { B = "a" } };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.Obj.B).FixToUpper();
			});

			validator.Fix(value);
			value.Obj.B.Should().Be("A");
		}

		[TestMethod]
		public void NotEmptyTest()
		{
			var value = new { t = "" };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.t).NotEmpty();
			});

			validator.Validate(value).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void NotEmpty_Nullable_Test()
		{
			var value = new { t = (int?)0 };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.t).NotEmpty();
			});

			validator.Validate(new { t = (int?)1 }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = (int?)0 }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = (int?)null }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void LengthBetweenTest()
		{
			var value = new { t = "aaa" };

			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.t).LengthBetween(2, 3);
			});

			validator.Validate(new { t = "aa" }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = "aaa" }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = "a" }).Issues.Should().HaveCount(1);
			validator.Validate(new { t = "aaaa" }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void EmailTest()
		{
			var value = new { t = "d7kolen@gmail.com" };
			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.t).Email();
			});

			validator.Validate(new { t = "d7kolen-gmail.com" }).Issues.Should().HaveCount(1);
			validator.Validate(new { t = "d7kolen@gmail.com" }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = "d7kolen+4" }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void ValidationWithPrefixTest()
		{
			var value = new { t = "d7kolen@gmail.com" };
			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.t).Email();
			});

			validator.Validate(new { t = "d7kolen@gmail.com" }).Issues.Should().HaveCount(0);

			var obj = new { t = "d7kolen-gmail.com" };
			var context = new ValidationContext("MyObject", obj);
			var result = validator.ValidateObject(obj, context).Issues;

			result.Should().HaveCount(1);
			result[0].ValuePath.Should().Be("MyObject.t");
		}

		[TestMethod]
		public void UrlTest()
		{
			var value = new { t = "http://www.ya.ru" };
			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.t).Url();
			});

			validator.Validate(new { t = "http://www.ya.ru" }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = "aaa" }).Issues.Should().HaveCount(1);
			validator.Validate(new { t = "www.ya.ru" }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void FileNameTest()
		{
			var value = new { t = "abc" };
			var validator = new ValidationRepository().Create(value, t =>
			{
				t.RuleFor(x => x.t).FixFileName();
			});

			validator.Fix(value = new { t = "abc" });
			value.t.Should().Be("abc");

			validator.Fix(value = new { t = "abc.txt" });
			value.t.Should().Be("abc.txt");

			validator.Fix(value = new { t = "abc+=.txt" });
			value.t.Should().Be("abc+=.txt");

			validator.Fix(value = new { t = @"abc/\?.txt" });
			value.t.Should().Be("abc___.txt");
		}

		[TestMethod]
		public void EnumTest()
		{
			var validator = new ValidationRepository().Create(new { t = TestEnum.Test0 }, t =>
			{
				t.RuleFor(x => x.t).Enum();
			});

			validator.Validate(new { t = TestEnum.Test0 }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = (TestEnum)(-1) }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void ForbiddenForEnumTest()
		{
			var validator = new ValidationRepository().Create(new { t = TestEnum.Test0 }, t =>
			{
				t.RuleFor(x => x.t).Forbidden(TestEnum.Test0);
			});

			validator.Validate(new { t = TestEnum.Test0 }).Issues.Should().HaveCount(1);
			validator.Validate(new { t = TestEnum.Test1 }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void ForbiddenForIntTest()
		{
			var validator = new ValidationRepository().Create(new { t = 1 }, t =>
			{
				t.RuleFor(x => x.t).Forbidden(1);
			});

			validator.Validate(new { t = 1 }).Issues.Should().HaveCount(1);
			validator.Validate(new { t = 2 }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = 0 }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void ForbiddenForStringTest()
		{
			var validator = new ValidationRepository().Create(new { t = "abc" }, t =>
			{
				t.RuleFor(x => x.t).Forbidden("abc");
			});

			validator.Validate(new { t = "abc" }).Issues.Should().HaveCount(1);
			validator.Validate(new { t = "abc1" }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = "abc " }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = "Abc" }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void ForbiddenForString_WithNumRule_Test()
		{
			var validator = new ValidationRepository().Create(new { t = "abc" }, t =>
			{
				t.RuleFor(x => x.t).Forbidden((string)null);
			});

			validator.Validate(new { t = (string)null }).Issues.Should().HaveCount(0);
			validator.Validate(new { t = "abc" }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void Base64_Test()
		{
			var validator = new ValidationRepository().Create(new { t = "abc" }, t =>
			{
				t.RuleFor(x => x.t).Base64();
			});

			validator.Validate(new { t = "abc" }).Issues.Should().HaveCount(1);
			validator.Validate(new { t = Convert.ToBase64String(new byte[] { 1, 2, 3 }) }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void ComplexRule_Test()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { k = new { a = 1, b = "def" } }, t =>
			{
				t.RuleFor(x => x.k).Complex(t1 =>
				{
					t1.RuleFor(x => x.a).Greater(0);
					t1.RuleFor(x => x.b).LengthBetween(2, 3);
				});
			});

			validator.Validate(new { k = new { a = 1, b = "abc" } }).Issues.Should().HaveCount(0);
			validator.Validate(new { k = new { a = -1, b = "abc" } }).Issues.Should().HaveCount(1);
			validator.Validate(new { k = new { a = -1, b = "abc+asb" } }).Issues.Should().HaveCount(2);
		}

		[TestMethod]
		public void ComplexRule_Test1()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { k = new { a = 1, b = "def" } }, t =>
			{
				t.RuleFor(x => x.k).Complex(repository.Create(t.Example()?.k, t1 =>
				{
					t1.RuleFor(x => x.a).Greater(0);
					t1.RuleFor(x => x.b).LengthBetween(2, 3);
				}));
			});

			validator.Validate(new { k = new { a = 1, b = "abc" } }).Issues.Should().HaveCount(0);
			validator.Validate(new { k = new { a = -1, b = "abc" } }).Issues.Should().HaveCount(1);
			validator.Validate(new { k = new { a = -1, b = "abc+asb" } }).Issues.Should().HaveCount(2);
		}

		[TestMethod]
		public void ComplexRule_WithUpdate_Test()
		{
			var repository = new ValidationRepository();

			var value = new { k = new { a = 1, b = " abc " } };

			var validator = repository.Create(value, t =>
			{
				t.RuleFor(x => x.k).Complex(t1 =>
				{
					t1.RuleFor(x => x.a).Greater(0);
					t1.RuleFor(x => x.b).Trim();
				});
			});

			validator.Fix(value).k.b.Should().Be("abc");
		}

		[TestMethod]
		public void AllArrayRule_Test()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { k = new[] { new { a = 1 } } }, t =>
			{
				t.RuleFor(x => x.k.First()).Complex(t1 =>
				{
					t1.RuleFor(x => x.a).Greater(0);
				});
			});

			validator.Validate(new { k = new[] { new { a = 1 }, new { a = 2 } } }).Issues.Should().HaveCount(0);
			validator.Validate(new { k = new[] { new { a = -1 }, new { a = 2 } } }).Issues.Should().HaveCount(1);
			validator.Validate(new { k = new[] { new { a = -1 }, new { a = -2 } } }).Issues.Should().HaveCount(2);
		}

		[TestMethod]
		public void AllArrayRule_Test1()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { k = new[] { new { a = 1 } } }, t =>
			{
				t.RuleFor(x => x.k.First()).Complex(t1 =>
				{
					t1.RuleFor(x => x.a).Greater(0);
				});
			});

			validator.Validate(new { k = new[] { new { a = 1 }, new { a = 2 } } }).Issues.Should().HaveCount(0);
			validator.Validate(new { k = new[] { new { a = -1 }, new { a = 2 } } }).Issues.Should().HaveCount(1);
			validator.Validate(new { k = new[] { new { a = -1 }, new { a = -2 } } }).Issues.Should().HaveCount(2);
		}

		[TestMethod]
		public void CustomRule_Test()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { a = 1, b = 2 }, t =>
			{
				t.RuleFor(x => x).Check((c, x) =>
				{
					if (x.a == x.b)
						return new[] { c.Issue(null, $"Custom.SameAB", $"a and b cannot be same.") };
					return null;
				});
			});

			validator.Validate(new { a = 1, b = 2 }).Issues.Should().HaveCount(0);
			validator.Validate(new { a = 1, b = 1 }).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void ConvertRule_Test()
		{
			var repository = new ValidationRepository();

			var obj = new { a = 1 };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.a).Fix(x => 2);
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			obj.a.Should().Be(1);

			validator.Validate(obj).Update();
			obj.a.Should().Be(2);
		}

		[TestMethod]
		public void AvailableRule_Test()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { a = "abc" }, t =>
			{
				t.RuleFor(x => x.a).Available("abc", "def");
			});

			validator.Validate(new { a = "abc" }).Issues.Should().HaveCount(0);
			validator.Validate(new { a = "def" }).Issues.Should().HaveCount(0);
			validator.Validate(new { a = "ghi" }).Issues.Should().HaveCount(1);
			validator.Validate(new { a = (string)null }).Issues.Should().HaveCount(0);
			validator.Validate(new { a = "" }).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void SkipEmpty_Test()
		{
			var repository = new ValidationRepository();

			var obj = new { a = new[] { null, "abc", "def" } };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.a).SkipEmpty();
			});

			validator.Fix(obj);
			obj.a.Should().BeEquivalentTo("abc", "def");
		}

		[TestMethod]
		public void SkipEmpty_WithTrimRule_Test()
		{
			var repository = new ValidationRepository();

			var obj = new { a = new[] { null, "abc", "def", " " } };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.a).SkipEmpty();
				t.RuleFor(x => x.a.First()).Trim();
			});

			validator.Fix(obj);
			obj.a.Should().BeEquivalentTo("abc", "def");
		}

		[TestMethod]
		public void Validate_BasicTypes_Test()
		{
			var repository = new ValidationRepository();

			var request = " abc ";

			request = repository.Fix(new { request }, t =>
			{
				t.RuleFor(x => x.request).Trim();
			}).request;

			request.Should().Be("abc");
		}

		[TestMethod]
		public void Default_Test()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { a = "abc" }, t =>
			{
				t.RuleFor(x => x.a).Trim().FixEmpty(() => "empty");
			});

			validator.Update(new { a = "abc" }).a.Should().Be("abc");
			validator.Update(new { a = "" }).a.Should().Be("empty");
			validator.Update(new { a = "   " }).a.Should().Be("empty");
			validator.Update(new { a = (string)null }).a.Should().Be("empty");
		}

		[TestMethod]
		public void Default_String_Test()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { a = "abc" }, t =>
			{
				t.RuleFor(x => x.a).Trim().FixEmpty();
			});

			validator.Update(new { a = "abc" }).a.Should().Be("abc");
			validator.Update(new { a = "" }).a.Should().Be("");
			validator.Update(new { a = (string)null }).a.Should().Be("");
		}

		[TestMethod]
		public void Default_Nullable_Test()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { a = (int?)null }, t =>
			{
				t.RuleFor(x => x.a).FixEmpty();
			});

			validator.Update(new { a = (int?)5 }).a.Should().Be(5);
			validator.Update(new { a = (int?)0 }).a.Should().Be(0);
			validator.Update(new { a = (int?)null }).a.Should().Be(0);
		}

		[TestMethod]
		public void Default_Test1()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { a = new[] { 1 } }, t =>
			{
				t.RuleFor(x => x.a).Trim().FixEmpty();
			});

			validator.Update(new { a = new[] { 1, 2 } }).a.Should().BeEquivalentTo(1, 2);
			validator.Update(new { a = (int[])null }).a.Should().BeEmpty();
		}

		[TestMethod]
		public void Default_Test2()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { a = new List<int>() }, t =>
			{
				t.RuleFor(x => x.a).Trim().FixEmpty();
			});

			validator.Update(new { a = new List<int>() { 1, 2 } }).a.Should().BeEquivalentTo(1, 2);
			validator.Update(new { a = (List<int>)null }).a.Should().BeEmpty();
		}

		[TestMethod]
		public void Default_ForInterface_Test()
		{
			var repository = new ValidationRepository();

			var validator = repository.Create(new { a = (IMyValidationObj)null }, t =>
			{
				t.RuleFor(x => x.a).FixEmpty();
			});

			var res = validator.Update(new { a = (IMyValidationObj)null });
			res.a.Should().NotBeNull();
			(res.a is IMyValidationObj).Should().BeTrue();
		}

		[TestMethod]
		public void FixValue_Test()
		{
			var repository = new ValidationRepository();

			repository.FixValue("a", "param", x => x.Trim()).Should().Be("a");
			repository.FixValue("", "param", x => x.Trim()).Should().Be(null);
			repository.FixValue("   ", "param", x => x.Trim()).Should().Be(null);

			AssertionExtensions.Should(() => repository.FixValue("   ", "param", x => x.NotEmpty()))
				.Throw<ValidationException>();
		}

		[TestMethod]
		public void FixValue_WithComplex_Test()
		{
			var repository = new ValidationRepository();

			var value = new { a = new { b = " a " } };

			var tValue = repository.FixValue(value, nameof(value), t1 => t1.NotEmpty().Complex(t =>
			 {
				 t.RuleFor(x => x.a.b).Trim();
			 }));

			tValue.a.b.Should().Be("a");
		}

		//Memory leak
		//[TestMethod]
		//public void FixValue_WithExpression_Test()
		//{
		//	var repository = new ValidationRepository();
		//	var param = "a";
		//
		//	repository.FixValue(() => param, x => x.Trim()).Should().Be("a");
		//
		//	param = " ";
		//	AssertionExtensions.Should(() => repository.FixValue(() => param, x => x.NotEmpty()))
		//		.Throw<ValidationException>().WithMessage("'param' cannot be empty.");
		//}

		[TestMethod]
		public void DefferedUpdate_Test()
		{
			var repository = new ValidationRepository();

			var value = new { a = new[] { new { b = " a " }, new { b = " b " }, new { b = " " } } };

			var updater = repository.Create(value, t =>
			{
				t.RuleFor(x => x.a.First().b).Trim();
			});

			updater.Validate(value).Update();

			value.a[0].b.Should().Be("a");
			value.a[1].b.Should().Be("b");
			value.a[2].b.Should().Be(null);
		}

		[TestMethod]
		public void ValidateDto_WithIf_Test()
		{
			var repository = new ValidationRepository();

			var check = false;

			repository.Validate(new ValidationObj() { A = 1 }, t =>
			{
				if (check)
					t.RuleFor(x => x.A).Greater(0);
			}).Issues.Should().HaveCount(0);

			check = true;

			repository.Validate(new ValidationObj() { A = 0 }, t =>
			{
				if (check)
					t.RuleFor(x => x.A).Greater(0);
			}).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void ValidateDto_NewEra_Test()
		{
			var repository = new ValidationRepository();

			repository.Validate(new { A = new DateTime(1900, 1, 1) }, t =>
			{
				t.RuleFor(x => x.A).NewEra();
			}).Issues.Should().HaveCount(0);

			repository.Validate(new { A = new DateTimeOffset(new DateTime(1900, 1, 1), TimeSpan.Zero) }, t =>
			{
				t.RuleFor(x => x.A).NewEra();
			}).Issues.Should().HaveCount(0);

			repository.Validate(new { A = new DateTime(1899, 1, 1) }, t =>
			{
				t.RuleFor(x => x.A).NewEra();
			}).Issues.Should().HaveCount(1);

			repository.Validate(new { A = new DateTimeOffset(new DateTime(1899, 1, 1)) }, t =>
			{
				t.RuleFor(x => x.A).NewEra();
			}).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void ValidateDto_EarlierNow_Test()
		{
			var repository = new ValidationRepository();

			repository.Validate(new { A = DateTime.UtcNow.ToLocalTime().AddMinutes(-1) }, t =>
			{
				t.RuleFor(x => x.A).EarlierNow();
			}).Issues.Should().HaveCount(0);

			repository.Validate(new { A = DateTime.UtcNow.ToLocalTime().AddMinutes(+1) }, t =>
			{
				t.RuleFor(x => x.A).EarlierNow();
			}).Issues.Should().HaveCount(1);

			repository.Validate(new { A = DateTime.UtcNow.AddMinutes(-1) }, t =>
			{
				t.RuleFor(x => x.A).EarlierNow();
			}).Issues.Should().HaveCount(0);

			repository.Validate(new { A = DateTime.UtcNow.AddMinutes(+1) }, t =>
			{
				t.RuleFor(x => x.A).EarlierNow();
			}).Issues.Should().HaveCount(1);

			repository.Validate(new { A = (DateTime?)DateTime.UtcNow }, t =>
			{
				t.RuleFor(x => x.A).EarlierNow();
			}).Issues.Should().HaveCount(0);

			var time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-1.5), DateTimeKind.Unspecified);
			repository.Validate(new { A = new DateTimeOffset(time, TimeSpan.FromHours(-1)) }, t =>
			{
				t.RuleFor(x => x.A).EarlierNow();
			}).Issues.Should().HaveCount(0);

			time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-0.5), DateTimeKind.Unspecified);
			repository.Validate(new { A = new DateTimeOffset(time, TimeSpan.FromHours(-1)) }, t =>
			{
				t.RuleFor(x => x.A).EarlierNow();
			}).Issues.Should().HaveCount(1);

			time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-1), DateTimeKind.Unspecified);
			repository.Validate(new { A = (DateTimeOffset?)new DateTimeOffset(time, TimeSpan.FromHours(-1)) }, t =>
			{
				t.RuleFor(x => x.A).EarlierNow();
			}).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void FixLaterNow_Test0()
		{
			var repository = new ValidationRepository();

			var value = DateTime.UtcNow.ToLocalTime().AddHours(-1);
			var obj = new { A = value };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixLaterNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().Be(value);
		}

		[TestMethod]
		public void FixLaterNow_Test1()
		{
			var repository = new ValidationRepository();

			var obj = new { A = DateTime.UtcNow.ToLocalTime().AddHours(+1) };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixLaterNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
			obj.A.Kind.Should().Be(DateTimeKind.Utc);
		}

		[TestMethod]
		public void FixLaterNow_Test2()
		{
			var repository = new ValidationRepository();

			var obj = new { A = DateTime.UtcNow.AddHours(-1) };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixLaterNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().BeCloseTo(DateTime.UtcNow.AddHours(-1), TimeSpan.FromSeconds(2));
			obj.A.Kind.Should().Be(DateTimeKind.Utc);
		}

		[TestMethod]
		public void FixLaterNow_Test3()
		{
			var repository = new ValidationRepository();

			var obj = new { A = DateTime.UtcNow.AddHours(+1) };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixLaterNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
			obj.A.Kind.Should().Be(DateTimeKind.Utc);
		}

		[TestMethod]
		public void FixLaterNow_Test4()
		{
			var repository = new ValidationRepository();

			var obj = new { A = (DateTime?)DateTime.UtcNow };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixLaterNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
			obj.A.Value.Kind.Should().Be(DateTimeKind.Utc);
		}

		[TestMethod]
		public void FixLaterNow_Test5()
		{
			var repository = new ValidationRepository();

			var time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-1.5), DateTimeKind.Unspecified);
			var value = new DateTimeOffset(time, TimeSpan.FromHours(-1));
			var obj = new { A = value };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixLaterNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().Be(value);
		}

		[TestMethod]
		public void FixLaterNow_Test6()
		{
			var repository = new ValidationRepository();

			var time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-0.5), DateTimeKind.Unspecified);
			var value = new DateTimeOffset(time, TimeSpan.FromHours(-1));
			var obj = new { A = value };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixLaterNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().BeCloseTo(new DateTimeOffset(DateTime.UtcNow));
		}

		[TestMethod]
		public void FixLaterNow_Test7()
		{
			var repository = new ValidationRepository();

			var time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-1), DateTimeKind.Unspecified);
			var value = new DateTimeOffset(time, TimeSpan.FromHours(-1));
			var obj = new { A = value };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixLaterNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().Be(value);
		}


		[TestMethod]
		public void FixEarlierNow_Test0()
		{
			var repository = new ValidationRepository();

			var value = DateTime.UtcNow.ToLocalTime().AddHours(+1);
			var obj = new { A = value };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixEarlierNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().Be(value);
		}

		[TestMethod]
		public void FixEarlierNow_Test1()
		{
			var repository = new ValidationRepository();

			var obj = new { A = DateTime.UtcNow.ToLocalTime().AddHours(-1) };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixEarlierNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
			obj.A.Kind.Should().Be(DateTimeKind.Utc);
		}

		[TestMethod]
		public void FixEarlierNow_Test2()
		{
			var repository = new ValidationRepository();

			var value = DateTime.UtcNow.AddHours(+1);
			var obj = new { A = value };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixEarlierNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().Be(value);
		}

		[TestMethod]
		public void FixEarlierNow_Test3()
		{
			var repository = new ValidationRepository();

			var obj = new { A = DateTime.UtcNow.AddHours(-1) };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixEarlierNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
			obj.A.Kind.Should().Be(DateTimeKind.Utc);
		}

		[TestMethod]
		public void FixEarlierNow_Test4()
		{
			var repository = new ValidationRepository();

			var obj = new { A = (DateTime?)DateTime.UtcNow };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixEarlierNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
			obj.A.Value.Kind.Should().Be(DateTimeKind.Utc);
		}

		[TestMethod]
		public void FixEarlierNow_Test5()
		{
			var repository = new ValidationRepository();

			var time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-0.5), DateTimeKind.Unspecified);
			var value = new DateTimeOffset(time, TimeSpan.FromHours(-1));
			var obj = new { A = value };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixEarlierNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().Be(value);
		}

		[TestMethod]
		public void FixEarlierNow_Test6()
		{
			var repository = new ValidationRepository();

			var time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-1.5), DateTimeKind.Unspecified);
			var value = new DateTimeOffset(time, TimeSpan.FromHours(-1));
			var obj = new { A = value };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixEarlierNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);

			validator.Fix(obj);

			obj.A.Should().BeCloseTo(new DateTimeOffset(DateTime.UtcNow));
		}

		[TestMethod]
		public void FixEarlierNow_Test7()
		{
			var repository = new ValidationRepository();

			var time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(+1), DateTimeKind.Unspecified);
			var value = new DateTimeOffset(time, TimeSpan.FromHours(-1));
			var obj = new { A = value };

			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.A).FixEarlierNow();
			});

			validator.Validate(obj).Issues.Should().HaveCount(0);
			validator.Fix(obj);

			obj.A.Should().Be(value);
		}

		[TestMethod]
		public void ValidateDto_LaterNow_Test()
		{
			var repository = new ValidationRepository();

			repository.Validate(new { A = DateTime.UtcNow.ToLocalTime().AddMinutes(+1) }, t =>
			{
				t.RuleFor(x => x.A).LaterNow();
			}).Issues.Should().HaveCount(0);

			repository.Validate(new { A = DateTime.UtcNow.ToLocalTime().AddMinutes(-1) }, t =>
			{
				t.RuleFor(x => x.A).LaterNow();
			}).Issues.Should().HaveCount(1);

			repository.Validate(new { A = DateTime.UtcNow.AddMinutes(+1) }, t =>
			{
				t.RuleFor(x => x.A).LaterNow();
			}).Issues.Should().HaveCount(0);

			repository.Validate(new { A = DateTime.UtcNow.AddMinutes(-1) }, t =>
			{
				t.RuleFor(x => x.A).LaterNow();
			}).Issues.Should().HaveCount(1);

			repository.Validate(new { A = (DateTime?)DateTime.UtcNow.AddMinutes(+1) }, t =>
			{
				t.RuleFor(x => x.A).LaterNow();
			}).Issues.Should().HaveCount(0);

			var time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-0.5), DateTimeKind.Unspecified);
			repository.Validate(new { A = new DateTimeOffset(time, TimeSpan.FromHours(-1)) }, t =>
			{
				t.RuleFor(x => x.A).LaterNow();
			}).Issues.Should().HaveCount(0);

			time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-1.5), DateTimeKind.Unspecified);
			repository.Validate(new { A = new DateTimeOffset(time, TimeSpan.FromHours(-1)) }, t =>
			{
				t.RuleFor(x => x.A).LaterNow();
			}).Issues.Should().HaveCount(1);

			time = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-0.5), DateTimeKind.Unspecified);
			repository.Validate(new { A = (DateTimeOffset?)new DateTimeOffset(time, TimeSpan.FromHours(-1)) }, t =>
			{
				t.RuleFor(x => x.A).LaterNow();
			}).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void CastRule_Test()
		{
			var repository = new ValidationRepository();

			var obj = new { a = 1m };
			var validator = repository.Create(obj, t =>
			{
				t.RuleFor(x => x.a).Cast(x => x.ToString()).Fix(x => x.Replace(".", "").Replace(",", "")).Cast(x => decimal.Parse(x));
			});

			validator.Fix(new { a = 1m }).a.Should().Be(1m);
			validator.Fix(new { a = 1.1m }).a.Should().Be(11m);
		}

		[TestMethod]
		public void RuleForIf_Test()
		{
			var repository = new ValidationRepository();

			var obj = new { a = 1 };
			var validator = repository.Create(obj, t =>
			{
				t.RuleForIf(x => x.a, x => x.a == 1).Fix(x => 2);
			});

			validator.Fix(new { a = 1 }).a.Should().Be(2);
			validator.Fix(new { a = 3 }).a.Should().Be(3);
		}
	}

	enum TestEnum { Test0 = 0, Test1 = 1 }

	public class ValidationObj
	{
		public int A { get; set; }
		public string B { get; set; }
		public ValidationObj Obj { get; set; }
	}

	public class ValidationObjValidation1 : ValidationRuleFactory<ValidationObj>
	{
		public ValidationObjValidation1(ValidationRepository repository) : base(repository, null)
		{
			BaseRules();
		}

		private void BaseRules()
		{
			this.RuleFor(x => x.A).Greater(0);
			this.RuleFor(x => x.Obj.A).Greater(0);
		}
	}

	public interface IMyValidationObj
	{
		string A { get; set; }
	}

	public interface IMyValidationObjChild : IMyValidationObj
	{
		string B { get; set; }
	}
}
