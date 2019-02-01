using d7k.Dto;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Utilities.Dto.Tests
{
	[TestClass]
	public class DtoCopierTests
	{
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
		public void CopyInterf_WithHelper_Test()
		{
			var src = new CopyInterfInfo() { A = 1, Other = "other" };
			var dst = new CopyInterfInfo();

			dst = dst.ReadFrom(src, typeof(ICopyInfo));

			dst.A.Should().Be(1);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void CopyObj_WithHelper_Test()
		{
			var src = new CopyObjInfo() { A = 1, Other = "other" };
			var dst = new CopyObjInfo();

			dst = dst.ReadFrom(src, typeof(ICopyInfo));

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

		[TestMethod]
		public void UpdateObj_WithExclude_WithHelper_Test()
		{
			var src = new PartialCopyObjInfo() { A = 1, B = 2, Other = "other" };
			var dst = new PartialCopyObjInfo();

			dst = src.UpdateWithExclude(dst, typeof(IPartialCopyInfo), nameof(src.B));

			dst.A.Should().Be(1);
			dst.B.Should().Be(0);
			dst.Other.Should().BeNull();
		}

		[TestMethod]
		public void ReadFrom_ForInheritedClasses_Test()
		{
			var it0 = new CopyObjInfo() { A = 1, Other = "it0" };
			var it1 = new CopyObjInfoChild();

			it1.ReadFrom(it0);

			it1.A.Should().Be(1);
			it1.Other.Should().Be("it0");
			it1.B.Should().Be(0);
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
}
