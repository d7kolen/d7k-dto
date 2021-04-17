using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace d7k.Dto.Tests
{
	[TestClass]
	public class JObjectPathTests
	{
		[TestMethod]
		public void JObjectPath_GetValue_Test()
		{
			var path = new JObjectPath(x => x["field"]);

			var obj = JObject.FromObject(new { field = "name" });

			path.Get(obj, new[] { "field" }).Should().Be("name");
		}

		[TestMethod]
		public void JObjectPath_GetObject_Test()
		{
			var path = new JObjectPath(x => x["field"]);

			var obj = JObject.FromObject(new { field = new { other = "name" } });

			var res = (JObject)path.Get(obj, new[] { "field" });

			((string)res["other"]).Should().Be("name");
		}

		[TestMethod]
		public void JObjectPath_GetArray_Test()
		{
			var path = new JObjectPath(x => x["field"]);

			var obj = JObject.FromObject(new { field = new[] { "name" } });

			var res = (object[])path.Get(obj, new[] { "field" });

			res.Should().HaveCount(1);
			res[0].Should().Be("name");
		}

		[TestMethod]
		public void JObjectGetValue_WithArray_Test()
		{
			var path = new JObjectPath(x => x["field"].ScanArr);

			var obj = JObject.FromObject(new { field = new[] { "name", "name1" } });

			path.Get(obj, new object[] { "field", 0 }).Should().Be("name");
			path.Get(obj, new object[] { "field", 1 }).Should().Be("name1");
		}

		[TestMethod]
		public void JObjectSetPath_Test()
		{
			var path = new JObjectPath(x => x["field"]);

			var obj = JObject.FromObject(new { field = "name" });

			path.Set(obj, new[] { "field" }, "other");

			((string)obj["field"]).Should().Be("other");
		}

		[TestMethod]
		public void JObjectSetPath_Object_Test()
		{
			var path = new JObjectPath(x => x["field"]["other"]);

			var obj = JObject.FromObject(new { field = new { other = "name" } });

			path.Set(obj, new[] { "field" }, JObject.FromObject(new { other = "name1" }));

			((string)obj["field"]["other"]).Should().Be("name1");
		}

		[TestMethod]
		public void JObjectSetPath_Array_Test()
		{
			var path = new JObjectPath(x => x["field"]["other"]);

			var obj = JObject.FromObject(new { field = new[] { "name" } });

			path.Set(obj, new[] { "field" }, JArray.FromObject(new[] { "name1" }));

			((string)obj["field"][0]).Should().Be("name1");
		}

		[TestMethod]
		public void JObjectSetPath_PlainArray_Test()
		{
			var path = new JObjectPath(x => x["field"]["other"]);

			var obj = JObject.FromObject(new { field = new[] { "name" } });

			path.Set(obj, new[] { "field" }, new[] { "name1" });

			((string)obj["field"][0]).Should().Be("name1");
		}

		[TestMethod]
		public void JObjectSetPath_WithArray_Test()
		{
			var path = new JObjectPath(x => x["field"].ScanArr);

			var obj = JObject.FromObject(new { field = new[] { "name", "name1" } });

			path.Set(obj, new object[] { "field", 1 }, "other");

			((string)obj["field"][0]).Should().Be("name");
			((string)obj["field"][1]).Should().Be("other");
		}

		[TestMethod]
		public void JObject_GetAllIndexes_Test()
		{
			var path = new JObjectPath(x => x["field"]["other"]);

			var obj = JObject.FromObject(new { field = new { other = 1 } });

			var indexes = path.GetAllIndexes(obj).ToList();

			indexes.Should().HaveCount(1);
			((string)indexes[0][0]).Should().Be("field");
			((string)indexes[0][1]).Should().Be("other");
		}

		[TestMethod]
		public void JObject_GetAllIndexes_WithArr_Test()
		{
			var path = new JObjectPath(x => x["field"].ScanArr["other"]);

			var obj = JObject.FromObject(new { field = new[] { new { other = 1 } } });

			var indexes = path.GetAllIndexes(obj).ToList();

			indexes.Should().HaveCount(1);
			((string)indexes[0][0]).Should().Be("field");
			((int)indexes[0][1]).Should().Be(0);
			((string)indexes[0][2]).Should().Be("other");
		}

		[TestMethod]
		public void JObject_GetAllIndexes_WithArr_Test1()
		{
			var path = new JObjectPath(x => x["field"]["other"].ScanArr);

			var obj = JObject.FromObject(new { field = new { other = new[] { 1 } } });

			var indexes = path.GetAllIndexes(obj).ToList();

			indexes.Should().HaveCount(1);
			((string)indexes[0][0]).Should().Be("field");
			((string)indexes[0][1]).Should().Be("other");
			((int)indexes[0][2]).Should().Be(0);
		}

		[TestMethod]
		public void JObject_GetAllIndexes_WithArr_Test2()
		{
			var path = new JObjectPath(x => x["field"].ScanArr["other"]);

			var obj = JObject.FromObject(new { field = new[] { new { other = 1 }, new { other = 2 } } });

			var indexes = path.GetAllIndexes(obj).ToList();

			indexes.Should().HaveCount(2);

			((string)indexes[0][0]).Should().Be("field");
			((int)indexes[0][1]).Should().Be(0);
			((string)indexes[0][2]).Should().Be("other");

			((string)indexes[1][0]).Should().Be("field");
			((int)indexes[1][1]).Should().Be(1);
			((string)indexes[1][2]).Should().Be("other");
		}

		[TestMethod]
		public void JObject_GetAllIndexes_ToJObject_Test()
		{
			var path = new JObjectPath(x => x["field"]["other"]);

			var obj = JObject.FromObject(new { field = new { other = new { other1 = 1 } } });

			var indexes = path.GetAllIndexes(obj).ToList();

			indexes.Should().HaveCount(1);
			((string)indexes[0][0]).Should().Be("field");
			((string)indexes[0][1]).Should().Be("other");

			var part = path.Get(obj, indexes[0]);

			part.Should().BeOfType<JObject>();
			var objPart = (JObject)part;

			((int)objPart["other1"]).Should().Be(1);
		}

		[TestMethod]
		public void JObject_PathName_Test()
		{
			var path = new JObjectPath(x => x["field"]["other"]);
			path.PathName().Should().Be("JObject : field.other");
		}

		[TestMethod]
		public void JObject_PathName_Empty_Test()
		{
			var path = new JObjectPath(x => x);
			path.PathName().Should().Be("JObject");
		}

		[TestMethod]
		public void JObject_PathName_WithArray_Test()
		{
			var path = new JObjectPath(x => x["a"].ScanArr);
			path.PathName().Should().Be("JObject : a.[]");
		}

		[TestMethod]
		public void JObject_PathName_WithArray_Test1()
		{
			var path = new JObjectPath(x => x.ScanArr["a"]);
			path.PathName().Should().Be("JObject : [].a");
		}

		[TestMethod]
		public void JObject_IndexedName_WithArray_Test()
		{
			var path = new JObjectPath(x => x["a"].ScanArr);
			path.IndexedName(new object[] { "a", 2 }).Should().Be("a.[2]");
		}

		[TestMethod]
		public void JObject_IndexedName_WithArray_Test1()
		{
			var path = new JObjectPath(x => x.ScanArr["a"]);
			path.IndexedName(new object[] { 2, "a" }).Should().Be("[2].a");
		}
		
		[TestMethod]
		public void JObject_Test()
		{
			var obj = JObject.FromObject(new { field = " name " });

			var validation = new ValidationRepository().Create<JObject>(obj);

			validation.JObjectRuleFor(x => x["field"].Type<string>()).Trim();
			validation.Fix(obj);

			obj["field"].Value<string>().Should().Be("name");
		}
	}
}