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

			dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void Validate_WithBaseClass_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass0() { A = -1 };

			dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithSeveralValidators_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass1() { A = -1, B = "a" };

			var issues = dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(2);
		}

		[TestMethod]
		public void Validate_WithTrimValidation_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass1() { A = 1, B = " b " };

			obj = dto.FixValue(obj, nameof(obj), x => x.ValidateDto());
			obj.B.Should().Be("b");
		}

		[TestMethod]
		public void Validate_WithTrimValidation_FactorySignature_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopierContainer0), typeof(NestedCopierContainer1));

			var obj = new CopyClass0() { A = -1 };

			var issues = dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
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
		public void Copy_NestedWithGenerics_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenerics));

			var src = new CopyClass_MyTemplateItem() { A = new MyTemplateItem() { A = 1 } };
			var dst = new CopyClass0();

			var res = dto.Copy(dst, src);
			res.A.Should().Be(1);
		}

		[TestMethod]
		public void Copy_NestedWithGenerics_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenerics));

			var src = new CopyClass0() { A = 1 };
			var dst = new CopyClass_MyTemplateItem();

			var res = dto.Copy(dst, src);
			res.A.A.Should().Be(1);
		}

		[TestMethod]
		public void Update_NestedWithGenerics_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenerics));

			var src = new CopyClass0() { A = 1 };
			var dst = new CopyClass_MyTemplateItem();

			var res = dto.Update(dst, src, nameof(src.A));

			res.A.A.Should().Be(1);
		}

		[TestMethod]
		public void Update_NestedWithGenerics_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenerics));

			var src = new CopyClass0() { A = 1 };
			var dst = new CopyClass_MyTemplateItem();

			var res = dto.Update(dst, src);

			res.A.Should().BeNull();
		}

		[TestMethod]
		public void Copy_NestedWithGenerics_WithIgnore_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenerics_WithIgnore));

			var src = new CopyClass0() { A = 1 };
			var dst = new CopyClass_MyTemplateItem();

			var res = dto.Copy(dst, src);

			res.A.Should().BeNull();
		}

		[TestMethod]
		public void Copy_NestedWithGenerics_WithIgnore_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenerics_WithIgnore1));

			var src = new NestedWithGenerics_WithIgnore1.CopyClass0() { A = 1 };
			var dst = new NestedWithGenerics_WithIgnore1.CopyClass_MyTemplateItem();

			var res = dto.Copy(dst, src);

			res.A.Should().BeNull();
		}

		[TestMethod]
		public void Validate_NestedWithGenerics_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenerics));

			var obj = new CopyClass_MyTemplateItem();

			var issues = dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Copy_NestedWithGenericsTypedCast_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenericsTypedCast));

			var src = new NestedWithGenericsTypedCast.CopyClass0_string() { A = "1" };
			var dst = new NestedWithGenericsTypedCast.CopyClass0_int();

			var res = dto.Copy(dst, src);
			res.A.Should().Be(1);
		}

		[TestMethod]
		public void Copy_NestedWithGenericsTypedCast_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenericsTypedCast));

			var src = new NestedWithGenericsTypedCast.CopyClass1_string() { A = "1" };
			var dst = new NestedWithGenericsTypedCast.CopyClass1_int();

			AssertionExtensions.Should(() => dto.Copy(dst, src)).Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void Convert_NestedWithConvert_WithGeneric_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric));

			var src = new NestedWithConvert_WithGeneric.CopyClass_Decimal() { A = 1.1m };
			var dst = new NestedWithConvert_WithGeneric.CopyClass_Int();

			dto.Copy(dst, src);
			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void Convert_NestedWithConvert_WithGeneric_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric));

			var src = new NestedWithConvert_WithGeneric.CopyClass_Int() { A = 1 };
			var dst = new NestedWithConvert_WithGeneric.CopyClass_Int1();

			dto.Copy(dst, src);
			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void Update_Convert_NestedWithConvert_WithGeneric_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric));

			var src = new NestedWithConvert_WithGeneric.CopyClass_Decimal() { A = 1.1m };
			var dst = new NestedWithConvert_WithGeneric.CopyClass_Int();

			dto.Update(dst, src, nameof(src.A));
			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void Update_Convert_NestedWithConvert_WithGeneric_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric));

			var src = new NestedWithConvert_WithGeneric.CopyClass_Decimal() { A = 1.1m };
			var dst = new NestedWithConvert_WithGeneric.CopyClass_Int();

			dto.Update(dst, src);
			dst.A.Should().Be(0);
		}

		[TestMethod]
		public void Convert_NestedWithConvert_WithPartialGeneric_Test_1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithPartialGeneric_DstOnly));

			var dst = new NestedWithConvert_WithPartialGeneric_DstOnly.CopyClass_Decimal();
			var src = new NestedWithConvert_WithPartialGeneric_DstOnly.CopyClass_Int() { A = 1 };

			dto.Copy(dst, src);
			dst.A.Should().Be(1m);
		}

		[TestMethod]
		public void Convert_NestedWithConvert_WithPartialGeneric_Test_2()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithPartialGeneric_SrcOnly));

			var dst = new NestedWithConvert_WithPartialGeneric_SrcOnly.CopyClass_Decimal();
			var src = new NestedWithConvert_WithPartialGeneric_SrcOnly.CopyClass_Int() { A = 1 };

			dto.Copy(dst, src);
			dst.A.Should().Be(1m);
		}

		[TestMethod]
		public void NestedWithConvert_WithGeneric_WithFilters_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric_WithFilters));

			var src = new NestedWithConvert_WithGeneric_WithFilters.CopyClass_Decimal() { A = 1.1m };
			var dst = new NestedWithConvert_WithGeneric_WithFilters.CopyClass_Int();

			dto.Copy(dst, src);
			dst.A.Should().Be(0);
		}

		[TestMethod]
		public void Convert_NestedWithConvert_WithGeneric_WithFilters_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric_WithFilters));

			var src = new NestedWithConvert_WithGeneric_WithFilters.CopyClass_Int() { A = 1 };
			var dst = new NestedWithConvert_WithGeneric_WithFilters.CopyClass_Decimal();

			dto.Copy(dst, src);
			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void NestedWithConvert_WithGeneric_WithParentFilters_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric_WithParentFilters));

			var src = new NestedWithConvert_WithGeneric_WithParentFilters.CopyClass_Decimal() { A = 1.1m };
			var dst = new NestedWithConvert_WithGeneric_WithParentFilters.CopyClass_ChildItem();

			dto.Copy(dst, src);
			dst.A.Name.Should().Be(1.1m);
		}

		[TestMethod]
		public void NestedWithConvert_WithGeneric_WithParentFilters_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric_WithParentFilters));

			var src = new NestedWithConvert_WithGeneric_WithParentFilters.CopyClass_Decimal() { A = 1.1m };
			var dst = new NestedWithConvert_WithGeneric_WithParentFilters.CopyClass_OtherItem();

			dto.Copy(dst, src);
			dst.A.Should().BeNull();
		}

		[TestMethod]
		public void NestedWithConvert_WithGeneric_WithInterfaceFilters_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric_WithInterfaceFilters));

			var src = new NestedWithConvert_WithGeneric_WithInterfaceFilters.CopyClass_Decimal() { A = 1.1m };
			var dst = new NestedWithConvert_WithGeneric_WithInterfaceFilters.CopyClass_ChildItem();

			dto.Copy(dst, src);
			dst.A.Name.Should().Be(1.1m);
		}

		[TestMethod]
		public void NestedWithConvert_WithGeneric_WithInterfaceFilters_Test1()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithConvert_WithGeneric_WithInterfaceFilters));

			var src = new NestedWithConvert_WithGeneric_WithInterfaceFilters.CopyClass_Decimal() { A = 1.1m };
			var dst = new NestedWithConvert_WithGeneric_WithInterfaceFilters.CopyClass_OtherItem();

			dto.Copy(dst, src);
			dst.A.Should().BeNull();
		}

		[TestMethod]
		public void ClassConvert_WithOtherMethod_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(ClassConvert_WithOtherMethod));

			var src = new CopyClass0() { A = 1 };
			var dst = new CopyClass1();

			dto.Copy(dst, src);
			dst.B.Should().Be("1");
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

			var issues = dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithoutClasses_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithoutClasses));

			var obj = new CopyClass0_WithInterf() { A = -1 };

			dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithStrongType_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopier_StrongValidatorType));

			var obj = new CopyClass0();
			var issues = dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithStrongType_ChildClass_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopier_StrongValidatorType));

			var obj = new CopyClass0_WithInterf();
			var issues = dto.Validate(obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void Validate_WithoutType_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedCopier_StrongValidatorType));

			var obj = new CopyClass0_WithInterf();
			var issues = dto.Validate((object)obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(1);
		}

		[TestMethod]
		public void BaseClassValidation_Validate_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(BaseClassValidation));

			var obj = new BaseClassValidationChild() { A = -1 };
			var issues = dto.Validate((object)obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(1);

			obj = new BaseClassValidationChild() { A = 1 };
			issues = dto.Validate((object)obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(0);
		}

		[TestMethod]
		public void ClassValidation_Validate_WithOtherMethod_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(ClassValidation_WithOtherMethods));

			var obj = new BaseClassValidationChild() { A = -1 };
			var issues = dto.Validate((object)obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(1);

			obj = new BaseClassValidationChild() { A = 1 };
			issues = dto.Validate((object)obj, x => x.RuleFor(t => t).ValidateDto()).Issues;
			issues.Should().HaveCount(0);
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

		[TestMethod]
		public void CastValues_Test()
		{
			var dto = new DtoComplex().ByNestedClasses(typeof(NestedWithGenerics));

			var src = new MyTemplateItem() { A = 1 };

			dto.CastValue(src, out int dst);

			dst.Should().Be(1);
		}

		[TestMethod]
		public void CastValues_DefaultCast_Test()
		{
			var dto = new DtoComplex();

			dto.CastValue(1.5m, out int dst);

			dst.Should().Be(1);
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

	public interface ITemplateInterf1<T>
	{
		T A { get; set; }
	}

	public class CopyClass0_WithInterf : CopyClass0, ICopyClass0 { }

	public class CopyClass1_WithInterf : CopyClass1, ICopyClass0, ICopyClass1 { }


	[Dto]
	[DtoContainer]
	public class NestedCopierContainer2
	{
		public class CopyClass0Ext : CopyClass0, ICopyClass0 { }

	}

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

	public class ClassConvert_WithOtherMethod
	{
		class CopyClass0Ext : CopyClass0, ICopyClass0 { }
		class CopyClass1Ext : CopyClass1, ICopyClass1 { }

		[DtoConvert]
		static void Convert(ICopyClass1 dst, ICopyClass0 src)
		{
			OtherMethod(dst, src);
		}

		private static void OtherMethod(ICopyClass1 dst, ICopyClass0 src)
		{
			dst.B = src.A.ToString();
		}
	}

	public class NestedWithConvert_WithGeneric
	{
		public class CopyClass_Decimal
		{
			public decimal A { get; set; }
		}

		public class CopyClass_Int
		{
			public int A { get; set; }
		}

		public class CopyClass_Int1
		{
			public int A { get; set; }
		}

		[DtoNonCopy]
		public interface ITemplateInterf1<T>
		{
			T A { get; set; }
		}

		class CopyClass0Ext : CopyClass_Decimal, ITemplateInterf1<decimal> { }
		class CopyClass1Ext : CopyClass_Int, ITemplateInterf1<int> { }
		class CopyClass2Ext : CopyClass_Int1, ITemplateInterf1<int> { }

		[DtoConvert]
		static void Convert<TDst, TSrc>(ITemplateInterf1<TDst> dst, ITemplateInterf1<TSrc> src, DtoComplex dto)
		{
			dst.A = dto.CastValue(src.A, out TDst t);
		}
	}

	public class NestedWithConvert_WithGeneric_WithFilters
	{
		public class CopyClass_Decimal
		{
			public decimal A { get; set; }
		}

		public class CopyClass_Int
		{
			public int A { get; set; }
		}

		public class CopyClass_Int1
		{
			public int A { get; set; }
		}

		[DtoNonCopy]
		public interface ITemplateInterf1<T>
		{
			T A { get; set; }
		}

		class CopyClass0Ext : CopyClass_Decimal, ITemplateInterf1<decimal> { }
		class CopyClass1Ext : CopyClass_Int, ITemplateInterf1<int> { }
		class CopyClass2Ext : CopyClass_Int1, ITemplateInterf1<int> { }

		[DtoConvertFilter(typeof(decimal), typeof(int))]
		[DtoConvert]
		static void Convert<TDst, TSrc>(ITemplateInterf1<TDst> dst, ITemplateInterf1<TSrc> src, DtoComplex dto)
		{
			dst.A = dto.CastValue(src.A, out TDst t);
		}
	}

	public class NestedWithConvert_WithGeneric_WithParentFilters
	{
		public class Item { public decimal Name { get; set; } }
		public class ChildItem : Item { }
		public class OtherItem { public decimal Name { get; set; } }

		public class CopyClass_Decimal
		{
			public decimal A { get; set; }
		}

		public class CopyClass_Item
		{
			public Item A { get; set; }
		}

		public class CopyClass_ChildItem
		{
			public ChildItem A { get; set; }
		}

		public class CopyClass_OtherItem
		{
			public OtherItem A { get; set; }
		}

		[DtoNonCopy]
		public interface ITemplateInterf1<T>
		{
			T A { get; set; }
		}

		class CopyClass0Ext : CopyClass_Decimal, ITemplateInterf1<decimal> { }
		class CopyClass2Ext : CopyClass_ChildItem, ITemplateInterf1<ChildItem> { }
		class CopyClass3Ext : CopyClass_OtherItem, ITemplateInterf1<OtherItem> { }

		[DtoConvertFilter(typeof(Item))]
		[DtoConvert]
		static void Convert<TDst>(ITemplateInterf1<TDst> dst, ITemplateInterf1<decimal> src, DtoComplex dto)
			where TDst : Item, new()
		{
			dst.A = new TDst();
			dst.A.Name = src.A;
		}
	}

	public class NestedWithConvert_WithGeneric_WithInterfaceFilters
	{
		public interface IName { decimal Name { get; set; } }

		public class Item { public decimal Name { get; set; } }
		public class ChildItem { public decimal Name { get; set; } }
		public class OtherItem { public decimal Name { get; set; } }

		public class CopyClass_Decimal
		{
			public decimal A { get; set; }
		}

		public class CopyClass_Item
		{
			public Item A { get; set; }
		}

		public class CopyClass_ChildItem
		{
			public ChildItem A { get; set; }
		}

		public class CopyClass_OtherItem
		{
			public OtherItem A { get; set; }
		}

		[DtoNonCopy]
		public interface ITemplateInterf1<T>
		{
			T A { get; set; }
		}

		class Item0Ext : Item, IName { }
		class Item1Ext : ChildItem, IName { }

		class CopyClass0Ext : CopyClass_Decimal, ITemplateInterf1<decimal> { }
		class CopyClass2Ext : CopyClass_ChildItem, ITemplateInterf1<ChildItem> { }
		class CopyClass3Ext : CopyClass_OtherItem, ITemplateInterf1<OtherItem> { }

		[DtoConvertFilter(typeof(IName))]
		[DtoConvert]
		static void Convert<TDst>(ITemplateInterf1<TDst> dst, ITemplateInterf1<decimal> src, DtoComplex dto)
			where TDst : new()
		{
			dst.A = new TDst();
			var name = dto.As<IName>(dst.A);
			name.Name = src.A;
		}
	}

	public class NestedWithConvert_WithPartialGeneric_DstOnly
	{
		public class CopyClass_Decimal
		{
			public decimal A { get; set; }
		}

		public class CopyClass_Int
		{
			public int A { get; set; }
		}

		public class CopyClass_Int1
		{
			public int A { get; set; }
		}

		[DtoNonCopy]
		public interface ITemplateInterf1<T>
		{
			T A { get; set; }
		}

		class CopyClass0Ext : CopyClass_Decimal, ITemplateInterf1<decimal> { }
		class CopyClass1Ext : CopyClass_Int, ITemplateInterf1<int> { }
		class CopyClass2Ext : CopyClass_Int1, ITemplateInterf1<int> { }

		[DtoConvert]
		static void Convert<TDst>(ITemplateInterf1<TDst> dst, ITemplateInterf1<int> src, DtoComplex dto)
		{
			dst.A = (TDst)(object)(decimal)src.A;
		}
	}

	public class NestedWithConvert_WithPartialGeneric_SrcOnly
	{
		public class CopyClass_Decimal
		{
			public decimal A { get; set; }
		}

		public class CopyClass_Int
		{
			public int A { get; set; }
		}

		public class CopyClass_Int1
		{
			public int A { get; set; }
		}

		[DtoNonCopy]
		public interface ITemplateInterf1<T>
		{
			T A { get; set; }
		}

		class CopyClass0Ext : CopyClass_Decimal, ITemplateInterf1<decimal> { }
		class CopyClass1Ext : CopyClass_Int, ITemplateInterf1<int> { }
		class CopyClass2Ext : CopyClass_Int1, ITemplateInterf1<int> { }

		[DtoConvert]
		static void Convert<TSrc>(ITemplateInterf1<decimal> dst, ITemplateInterf1<TSrc> src, DtoComplex dto)
		{
			dst.A = (decimal)(int)(object)src.A;
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

	public class MyTemplateItem
	{
		public int A { get; set; }
	}

	public class CopyClass_MyTemplateItem
	{
		public MyTemplateItem A { get; set; }
	}

	public class CopyClass_string
	{
		public string A { get; set; }
	}

	public class NestedWithGenerics
	{
		public interface ITemplateInterf<T>
		{
			T A { get; set; }
		}

		class CopyClass0Ext : CopyClass0, ITemplateInterf<int> { }
		class CopyClass1Ext : CopyClass_MyTemplateItem, ITemplateInterf<MyTemplateItem> { }

		[DtoCast]
		static int Cast(MyTemplateItem src)
		{
			return src.A;
		}

		[DtoCast]
		static MyTemplateItem Cast(int src)
		{
			return new MyTemplateItem() { A = src };
		}

		[DtoValidate]
		static void Validate<T>(ValidationRuleFactory<ITemplateInterf<T>> t)
		{
			t.RuleFor(x => x.A).NotEmpty();
		}
	}

	public class NestedWithGenerics_WithIgnore
	{
		[DtoNonCopy]
		public interface ITemplateInterf<T>
		{
			T A { get; set; }
		}

		class CopyClass0Ext : CopyClass0, ITemplateInterf<int> { }
		class CopyClass1Ext : CopyClass_MyTemplateItem, ITemplateInterf<MyTemplateItem> { }

		[DtoCast]
		static int Cast(MyTemplateItem src)
		{
			return src.A;
		}

		[DtoCast]
		static MyTemplateItem Cast(int src)
		{
			return new MyTemplateItem() { A = src };
		}
	}

	public class NestedWithGenerics_WithIgnore1
	{
		[DtoNonCopy]
		public interface ITemplateInterf<T>
		{
			T A { get; set; }
		}

		public class CopyClass0 : ITemplateInterf<int>
		{
			public int A { get; set; }
		}

		public class CopyClass_MyTemplateItem : ITemplateInterf<MyTemplateItem>
		{
			public MyTemplateItem A { get; set; }
		}

		[DtoCast]
		static int Cast(MyTemplateItem src)
		{
			return src.A;
		}

		[DtoCast]
		static MyTemplateItem Cast(int src)
		{
			return new MyTemplateItem() { A = src };
		}
	}

	public class NestedWithGenericsTypedCast
	{
		public interface ITemplateInterf<T>
		{
			T A { get; set; }
		}

		public class CopyClass0_int
		{
			public int A { get; set; }
		}

		public class CopyClass0_string
		{
			public string A { get; set; }
		}

		public class CopyClass1_int
		{
			public int A { get; set; }
		}

		public class CopyClass1_string
		{
			public string A { get; set; }
		}

		class CopyClass0_int_dto : CopyClass0_int, ITemplateInterf<int> { }
		class CopyClass0_string_dto : CopyClass0_string, ITemplateInterf<string> { }

		class CopyClass1_int_dto : CopyClass1_int, ITemplateInterf1<int> { }
		class CopyClass1_string_dto : CopyClass1_string, ITemplateInterf1<string> { }

		[DtoCast(typeof(ITemplateInterf<>))]
		static int Cast(string src)
		{
			return int.Parse(src);
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

	public class BaseClassValidationParent
	{
		public int A { get; set; }
	}

	public class BaseClassValidationChild : BaseClassValidationParent
	{
		public int A { get; set; }
	}

	public class BaseClassValidation
	{
		public interface IInterf
		{
			int A { get; set; }
		}

		[DtoValidate]
		static void Validate(ValidationRuleFactory<IInterf> fact)
		{
			fact.RuleFor(x => x.A).Greater(0);
		}

		class MyA : BaseClassValidationParent, IInterf { }
	}

	public class ClassValidation_WithOtherMethods
	{
		public interface IInterf
		{
			int A { get; set; }
		}

		[DtoValidate]
		static void Validate(ValidationRuleFactory<IInterf> fact)
		{
			OtherMethod(fact);
		}

		static void OtherMethod(ValidationRuleFactory<IInterf> fact)
		{
			fact.RuleFor(x => x.A).Greater(0);
		}

		class MyA : BaseClassValidationParent, IInterf { }
	}
}
