using MicroDI;
using MicroDI.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Tests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestContainerDirect()
		{
			IContainer container = new Container();

			String arg = "Some Parameter";

			container.AddTransient<Object, String>(arg.ToCharArray());

			var resolved = container.Resolve<Object>();

			Assert.AreEqual(resolved.ToString(), arg);
		}
		[TestMethod]
		public void TestContainerInterfaceParam()
		{
			IContainer container = new Container();

			IEnumerable<String> arg = new[] { "Value 1", "Value 2" };

			container.AddTransient<IEnumerable<String>, List<String>>(arg);

			IEnumerable<String> resolved = container.Resolve<IEnumerable<String>>();

			Assert.IsTrue(resolved.SequenceEqual(arg));
		}
		private sealed class SingletonService
		{
			public SingletonService()
			{
				Interlocked.Increment(ref instanceCount);
				passedArg = "No Arg";
			}
			public SingletonService(String arg)
			{
				Interlocked.Increment(ref instanceCount);
				passedArg = arg;
			}
			private static Int32 instanceCount;
			private readonly String passedArg;

			public override String ToString()
			{
				return GetStringRepresentation(instanceCount, passedArg);
			}
			public static String GetStringRepresentation(Int32 instanceCount, String arg)
			{
				return $"Instances created: {instanceCount}, Arg passed to this instance: {arg}";
			}
		}

		[TestMethod]
		public void TestContainerSingleton()
		{
			IContainer container = new Container();

			String arg = "Some Parameter";

			container.AddSingleton<Object, SingletonService>(arg);

			String expected = SingletonService.GetStringRepresentation(1, arg);

			for (int i = 1; i < 10; i++)
			{
				var resolved = container.Resolve<Object>();
				Assert.AreEqual(expected, resolved.ToString());
			}
		}

		private sealed class TransientService1
		{
			public TransientService1()
			{
				Interlocked.Increment(ref instanceCount);
				PassedArg = "No Arg";
			}
			public TransientService1(String arg)
			{
				Interlocked.Increment(ref instanceCount);
				PassedArg = arg;
			}
			private static Int32 instanceCount;
			public readonly String PassedArg;

			public override String ToString()
			{
				return GetStringRepresentation(instanceCount, PassedArg);
			}
			public static String GetStringRepresentation(Int32 instanceCount, String arg)
			{
				return $"Instances created: {instanceCount}, Arg passed to this instance: {arg}";
			}
		}

		[TestMethod]
		public void TestContainerTransient()
		{
			IContainer container = new Container();

			String arg = "Some Parameter";

			container.AddTransient<Object, TransientService1>(arg);

			for (Int32 i = 1; i < 10; i++)
			{
				Object? resolved = container.Resolve<Object>();
				String expected = TransientService1.GetStringRepresentation(i, arg);
				Assert.AreEqual(expected, resolved.ToString());
			}
		}
		private sealed class TransientService2
		{
			public TransientService2()
			{
				Interlocked.Increment(ref instanceCount);
				PassedArg = "No Arg";
			}
			public TransientService2(String arg)
			{
				Interlocked.Increment(ref instanceCount);
				PassedArg = arg;
			}
			private static Int32 instanceCount;
			public readonly String PassedArg;

			public override String ToString()
			{
				return GetStringRepresentation(instanceCount, PassedArg);
			}
			public static String GetStringRepresentation(Int32 instanceCount, String arg)
			{
				return $"Instances created: {instanceCount}, Arg passed to this instance: {arg}";
			}
		}
		[TestMethod]
		public void TestContainerByName()
		{
			IContainer container = new Container();

			container.AddTransient(typeof(TransientService2), typeof(TransientService2), "Arg");
			for (int i = 0; i< 10; i++)
			{
				container.AddTransient(typeof(TransientService2), $"Service {i}", typeof(TransientService2), $"Arg {i}");
			}

			var resolved = container.Resolve<TransientService2>();
			Assert.AreEqual("Arg", resolved.PassedArg);

			for (int i = 0; i < 10; i++)
			{
				resolved = container.Resolve<TransientService2>($"Service {i}");
				Assert.AreEqual($"Arg {i}", resolved.PassedArg);
			}
		}
	}
}