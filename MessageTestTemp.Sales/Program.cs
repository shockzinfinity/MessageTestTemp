using MassTransit;
using MassTransit.RabbitMqTransport;
using System;

namespace MessageTestTemp.Sales
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.Title = "Sales consumer";
			Console.WriteLine("SALES");

			RunMassTransitReceiverWithRabbit();
		}

		private static void RunMassTransitReceiverWithRabbit()
		{
			IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
			{
				IRabbitMqHost host = rabbit.Host(new Uri("rabbitmq://sapdev1.semubot.com/accounting"), settings =>
				{
					settings.Password("accountant");
					settings.Username("accountant");
				});

				rabbit.ReceiveEndpoint(host, "mycompany.domains.queues.events.sales", conf =>
				{
					conf.Consumer<CustomerRegisteredConsumerSls>();
				});
			});

			busControl.Start();
			Console.ReadKey();

			busControl.Stop();
		}
	}
}