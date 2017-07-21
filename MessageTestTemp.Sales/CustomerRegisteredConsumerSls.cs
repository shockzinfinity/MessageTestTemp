using MassTransit;
using MessageTestTemp.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageTestTemp.Sales
{
	public class CustomerRegisteredConsumerSls : IConsumer<ICustomerRegistered>
	{
		public Task Consume(ConsumeContext<ICustomerRegistered> context)
		{
			ICustomerRegistered newCustomer = context.Message;

			Console.WriteLine("Great to see the new customer finally being registered, a big sigh from sales!"); ;
			Console.WriteLine(newCustomer.Address);
			Console.WriteLine(newCustomer.Name);
			Console.WriteLine(newCustomer.Id);

			return Task.FromResult(context.Message);
		}
	}
}
