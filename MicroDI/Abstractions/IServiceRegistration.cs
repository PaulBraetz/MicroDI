using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroDI.Abstractions
{
	public interface IServiceRegistration
	{
		IServiceDefinition Definition { get; }
		IServiceFactory Factory { get; }
	}
}
