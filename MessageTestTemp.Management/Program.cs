using MassTransit;
using MassTransit.RabbitMqTransport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageTestTemp.Management
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Title = "Management consumer";
			Console.WriteLine("MANAGEMENT");

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

				rabbit.ReceiveEndpoint(host, "mycompany.domains.queues.events.mgmt", conf =>
				{
					conf.Consumer<CustomerRegisteredConsumerMgmt>();
				});
			});

			busControl.Start();
			Console.ReadKey();

			busControl.Stop();
		}
	}
}
