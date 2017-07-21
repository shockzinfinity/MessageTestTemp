using MassTransit;
using MessageTestTemp.Messaging;
using System;
using System.Threading.Tasks;

namespace MessageTestTemp.Publisher2
{
	internal class Program
	{
		public static object RegisterNewOrderConsumer { get; private set; }

		private static void Main(string[] args)
		{
			Console.Title = "Publisher window 2";
			Console.WriteLine("CUSTOMER REGISTRATION COMMAND PUBLISHER 2.");

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
				Address = "Old street",
				Id = Guid.NewGuid(),
				Preferred = true,
				registeredUtc = DateTime.UtcNow,
				Name = "Bad people LTD",
				Type = 2,
				DefaultDiscount = 30
			}, c => c.FaultAddress = new Uri($"{rabbitMqAddress}/mycompany.queues.errors.newcustomers"));

			Console.ReadKey();
		}
	}
}