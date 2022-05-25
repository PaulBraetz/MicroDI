using MicroDI;
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
		public void TestContainerFactoryDirect()
		{
			IContainerFactory factory = new ContainerFactory();

			String arg = "Some Parameter";

			factory.AddTransient<Object, String>(arg.ToCharArray());

			IContainer container = factory.Build();

			var resolved = container.Resolve<Object>();

			Assert.AreEqual(resolved.ToString(), arg);
		}
		[TestMethod]
		public void TestContainerFactoryInterfaceParam()
		{
			IContainerFactory factory = new ContainerFactory();

			IEnumerable<String> arg = new[] { "Value 1", "Value 2" };

			factory.AddTransient<IEnumerable<String>, List<String>>(arg);

			IContainer container = factory.Build();

			IEnumerable<String> resolved = container.Resolve<IEnumerable<String>>();

			Assert.IsTrue(resolved.SequenceEqual(arg));
		}
		private sealed class TestService
		{
			public TestService()
			{
				Interlocked.Increment(ref instanceCount);
				passedArg = "No Arg";
			}
			public TestService(String arg)
			{
				Interlocked.Increment(ref instanceCount);
				passedArg = arg;
			}
			private static int instanceCount;
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
		public void TestContainerFactorySingleton()
		{

			IContainerFactory factory = new ContainerFactory();

			String arg = "Some Parameter";

			IContainer container = factory.AddTransient<Object, TestService>(arg).Build();

			var resolved = container.Resolve<Object>();

			Assert.AreEqual(resolved.ToString(), arg);
		}
	}
}