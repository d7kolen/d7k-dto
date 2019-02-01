using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace d7k.Dto.Tests
{
	[TestClass]
	public class DtoAdapterTests
	{
		[TestMethod]
		public void DtoAdapter_Test()
		{
			var obj = new MyObj() { A = 1, B = "abc" };
			var adapt = obj.DtoAdapter<IMyObj>();

			adapt.A.Should().Be(1);

			obj.A = 2;
			adapt.A.Should().Be(2);

			adapt.A = 3;
			obj.A.Should().Be(3);
		}

		[TestMethod]
		public void DtoAdapter_ForAdapter_Test()
		{
			var obj = new MyObj() { A = 1, B = "abc" };
			var adapt = obj.DtoAdapter<IMyObj>();

			var adaptV1 = adapt.DtoAdapter<IMyObj>();

			adapt.GetType().Should().Be(adaptV1.GetType());
		}

		[TestMethod]
		public void DtoAdapter_ForPrivate_Test()
		{
			var obj = new MyPrivateObj() { A = 1, B = "abc" };
			var adapt = obj.DtoAdapter<IMyObj>();

			adapt.A.Should().Be(1);

			obj.A = 2;
			adapt.A.Should().Be(2);

			adapt.A = 3;
			obj.A.Should().Be(3);
		}

		[TestMethod]
		public void DtoAdapter_ForAnanim_Test()
		{
			var obj = new { A = 1, B = "abc" };
			var adapt = obj.DtoAdapter<IMyObj>();

			adapt.A.Should().Be(1);

			adapt.A = 3;
			adapt.A.Should().Be(3);
			obj.A.Should().Be(3);
		}

		[TestMethod]
		public void DtoAdapter_MappingFail_Test()
		{
			var obj0 = new MySecondObj() { B = "abc" };

			AssertionExtensions.Should(() => obj0.DtoAdapter<IMyObj>()).Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void DtoAdapter_PrivateInterface_Test()
		{
			var obj0 = new MyObj() { A = 1, B = "abc" };

			AssertionExtensions.Should(() => obj0.DtoAdapter<IMyPrivateObj>()).Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void DtoAdapter_InterfaceWithMethods_Test()
		{
			var obj0 = new MyMethodObj() { A = 1 };

			AssertionExtensions.Should(() => obj0.DtoAdapter<IMyMethodObj>()).Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void DtoAdapter_MappingTypeFail_Test()
		{
			var obj0 = new MyWrongTypeObj() { A = "abc" };

			AssertionExtensions.Should(() => obj0.DtoAdapter<IMyObj>()).Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void DtoAdapter_Empty_Test()
		{
			var obj0 = new MyEmptyObj();

			Assert.IsTrue(obj0.DtoAdapter<IMyEmptyObj>() is IMyEmptyObj);
		}

		[TestMethod]
		public void DtoAdapter_PrivateEmpty_Test()
		{
			var obj0 = new MyPrivateEmptyObj();

			Assert.IsTrue(obj0.DtoAdapter<IMyEmptyObj>() is IMyEmptyObj);
		}

		[TestMethod]
		public void DtoAdapter_ForOtherInterfAdapter_Test()
		{
			var obj = new MyClassWithTwoInterfaces { A = 1, B = 2 };

			var adapt0 = obj.DtoAdapter<IMyObj>();
			adapt0.A.Should().Be(1);
			adapt0.A = 11;
			obj.A.Should().Be(11);

			var adapt1 = adapt0.DtoAdapter<IMyOtherObj>();
			adapt1.B.Should().Be(2);
			adapt1.B = 12;
			obj.B.Should().Be(12);
		}

		[TestMethod]
		public void Dto_Test()
		{
			var dto = DtoFactory.Dto<IMyObj>();
			dto.A = 1;
			dto.A.Should().Be(1);
		}

		[TestMethod]
		public void Dto_ForChildInterface()
		{
			var dto = DtoFactory.Dto<IMyChildObj>();
			dto.A = 1;
			dto.B = 2;

			dto.A.Should().Be(1);
			dto.B.Should().Be(2);
		}
	}

	public interface IMyObj
	{
		int A { get; set; }
	}

	public interface IMyChildObj : IMyObj
	{
		int B { get; set; }
	}

	public interface IMyOtherObj
	{
		int B { get; set; }
	}

	interface IMyPrivateObj
	{
		int A { get; set; }
	}

	public class MyObj
	{
		public int A { get; set; }
		public string B { get; set; }
	}

	class MyPrivateObj
	{
		public int A { get; set; }
		public string B { get; set; }
	}

	public class MySecondObj
	{
		public string B { get; set; }
	}

	public class MyWrongTypeObj
	{
		public string A { get; set; }
	}

	public interface IMyMethodObj
	{
		int A { get; set; }
		void Meth();
	}

	public class MyMethodObj
	{
		public int A { get; set; }

		public void Meth() { }
	}

	public interface IMyEmptyObj { }

	public class MyEmptyObj { }

	class MyPrivateEmptyObj { }

	class MyClassWithTwoInterfaces
	{
		public int A { get; set; }
		public int B { get; set; }
	}
}
