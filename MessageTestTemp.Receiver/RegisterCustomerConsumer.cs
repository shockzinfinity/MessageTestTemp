using MassTransit;
using MessageTestTemp.Domain;
using MessageTestTemp.Messaging;
using System;
using System.Threading.Tasks;

namespace MessageTestTemp.Receiver
{
	public class RegisterCustomerConsumer : IConsumer<IRegisterCustomer>
	{
		private readonly ICustomerRepository _customerRepository;

		public RegisterCustomerConsumer(ICustomerRepository customerRepository)
		{
			if (customerRepository == null)
				throw new ArgumentNullException(nameof(customerRepository));

			_customerRepository = customerRepository;
		}

		public Task Consume(ConsumeContext<IRegisterCustomer> context)
		{
			// for failure test
			//throw new ArgumentException("We pretend that an exception was thrown...");

			IRegisterCustomer newCustomer = context.Message;
			Console.WriteLine($"A new customer has signed up, it's time to register it. Details: ");
			Console.WriteLine(newCustomer.Address);
			Console.WriteLine(newCustomer.Name);
			Console.WriteLine(newCustomer.Id);
			Console.WriteLine(newCustomer.Preferred);

			// for DI
			_customerRepository.Save(new Customer(newCustomer.Id, newCustomer.Name, newCustomer.Address)
			{
				DefaultDiscount = newCustomer.DefaultDiscount,
				Preferred = newCustomer.Preferred,
				RegisteredUtc = newCustomer.RegisteredUtc,
				Type = newCustomer.Type
			});

			context.Publish<ICustomerRegistered>(new
			{
				Address = newCustomer.Address,
				Id = newCustomer.Id,
				RegisteredUtc = newCustomer.RegisteredUtc,
				Name = newCustomer.Name
			});

			return Task.FromResult(context.Message);
		}
	}
}