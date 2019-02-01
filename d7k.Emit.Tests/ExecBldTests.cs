using System;
using System.Reflection;
using System.Reflection.Emit;
using d7k.Emit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace d7k.Tests
{
	/// <summary>
	/// Summary description for ExecBldTest
	/// </summary>
	[TestClass]
	public class ExecBldTest
	{
		[TestMethod]
		public void ReturnExecBldTest()
		{
			ExecBld.Return().CreateMethod<int>()(0);

			var ex = ExecBld.Return(ExecBld.GetArg(0));
			Assert.AreEqual(1, ex.CreateMethod<int, int>()(1));
		}

		[TestMethod]
		public void GetConstExecBldTest()
		{
			var ex = ExecBld.Return(ExecBld.GetConst(5));
			Assert.AreEqual(5, ex.CreateMethod<int, int>()(1));

			ex = ExecBld.Return(ExecBld.GetConst(true));
			Assert.IsTrue(ex.CreateMethod<int, bool>()(1));

			ex = ExecBld.Return(ExecBld.GetConst(false));
			Assert.IsFalse(ex.CreateMethod<int, bool>()(1));
		}

		[TestMethod]
		public void GetFieldExecBldTest()
		{
			var data = new Data() { A = 5 };

			var ex = ExecBld.Return(
				ExecBld.GetFld(data.GetFld("A"), ExecBld.GetArg(0)));

			var func = ex.CreateMethod<Data, int>();

			Assert.AreEqual(5, func(data));
		}

		[TestMethod]
		public void SetFieldExecBldTest()
		{
			var data = new Data();

			var ex = ExecBld.Return(
				ExecBld.SetFld(data.GetFld("A"), ExecBld.GetArg(0), ExecBld.GetConst(5)));

			ex.CreateMethod<Data>()(data);
			Assert.AreEqual(5, data.A);
		}

		[TestMethod]
		public void GetPropertyExecBldTest()
		{
			var data = new Data() { B = 5 };

			var ex = ExecBld.Return(
				ExecBld.GetProp(data.GetProp("B"), ExecBld.GetArg(0)));

			var func = ex.CreateMethod<Data, int>();
			Assert.AreEqual(5, func(data));
		}

		[TestMethod]
		public void SetPropExecBldTest()
		{
			var data = new Data();

			var ex = ExecBld.Return(
				ExecBld.SetProp(data.GetProp("B"), ExecBld.GetArg(0), ExecBld.GetConst(5)));

			ex.CreateMethod<Data>()(data);
			Assert.AreEqual(5, data.B);
		}

		[TestMethod]
		public void GetItemExecBldTest()
		{
			var data = new[] { 3 };

			var ex = ExecBld.Return(
				ExecBld.GetItem<int>(
					ExecBld.GetArg(0),
					ExecBld.GetConst(0)));

			var func = ex.CreateMethod<int[], int>();

			Assert.AreEqual(3, func(data));
		}

		[TestMethod]
		public void SetItemExecBldTest()
		{
			var data = new[] { 0 };

			var ex = ExecBld.Return(
				ExecBld.SetItem<int>(
					ExecBld.GetArg(0),
					ExecBld.GetConst(0),
					ExecBld.GetConst(5)));

			ex.CreateMethod<int[]>()(data);

			Assert.AreEqual(5, data[0]);
		}

		[TestMethod]
		public void UnboxExecBldTest()
		{
			var ex = ExecBld.Return(
				ExecBld.Cast(ExecBld.GetArg(0), typeof(object), typeof(int)));

			var func = ex.CreateMethod<object, int>();

			Assert.AreEqual(3, func(3));
		}

		[TestMethod]
		public void CastClassExecBldTest()
		{
			var ex = ExecBld.Return(
				ExecBld.Cast(ExecBld.GetArg(0), typeof(object), typeof(string)));

			var func = ex.CreateMethod<object, string>();

			Assert.AreEqual("aaa", func("aaa"));
		}

		[TestMethod]
		public void CastValueExecBldTest()
		{
			var ex = ExecBld.Return(
				ExecBld.Cast(ExecBld.GetArg(0), typeof(int), typeof(int?)));

			var func = ex.CreateMethod<int, int?>();

			Assert.AreEqual(1, func(1));
		}

		[TestMethod]
		public void BlockExecBldTest()
		{
			var data = new[] { 0, 1 };

			var ex = ExecBld.Return(
				ExecBld.Block(
					ExecBld.SetItem<int>(
						ExecBld.GetArg(0),
						ExecBld.GetConst(0),
						ExecBld.GetConst(5)),
					ExecBld.SetItem<int>(
						ExecBld.GetArg(0),
						ExecBld.GetConst(1),
						ExecBld.GetConst(5))));

			ex.CreateMethod<int[]>()(data);

			Assert.AreEqual(5, data[0]);
			Assert.AreEqual(5, data[1]);
		}
	}

	static class ExecBldTestHelper
	{
		public static ModuleBuilder CreateModule(this ExecBldTest test)
		{
			var ass = AppDomain.CurrentDomain.DefineDynamicAssembly(
				new AssemblyName("test"), AssemblyBuilderAccess.RunAndCollect);
			return ass.DefineDynamicModule("test_m");
		}

		public static Action<T> CreateMethod<T>(this ExecBld ex)
		{
			var meth = new DynamicMethod("meth", null, new[] { typeof(T) }, true);
			ex.Build(meth.GetILGenerator());
			return (Action<T>)meth.CreateDelegate(typeof(Action<T>));
		}

		public static Func<TArg, TRes> CreateMethod<TArg, TRes>(this ExecBld ex)
		{
			var meth = new DynamicMethod("meth", typeof(TRes), new[] { typeof(TArg) }, true);
			ex.Build(meth.GetILGenerator());
			return (Func<TArg, TRes>)meth.CreateDelegate(typeof(Func<TArg, TRes>));
		}

		public static FieldInfo GetFld(this object obj, string fldName)
		{
			return obj.GetType().GetField(fldName);
		}

		public static PropertyInfo GetProp(this object obj, string fldName)
		{
			return obj.GetType().GetProperty(fldName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
		}
	}

	class Data
	{
		public int A;
		public int B { get; set; }
	}
}
