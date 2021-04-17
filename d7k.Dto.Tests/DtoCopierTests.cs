using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace d7k.Dto.Tests
{
	[TestClass]
	public class DtoCopierTests
	{
		#region Initialize

		static DtoCopier s_copier = new DtoCopier();

		#endregion

		[TestMethod]
		public void CopyInterf_Test()
		{
			var src = new CopyInterfInfo() { A = 1, Other = "other" };
			var dst = new CopyInterfInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfo), false, null);

			dst.A.Should().Be(1);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void CopyObj_Test()
		{
			var src = new CopyObjInfo() { A = 1, Other = "other" };
			var dst = new CopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfo), false, null);

			dst.A.Should().Be(1);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void CopyObjToClass_Test()
		{
			var src = new CopyObjInfo() { A = 1, Other = "other" };
			var dst = new CopyInterfInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfo), false, null);

			dst.A.Should().Be(1);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void CopyClassToObj_Test()
		{
			var src = new CopyInterfInfo() { A = 1, Other = "other" };
			var dst = new CopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfo), false, null);

			dst.A.Should().Be(1);
			dst.Other.Should().BeNull();
		}


		[TestMethod]
		public void CopyInterf_WithInheritance_Test()
		{
			var src = new InheritanceCopyInterfInfo() { A = 1, B = 2, C = 3, Other = "other" };
			var dst = new InheritanceCopyInterfInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfoChild), false, null);

			dst.A.Should().Be(1);
			dst.B.Should().Be(2);
			dst.C.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void CopyObj_WithInheritance_Test()
		{
			var src = new InheritanceCopyObjInfo() { A = 1, B = 2, C = 3, Other = "other" };
			var dst = new InheritanceCopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfoChild), false, null);

			dst.A.Should().Be(1);
			dst.B.Should().Be(2);
			dst.C.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void CopyInterfToObj_WithInheritance_Test()
		{
			var src = new InheritanceCopyInterfInfo() { A = 1, B = 2, C = 3, Other = "other" };
			var dst = new InheritanceCopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfoChild), false, null);

			dst.A.Should().Be(1);
			dst.B.Should().Be(2);
			dst.C.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void CopyObjToInterf_WithInheritance_Test()
		{
			var src = new InheritanceCopyObjInfo() { A = 1, B = 2, C = 3, Other = "other" };
			var dst = new InheritanceCopyInterfInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfoChild), false, null);

			dst.A.Should().Be(1);
			dst.B.Should().Be(2);
			dst.C.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void CopyObjToObj_WithInheritance_Test()
		{
			var src = new InheritanceCopyObjInfo() { A = 1, B = 2, C = 3, Other = "other" };
			var dst = new InheritanceCopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(InheritanceCopyObjInfo), false, null);

			dst.A.Should().Be(1);
			dst.B.Should().Be(2);
			dst.C.Should().Be(3);
			dst.Other.Should().Be("other");
		}

		[TestMethod]
		public void TmplCopy_Test()
		{
			var src = new TmplCopy<int>() { A = 1 };
			var dst = new TmplCopy<int?>();

			new DtoCopier().Copy(dst, typeof(TmplCopy<int>), src, typeof(TmplCopy<int?>), false, null);

			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void TmplCopy_With_ComplexProperty_Test()
		{
			var src = new TmplCopy<int>() { A = 1 };
			var dst = new TmplCopy<MyItem>();

			new DtoCopier().Copy(dst, typeof(TmplCopy<MyItem>), src, typeof(TmplCopy<int>), false, null);

			dst.A.A.Should().Be(1);
		}

		[TestMethod]
		public void TmplCopy_With_ComplexProperty_Test1()
		{
			var src = new TmplCopy<MyItem>() { A = new MyItem() { A = 1 } };
			var dst = new TmplCopy<int>();

			new DtoCopier().Copy(dst, typeof(TmplCopy<int>), src, typeof(TmplCopy<MyItem>), false, null);

			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void TmplCopy_With_ComplexProperty_Test2()
		{
			var src = new TmplCopy<int>() { A = 1 };
			var dst = new TmplCopy<MyItem_ImpliciteCast>();

			new DtoCopier().Copy(dst, typeof(TmplCopy<MyItem_ImpliciteCast>), src, typeof(TmplCopy<int>), false, null);

			dst.A.A.Should().Be(1);
		}

		[TestMethod]
		public void TmplCopy_With_ComplexProperty_Test3()
		{
			var src = new TmplCopy<MyItem_ImpliciteCast>() { A = new MyItem_ImpliciteCast() { A = 1 } };
			var dst = new TmplCopy<int>();

			new DtoCopier().Copy(dst, typeof(TmplCopy<int>), src, typeof(TmplCopy<MyItem_ImpliciteCast>), false, null);

			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void TmplCopy_With_CustomCast_Test()
		{
			var src = new TmplCopy<MyItem_WithoutCast>() { A = new MyItem_WithoutCast() { A = 1 } };
			var dst = new TmplCopy<int>();

			var casts = new DtoCopierCastStorage();
			casts.Append(typeof(MyItem_WithoutCast).GetMethod(nameof(MyItem_WithoutCast.ToInt)));

			new DtoCopier(casts).Copy(dst, typeof(TmplCopy<int>), src, typeof(TmplCopy<MyItem_WithoutCast>), false, null);

			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void TmplCopy_With_CustomTypeCast_Test()
		{
			var src = new TmplCopy<MyItem_WithoutCast>() { A = new MyItem_WithoutCast() { A = 1 } };
			var dst = new TmplCopy<int>();

			var casts = new DtoCopierCastStorage();
			casts.Append(typeof(MyItem_WithoutCast).GetMethod(nameof(MyItem_WithoutCast.ToInt)), typeof(TmplCopy<>));

			new DtoCopier(casts).Copy(dst, typeof(TmplCopy<int>), src, typeof(TmplCopy<MyItem_WithoutCast>), false, null);

			dst.A.Should().Be(1);
		}

		[TestMethod]
		public void TmplCopy_With_CustomTypeCast_ForOtherTmpl_Test()
		{
			var src = new TmplCopy<MyItem_WithoutCast>() { A = new MyItem_WithoutCast() { A = 1 } };
			var dst = new TmplCopy<int>();

			var casts = new DtoCopierCastStorage();
			casts.Append(typeof(MyItem_WithoutCast).GetMethod(nameof(MyItem_WithoutCast.ToInt)), typeof(OtherTmplCopy<>));

			AssertionExtensions.Should(() => new DtoCopier(casts).Copy(dst, typeof(TmplCopy<int>), src, typeof(TmplCopy<MyItem_WithoutCast>), false, null))
				.Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void UpdateInterf_Test()
		{
			var src = new PartialCopyInterfInfo() { A = 1, B = 2, Other = "other" };
			var dst = new PartialCopyInterfInfo();

			new DtoCopier().Copy(dst, src, typeof(IPartialCopyInfo), false, new HashSet<string>(new[] { nameof(src.A) }));

			dst.A.Should().Be(1);
			dst.B.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void UpdateObj_Test()
		{
			var src = new PartialCopyObjInfo() { A = 1, B = 2, Other = "other" };
			var dst = new PartialCopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(IPartialCopyInfo), false, new HashSet<string>(new[] { nameof(src.A) }));

			dst.A.Should().Be(1);
			dst.B.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void UpdateObjToClass_Test()
		{
			var src = new PartialCopyObjInfo() { A = 1, Other = "other" };
			var dst = new PartialCopyInterfInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfo), false, new HashSet<string>(new[] { nameof(src.A) }));

			dst.A.Should().Be(1);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void UpdateClassToObj_Test()
		{
			var src = new PartialCopyInterfInfo() { A = 1, Other = "other" };
			var dst = new PartialCopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfo), false, new HashSet<string>(new[] { nameof(src.A) }));

			dst.A.Should().Be(1);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void UpdateInterf_WithExclude_Test()
		{
			var src = new PartialCopyInterfInfo() { A = 1, B = 2, Other = "other" };
			var dst = new PartialCopyInterfInfo();

			new DtoCopier().Copy(dst, src, typeof(IPartialCopyInfo), true, new HashSet<string>(new[] { nameof(src.B) }));

			dst.A.Should().Be(1);
			dst.B.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void UpdateObj_WithExclude_Test()
		{
			var src = new PartialCopyObjInfo() { A = 1, B = 2, Other = "other" };
			var dst = new PartialCopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(IPartialCopyInfo), true, new HashSet<string>(new[] { nameof(src.B) }));

			dst.A.Should().Be(1);
			dst.B.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void UpdateObjToClass_WithExclude_Test()
		{
			var src = new PartialCopyObjInfo() { A = 1, Other = "other" };
			var dst = new PartialCopyInterfInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfo), true, new HashSet<string>(new[] { nameof(src.B) }));

			dst.A.Should().Be(1);
			dst.B.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void UpdateClassToObj_WithExclude_Test()
		{
			var src = new PartialCopyInterfInfo() { A = 1, Other = "other" };
			var dst = new PartialCopyObjInfo();

			new DtoCopier().Copy(dst, src, typeof(ICopyInfo), true, new HashSet<string>(new[] { nameof(src.B) }));

			dst.A.Should().Be(1);
			dst.B.Should().Be(0);
			dst.Other.Should().BeNull();
		}
	}

	interface ICopyInfo
	{
		int A { get; set; }
	}

	class CopyInterfInfo : ICopyInfo
	{
		public int A { get; set; }
		public string Other { get; set; }
	}

	class CopyObjInfo //: ICopyInfo
	{
		public int A { get; set; }
		public string Other { get; set; }
	}

	class CopyObjInfoChild : CopyObjInfo
	{
		public int B { get; set; }
	}

	interface IPartialCopyInfo
	{
		int A { get; set; }
		int B { get; set; }
	}

	class PartialCopyInterfInfo : IPartialCopyInfo
	{
		public int A { get; set; }
		public int B { get; set; }
		public string Other { get; set; }
	}

	class PartialCopyObjInfo //: IParitialCopyInfo
	{
		public int A { get; set; }
		public int B { get; set; }
		public string Other { get; set; }
	}

	interface ICopyInfoBase
	{
		int A { get; set; }
	}

	interface ICopyInfoChild : ICopyInfoBase
	{
		int B { get; set; }
	}

	class InheritanceCopyInterfInfo : ICopyInfoChild
	{
		public int A { get; set; }
		public int B { get; set; }
		public int C { get; set; }
		public string Other { get; set; }
	}

	class InheritanceCopyObjInfo
	{
		public int A { get; set; }
		public int B { get; set; }
		public int C { get; set; }
		public string Other { get; set; }
	}

	class TmplCopy<T>
	{
		public T A { get; set; }
	}

	class OtherTmplCopy<T>
	{
		public T A { get; set; }
	}

	class MyItem
	{
		public int A { get; set; }

		public static explicit operator MyItem(int src)
		{
			return new MyItem() { A = src };
		}

		public static explicit operator int(MyItem src)
		{
			return src.A;
		}
	}

	class MyItem_ImpliciteCast
	{
		public int A { get; set; }

		public static implicit operator MyItem_ImpliciteCast(int src)
		{
			return new MyItem_ImpliciteCast() { A = src };
		}

		public static implicit operator int(MyItem_ImpliciteCast src)
		{
			return src.A;
		}
	}

	class MyItem_WithoutCast
	{
		public int A { get; set; }

		public static int ToInt(MyItem_WithoutCast t)
		{
			return t.A;
		}
	}
}
