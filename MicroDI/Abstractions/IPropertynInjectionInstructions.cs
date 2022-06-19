using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI.Abstractions
{
	public interface IPropertynInjectionInstructions : IConstructorInjectionInstructions
	{
		IDictionary<String, Object> PropertyValues { get; }
		IDictionary<String, IServiceRegistration> PropertyInjectionValues { get; }
	}
}
