using Autofac;
using GreenPipes;
using MassTransit;
using MassTransit.RabbitMqTransport;
using MessageTestTemp.Domain;
using MessageTestTemp.Repository;
using System;

namespace MessageTestTemp.Receiver
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			Console.Title = "This is the customer registration command receiver.";
			Console.WriteLine("CUSTOMER REGISTRATION COMMAND RECEIVER.");

			RunMassTransitReceiverWithRabbit();
		}

		private static void RunMassTransitReceiverWithRabbit()
		{
			// for DI (autofac)
			var builder = new ContainerBuilder();

			builder.RegisterType<RegisterCustomerConsumer>();
			builder.RegisterType<RegisterCustomerFaultConsumer>();
			builder.RegisterType<CustomerRepository>().As<ICustomerRepository>();

			builder.Register(context =>
			{
				IBusControl busControl = Bus.Factory.CreateUsingRabbitMq(rabbit =>
				{
					IRabbitMqHost host = rabbit.Host(new Uri("rabbitmq://sapdev1.semubot.com/accounting"), settings =>
					{
						settings.Password("accountant");
						settings.Username("accountant");
					});

					rabbit.ReceiveEndpoint(host, "mycompany.domains.queues", conf =>
					{
						//conf.Consumer<RegisterCustomerConsumer>(); // ordinary

						conf.LoadFrom(context);

						// REF: for retry strategy
						//conf.UseRetry(r =>
						//{
						//	r.Handle<ArgumentException>();
						//	r.Handle<ArgumentException>(e => e.Message.IndexOf("We Pretend that an exception was thrown") > -1);
						//	r.Ignore<ArgumentException>();
						//	r.Exponential(5, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5));
						//	r.Incremental(5, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3));
						//	r.Interval(5, TimeSpan.FromSeconds(5));
						//	r.Intervals(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(4));
						//	r.Immediate(5);
						//});

						conf.UseRetry(r =>
						{
							r.Intervals(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(4));
						});
					});

					// for fault
					rabbit.ReceiveEndpoint(host, "mycompany.queues.errors.newcustomers", conf =>
					{
						conf.LoadFrom(context);
					});
				});

				return busControl;
			})
			.SingleInstance()
			.As<IBusControl>()
			.As<IBus>();

			var container = builder.Build();

			var bc = container.Resolve<IBusControl>();
			bc.Start();

			Console.ReadKey();

			bc.Stop();
		}
	}
}