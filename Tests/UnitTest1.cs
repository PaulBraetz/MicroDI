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
		public void TestContainerFactorySingleton()
		{

			IContainerFactory factory = new ContainerFactory();

			String arg = "Some Parameter";


			IContainer container = factory.AddSingleton<Object, SingletonService>(arg).Build();

			String expected = SingletonService.GetStringRepresentation(1, arg);

			for (int i = 1; i < 10; i++)
			{
				var resolved = container.Resolve<Object>();
				Assert.AreEqual(expected, resolved.ToString());
			}
		}

		private sealed class TransientService
		{
			public TransientService()
			{
				Interlocked.Increment(ref instanceCount);
				passedArg = "No Arg";
			}
			public TransientService(String arg)
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
		public void TestContainerFactoryTransient()
		{
			IContainerFactory factory = new ContainerFactory();

			String arg = "Some Parameter";

			IContainer container = factory.AddTransient<Object, TransientService>(arg).Build();

			for (Int32 i = 1; i < 10; i++)
			{
				Object? resolved = container.Resolve<Object>();
				String expected = TransientService.GetStringRepresentation(i, arg);
				Assert.AreEqual(expected, resolved.ToString());
			}
		}
	}
}