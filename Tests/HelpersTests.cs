using MicroDI;
using MicroDI.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
	[TestClass]
	public class HelpersTests
	{
		private sealed class TestClass1
		{
			public String? Property1 { get; set; }
			public Int32 Property2 { get; set; }
			public TestDependency1? Property3 { get; set; }
		}
		private sealed class TestDependency1
		{
			public TestDependency1()
			{
				InstanceCount++;
			}
			public static Int32 InstanceCount { get; private set; }
		}

		[TestMethod($"{nameof(Helpers)} provides {nameof(Helpers.GetConstructorExpression)}")]
		public void ProvidesGetConstructorExpression()
		{
			var definition = new ServiceDefinition(typeof(TestDependency1));
			var instructions = new ConstructorInjectionInstructions(typeof(TestDependency1), Array.Empty<Object>());
			var factory = new TransientServiceFactory(instructions);
			var registration = new ServiceRegistration(definition, factory);

			var lambda = Helpers.GetConstructorExpression(typeof(TestClass1), new Object[] { 827356385, "Test Value" }, new IServiceRegistration[] { registration });

			var func = (Func<TestClass1>)lambda.Compile();

			var instance = func.Invoke();

			Assert.AreEqual("Test Value", instance.Property1);
			Assert.AreEqual(827356385, instance.Property2);
			Assert.IsNotNull(instance.Property3);
			Assert.AreEqual(1, TestDependency1.InstanceCount);
		}
	}
}
