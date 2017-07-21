using MassTransit;
using MessageTestTemp.Messaging;
using System;
using System.Threading.Tasks;

namespace MessageTestTemp.Publisher
{
	internal class Program
	{
		public static object RegisterNewOrderConsumer { get; private set; }

		private static void Main(string[] args)
		{
			Console.Title = "Publisher window 1";
			Console.WriteLine("CUSTOMER REGISTRATION COMMAND PUBLISHER 1.");

			RunMassTransitPublisherWithRabbit();
		}

		private static void RunMassTransitPublisherWithRabbit()
		{
			string rabbitMqAddress = "rabbitmq://sapdev1.semubot.com/accounting";
			string rabbitQueue = "mycompany.domains.queues";
			Uri rabbitMqRootUri = new Uri(rabbitMqAddress);

			IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
			{
				rabbit.Host(rabbitMqRootUri, settings =>
				{
					settings.Password("accountant");
					settings.Username("accountant");
				});
			});

			Task<ISendEndpoint> sendEndpointTask = busControl.GetSendEndpoint(new Uri($"{rabbitMqAddress}/{rabbitQueue}"));
			ISendEndpoint sendEndpoint = sendEndpointTask.Result;

			Task sendTask = sendEndpoint.Send<IRegisterCustomer>(new
			{
				Address = "New street",
				Id = Guid.NewGuid(),
				Preferred = true,
				registeredUtc = DateTime.UtcNow,
				Name = "Nice people LTD",
				Type = 1,
				DefaultDiscount = 0
			});

			Console.ReadKey();
		}
	}
}