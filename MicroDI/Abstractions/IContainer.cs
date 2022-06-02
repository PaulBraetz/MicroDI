using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI.Abstractions
{
	public interface IContainer : IEnumerable<IServiceRegistration>
	{
		void Add(IServiceRegistration serviceRegistration);
		Object Resolve(IServiceDefinition serviceDefinition);
		Boolean IsRegistered(IServiceDefinition serviceDefinition);
	}
}
