using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace d7k.Dto.Tests
{
	[TestClass]
	public class PathValueIndexerTests
	{
		[TestMethod]
		public void GetValue_Test()
		{
			var value = new { a = 1 };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a);
			indexer.GeneralPath.Should().Be(".a");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(1);
			pathes[0].Path.Should().Be(".a");

			pathes[0].GetValue().Should().Be(1);
			pathes[0].SetValue(2);

			value.a.Should().Be(2);
			pathes[0].GetValue().Should().Be(2);
		}

		[TestMethod]
		public void GetValue_WithComplexPath_Test()
		{
			var value = new { a = new { b = 1 } };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a.b);
			indexer.GeneralPath.Should().Be(".a.b");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(1);
			pathes[0].Path.Should().Be(".a.b");

			pathes[0].GetValue().Should().Be(1);

			pathes[0].SetValue(2);

			value.a.b.Should().Be(2);
			pathes[0].GetValue().Should().Be(2);
		}

		[TestMethod]
		public void GetValue_WithInvalidPath_Test()
		{
			var value = new { a = new { b = 1 } };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a.b);
			indexer.GeneralPath.Should().Be(".a.b");

			var aProperty = value.GetType()
				.GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
				.FirstOrDefault(f => f.Name == $"<a>i__Field");

			aProperty.SetValue(value, null);
			value.a.Should().BeNull();

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(0);
		}

		[TestMethod]
		public void GetIndexedValue_ForArray_Test()
		{
			var value = new { a = new[] { 1, 2 } };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a.First());
			indexer.GeneralPath.Should().Be(".a[]");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(2);
			pathes[0].Path.Should().Be(".a[0]");
			pathes[1].Path.Should().Be(".a[1]");

			pathes[0].GetValue().Should().Be(1);
			pathes[0].SetValue(3);

			value.a.Should().BeEquivalentTo(new[] { 3, 2 });
			pathes[0].GetValue().Should().Be(3);
		}

		[TestMethod]
		public void GetIndexedValue_ForArray_Test1()
		{
			var value = new[] { new { a = 1 }, new { a = 2 } };
			var indexer = PathValueIndexerFactory.Create(value, x => x.First().a);
			indexer.GeneralPath.Should().Be("[].a");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(2);
			pathes[0].Path.Should().Be("[0].a");
			pathes[1].Path.Should().Be("[1].a");

			pathes[0].GetValue().Should().Be(1);
			pathes[0].SetValue(3);

			value[0].a.Should().Be(3);
			pathes[0].GetValue().Should().Be(3);

			value[1].a.Should().Be(2);
			pathes[1].GetValue().Should().Be(2);
		}

		[TestMethod]
		public void GetIndexedValue_ForArray_Test2()
		{
			var value = new { a = new[] { new[] { 1 }, new[] { 2 } } };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a.First().First());
			indexer.GeneralPath.Should().Be(".a[][]");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(2);
			pathes[0].Path.Should().Be(".a[0][0]");
			pathes[1].Path.Should().Be(".a[1][0]");

			pathes[0].GetValue().Should().Be(1);
			pathes[0].SetValue(3);

			value.a[0].Should().BeEquivalentTo(new[] { 3 });
			pathes[0].GetValue().Should().Be(3);
		}

		[TestMethod]
		public void GetIndexedValue_ForArray_Test3()
		{
			var value = new[] { 1, 2 };
			var indexer = PathValueIndexerFactory.Create(value, x => x.First());
			indexer.GeneralPath.Should().Be("[]");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(2);
			pathes[0].Path.Should().Be("[0]");
			pathes[1].Path.Should().Be("[1]");

			pathes[0].GetValue().Should().Be(1);
			pathes[0].SetValue(3);

			value[0].Should().Be(3);
			pathes[0].GetValue().Should().Be(3);

			value[1].Should().Be(2);
			pathes[1].GetValue().Should().Be(2);
		}

		[TestMethod]
		public void GetIndexedValue_ForList_Test()
		{
			var value = new { a = new List<int>(new[] { 1 }) };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a.First());
			indexer.GeneralPath.Should().Be(".a[]");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(1);
			pathes[0].Path.Should().Be(".a[0]");

			pathes[0].GetValue().Should().Be(1);

			pathes[0].SetValue(2);

			value.a.Should().BeEquivalentTo(new[] { 2 });
			pathes[0].GetValue().Should().Be(2);
		}

		[TestMethod]
		public void GetIndexedValue_ForDict_Test()
		{
			var value = new { a = new Dictionary<string, string>() { { "a", "b" }, { "c", "d" } } };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a.First());
			indexer.GeneralPath.Should().Be(".a[]");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(2);
			pathes[0].Path.Should().Be(".a['a']");
			pathes[1].Path.Should().Be(".a['c']");

			pathes[0].GetValue().Should().Be("b");
			pathes[0].SetValue("b1");

			value.a["a"].Should().Be("b1");

			pathes[1].GetValue().Should().Be("d");
			pathes[1].SetValue("d1");

			value.a["c"].Should().Be("d1");
		}

		[TestMethod]
		public void GetIndexedValue_ForArray_WithComplexType_Test()
		{
			var value = new { a = new[] { new { b = 1 } } };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a.First().b);
			indexer.GeneralPath.Should().Be(".a[].b");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(1);
			pathes[0].Path.Should().Be(".a[0].b");

			pathes[0].GetValue().Should().Be(1);

			pathes[0].SetValue(2);

			value.a[0].b.Should().Be(2);
			pathes[0].GetValue().Should().Be(2);
		}

		[TestMethod]
		public void GetIndexedValue_ForArray_WithComplexType_Test1()
		{
			var value = new { a = new[] { new { b = new[] { new { c = 1 } } } } };
			var indexer = PathValueIndexerFactory.Create(value, x => x.a.First().b.First().c);
			indexer.GeneralPath.Should().Be(".a[].b[].c");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(1);
			pathes[0].Path.Should().Be(".a[0].b[0].c");

			pathes[0].GetValue().Should().Be(1);

			pathes[0].SetValue(2);

			value.a[0].b[0].c.Should().Be(2);
			pathes[0].GetValue().Should().Be(2);
		}

		[TestMethod]
		public void RootAccess_Test()
		{
			var value = 5;
			var indexer = PathValueIndexerFactory.Create(value, x => x);
			indexer.GeneralPath.Should().Be("");

			var pathes = indexer.GetPathes(value).ToList();
			pathes.Should().HaveCount(1);
			pathes[0].Path.Should().Be("");

			pathes[0].GetValue().Should().Be(5);
			pathes[0].SetValue(3);

			/*The variable under scope, but any case we can read it.*/
			pathes[0].GetValue().Should().Be(5);
			value.Should().Be(5);
		}
	}
}
