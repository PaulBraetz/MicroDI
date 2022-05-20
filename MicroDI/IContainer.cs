using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI
{
	public interface IContainer
	{
		Object Resolve(Type serviceType);
		Boolean IsRegistered(Type serviceType);
	}
}
