using MessageTestTemp.Domain;
using System;
using System.Threading;

namespace MessageTestTemp.Repository
{
	public class CustomerRepository : ICustomerRepository
	{
		public void Save(Customer customer)
		{
			Thread.Sleep(3 * 1000);

			Console.WriteLine($"The concrete customer repository was called for customer [{customer.Id}]'{customer.Name}'");
		}
	}
}