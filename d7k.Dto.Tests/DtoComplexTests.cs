using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto.Tests
{
	/// <summary>
	/// Summary description for NestedDtoCopierTests
	/// </summary>
	[TestClass]
	public class DtoComplexTests
	{
		#region Additional test attributes

		//public NestedDtoCopierTests()
		//{
		//	//
		//	// TODO: Add constructor logic here
		//	//
		//}
		//
		//private TestContext testContextInstance;
		//
		///// <summary>
		/////Gets or sets the test context which provides
		/////information about and functionality for the current test run.
		/////</summary>
		//public TestContext TestContext
		//{
		//	get
		//	{
		//		return testContextInstance;
		//	}
		//	set
		//	{
		//		testContextInstance = value;
		//	}
		//}
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		// [TestInitialize()]
		// public void MyTestInitialize() { }
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		[TestMethod]
		public void Copy_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0));

			var it = new CopyClass0();
			var res = dto.Copy(it, new CopyClass0() { A = 2 });

			res.Should().BeSameAs(it);
			res.A.Should().Be(2);
		}

		[TestMethod]
		public void ByNestedClassesWithAttributes_Test()
		{
			var dto = new DtoComplex().ByNestedClassesWithAttributes(new[] { typeof(DtoAttribute), typeof(Dto1Attribute) });

			var res = dto.Copy(new CopyClass1(), new CopyClass0() { A = 2 });

			res.A.Should().Be(2);
			res.B.Should().BeNull();
		}

		[TestMethod]
		public void ByNestedClassesWithDefaultContainerAttribute_Test()
		{
			//NestedCopierContainer0 and NestedCopierContainer1 marked DtoContainer
			var dto = new DtoComplex().ByNestedClassesWithAttributes();

			var res = dto.Copy(new CopyClass1(), new CopyClass0() { A = 2 });

			res.A.Should().Be(2);
			res.B.Should().BeNull();
		}

		[TestMethod]
		public void Copy_ToComplexClass_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var res = dto.Copy(new CopyClass1(), new CopyClass0() { A = 2 });

			res.A.Should().Be(2);
			res.B.Should().BeNull();
		}

		[TestMethod]
		public void Copy_FromComplexClass_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var res = dto.Copy(new CopyClass0(), new CopyClass1() { A = 2, B = "abc" });
			res.A.Should().Be(2);
		}

		[TestMethod]
		public void Copy_WithTransformFunction_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert));

			var dst = new CopyClass1();
			var res = dto.Copy(dst, new CopyClass0() { A = 2 });

			res.B.Should().Be("2");
		}

		[TestMethod]
		public void Copy_WithTransformFunction_ForExactMatch_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvertWithExactMatch));

			var dst = new CopyClass1();
			var res = dto.Copy(dst, new CopyClass0() { A = 2 });

			res.B.Should().Be("2");
		}

		[TestMethod]
		public void Update_ComplexClass_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var dst = new CopyClass1();
			var res = dto.Update(dst, new CopyClass1() { A = 2, B = "abc" }, nameof(CopyClass1.A));

			res.Should().BeSameAs(dst);
			res.A.Should().Be(2);
			res.B.Should().BeNull();
		}

		[TestMethod]
		public void Update_WithTransformFunction_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert));

			var dst = new CopyClass1();
			var res = dto.Update(dst, new CopyClass0() { A = 2 }, nameof(CopyClass1.B));

			res.B.Should().Be("2");
		}

		[TestMethod]
		public void Update_WithTransformFunction_ForExactMatch_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvertWithExactMatch));

			var dst = new CopyClass1();
			var res = dto.Update(dst, new CopyClass0() { A = 2 }, nameof(CopyClass1.B));

			res.B.Should().Be("2");
		}

		[TestMethod]
		public void Update_WithTransformFunction_IgnoreField_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert));

			var dst = new CopyClass1();
			var res = dto.Update(dst, new CopyClass0() { A = 2 });

			res.B.Should().BeNull();
		}

		[TestMethod]
		public void Validate_WithBaseClass_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass0() { A = 1 };

			new ValidationRepository().Validate(obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void Validate_WithBaseClass_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass0() { A = -1 };

			new ValidationRepository().Validate(obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithSeveralValidators_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass1() { A = -1, B = "a" };

			var issues = new ValidationRepository().Validate(obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues;
			issues.Should().HaveCount(2);
		}

		[TestMethod]
		public void Validate_WithTrimValidation_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass1() { A = 1, B = " b " };

			obj = new ValidationRepository().FixValue(obj, nameof(obj), x => x.ValidateDto(dto));
			obj.B.Should().Be("b");
		}

		[TestMethod]
		public void Validate_WithTrimValidation_FactorySignature_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass0() { A = -1 };

			var issues = new ValidationRepository().Validate(obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Copy_FromComplexClass_NotPublicDescription_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedNotPublicCopierContainer0), typeof(NestedCopierContainer1));

			var res = dto.Copy(new CopyClass0(), new CopyClass1() { A = 2, B = "abc" });
			res.A.Should().Be(2);
		}

		[TestMethod]
		public void Copy_FromUnregistredTypes_Test()
		{
			var dto = new DtoComplex();

			var res = dto.Copy(new CopyClass0_WithInterf(), new CopyClass1_WithInterf() { A = 2, B = "abc" });
			res.A.Should().Be(2);
		}

		[TestMethod]
		public void Copy_FromMultidescription_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopier_MultiDescriptionContainer0), typeof(NestedCopier_MultiDescriptionContainer1));

			var obj = new CopyClass1() { A = 1, B = "abc" };

			var res = dto.Copy(new CopyClass1_Obther(), obj);
			res.A.Should().Be(1);
			res.B.Should().Be("abc");
		}

		[TestMethod]
		public void Copy_FromSameClass_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopier_MultiDescriptionContainer0));

			var obj = new CopyClass1() { A = 1, B = "abc" };

			var res = dto.Copy(new CopyClass1(), obj);
			res.A.Should().Be(1);
			res.B.Should().Be("abc");
		}

		[TestMethod]
		public void Copy_FromAbstractObject_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithAbstractConverter));

			var obj = new CopyClass0() { A = 1 };

			var res = dto.Copy(new CopyClass1(), new[] { obj });
			res.B.Should().Be("1");
		}

		[TestMethod]
		public void Copy_ToAbstractObject_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithAbstractConverter));

			var obj = new CopyClass0() { A = 1 };

			var res = dto.Copy(new List<string>(), new CopyClass1() { B = "1" });
			res.Single().Should().Be("1");
		}

		[TestMethod]
		public void Copy_SubClasses_Test()
		{
			var dto = new DtoComplex();
			var obj = new Sub0CopyClass0() { A = 1, B = "2" };

			var res = dto.Copy(new Sub1CopyClass0(), obj);
			res.A.Should().Be(1);
			res.B.Should().BeNull();
		}

		[TestMethod]
		public void Validate_NotPublicDescription_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass0() { A = -1 };

			var issues = new ValidationRepository().Validate(obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithoutClasses_Test()
		{
			var validation = new ValidationRepository();

			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithoutClasses));

			var obj = new CopyClass0_WithInterf() { A = -1 };

			validation.Validate(obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithStrongType_Test()
		{
			var validation = new ValidationRepository();

			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopier_StrongValidatorType));

			var obj = new CopyClass0();
			var issues = validation.Validate(obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithStrongType_ChildClass_Test()
		{
			var validation = new ValidationRepository();

			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopier_StrongValidatorType));

			var obj = new CopyClass0_WithInterf();
			var issues = validation.Validate(obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithoutType_Test()
		{
			var validation = new ValidationRepository();

			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopier_StrongValidatorType));

			var obj = new CopyClass0_WithInterf();
			var issues = validation.Validate((object)obj, x => x.RuleFor(t => t).ValidateDto(dto)).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void As_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedForAs));

			var obj = new CopyClass1() { A = 1 };

			var t0 = dto.As<ICopyClass0>(obj);

			t0.Should().NotBeNull();
			t0.A.Should().Be(1);

			var t1 = dto.As<ICopyClass1>(obj);
			t1.Should().BeNull();
		}

		[TestMethod]
		public void As_WithUnknown_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedForAs));

			var obj = new CopyClass0() { A = 1 };

			var t0 = dto.As<ICopyClass0>(obj);
			t0.Should().BeNull();
		}

		[TestMethod]
		public void As_WithNull_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedForAs));

			AssertionExtensions.Should(() => dto.As<ICopyClass0>(null)).Throw<NullReferenceException>();
		}

		[TestMethod]
		public void As_ForOtherAdapter_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer1));

			var obj = new CopyClass1() { A = 1, B = "abc" };

			var adapter0 = dto.As<ICopyClass0>(obj);
			adapter0.A.Should().Be(1);

			dto.As<ICopyClass1>(adapter0).B.Should().Be("abc");
		}

		[TestMethod]
		public void CopyAdaptedClasses_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer1));

			var obj0 = new CopyClass1() { A = 1, B = "abc" };
			var obj1 = new CopyClass1() { A = 2, B = "def" };

			var adapter0 = dto.As<ICopyClass0>(obj0);
			var adapter1 = dto.As<ICopyClass1>(obj1);

			dto.Copy(adapter0, adapter1);

			obj0.A.Should().Be(2);
			obj0.B.Should().Be("def");
		}

		[TestMethod]
		public void UpdateAdaptedClasses_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer1));

			var obj0 = new CopyClass1() { A = 1, B = "abc" };
			var obj1 = new CopyClass1() { A = 2, B = "def" };

			var adapter0 = dto.As<ICopyClass0>(obj0);
			var adapter1 = dto.As<ICopyClass1>(obj1);

			dto.Update(adapter0, adapter1, nameof(obj0.A), nameof(obj0.B));

			obj0.A.Should().Be(2);
			obj0.B.Should().Be("def");
		}

		[TestMethod]
		public void InvalidSignatures_Test()
		{
			AssertionExtensions.Should(() => new DtoComplex().ByNestedClasses(typeof(InvalidSignatures)))
				.Throw<InvalidOperationException>()
				.Where(x => x.Message.Contains(typeof(InvalidSignatures).FullName));
		}
	}

	public class CopyClass0
	{
		public int A { get; set; }
	}

	public class CopyClass1
	{
		public int A { get; set; }
		public string B { get; set; }
	}

	public class Sub0CopyClass0 : CopyClass0
	{
		public string B { get; set; }
	}

	public class Sub1CopyClass0 : CopyClass0
	{
		public string B { get; set; }
	}

	public class CopyClass1_Obther
	{
		public int A { get; set; }
		public string B { get; set; }
	}

	public interface ICopyClass0
	{
		int A { get; set; }
	}

	public interface ICopyClass1
	{
		string B { get; set; }
	}

	public class CopyClass0_WithInterf : CopyClass0, ICopyClass0 { }

	public class CopyClass1_WithInterf : CopyClass1, ICopyClass0, ICopyClass1 { }

	[Dto]
	[DtoContainer]
	public static class NestedCopierContainer0
	{
		public class CopyClass0Ext : CopyClass0, ICopyClass0 { }

		[DtoValidate]
		public static void Validate(ValidationRuleFactory<ICopyClass0> t)
		{
			t.RuleFor(x => x.A).Greater(0);
		}

		[DtoValidate]
		public static void Validate(ValidationRuleFactory<ICopyClass1> t)
		{
			t.RuleFor(x => x.B).Trim().Forbidden("a");
		}
	}

	[Dto1]
	[DtoContainer]
	public static class NestedCopierContainer1
	{
		public class CopyClass2Ext : CopyClass1, ICopyClass0, ICopyClass1 { }
	}

	public class NestedNotPublicCopierContainer0
	{
		class CopyClass0Ext : CopyClass0, ICopyClass0 { }

		[DtoValidate]
		static void Validate(ValidationRuleFactory<ICopyClass0> t)
		{
			t.RuleFor(x => x.A).Greater(0);
		}

		[DtoValidate]
		static void Validate(ValidationRuleFactory<ICopyClass1> t)
		{
			t.RuleFor(x => x.B).Trim().Forbidden("a");
		}
	}

	public class NestedWithConvert
	{
		class CopyClass0Ext : CopyClass0, ICopyClass0 { }
		class CopyClass1Ext : CopyClass1, ICopyClass1 { }

		[DtoConvert]
		static void Convert(ICopyClass1 dst, ICopyClass0 src)
		{
			dst.B = src.A.ToString();
		}
	}

	public class NestedWithConvertWithExactMatch
	{
		class CopyClass1Ext : CopyClass1, ICopyClass1 { }

		[DtoConvert]
		static void Convert(ICopyClass1 dst, CopyClass0 src)
		{
			dst.B = src.A.ToString();
		}
	}

	public class NestedWithAbstractConverter
	{
		class CopyClass0Ext : CopyClass0, ICopyClass0 { }
		class CopyClass1Ext : CopyClass1, ICopyClass1 { }

		[DtoConvert]
		static void Convert(ICopyClass1 dst, IEnumerable<CopyClass0> src)
		{
			dst.B = src.First().A.ToString();
		}

		[DtoConvert]
		static void Convert(List<string> dst, ICopyClass1 src)
		{
			dst.Add(src.B);
		}
	}

	public class NestedForAs
	{
		class CopyClass1Ext : CopyClass1, ICopyClass0 { }

		[DtoValidate]
		static void Validate(ValidationRuleFactory<ICopyClass0> t)
		{
			t.RuleFor(x => x.A).Greater(0);
		}

		[DtoValidate]
		static void Validate(ValidationRuleFactory<ICopyClass1> t)
		{
			t.RuleFor(x => x.B).Trim().Forbidden("a");
		}
	}

	public class NestedWithoutClasses
	{
		[DtoValidate]
		static void Validate(ValidationRuleFactory<ICopyClass0> t)
		{
			t.RuleFor(x => x.A).Greater(0);
		}

		[DtoValidate]
		static void Validate(ValidationRuleFactory<ICopyClass1> t)
		{
			t.RuleFor(x => x.B).Trim().Forbidden("a");
		}
	}

	public class NestedCopier_MultiDescriptionContainer0
	{
		public class CopyClass0Ext : CopyClass1, ICopyClass0 { }
		public class CopyClass1_ObjerExt : CopyClass1_Obther, ICopyClass0, ICopyClass1 { }
	}

	public class NestedCopier_MultiDescriptionContainer1
	{
		public class CopyClass0Ext : CopyClass1, ICopyClass1 { }
	}

	public class NestedCopier_StrongValidatorType
	{
		[DtoValidate]
		static void Validate(ValidationRuleFactory<CopyClass0> t)
		{
			t.RuleFor(x => x.A).Greater(0);
		}
	}

	public class InvalidSignatures
	{
		[DtoValidate]
		static void Validate(ValidationRuleFactory<List<int>> t, int b, string c)
		{
		}
	}

	public class DtoAttribute : Attribute { }
	public class Dto1Attribute : Attribute { }
}
