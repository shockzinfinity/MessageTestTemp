using MassTransit;
using MessageTestTemp.Messaging;
using System.Threading.Tasks;

namespace MessageTestTemp.Receiver
{
	public class RegisterCustomerFaultConsumer : IConsumer<Fault<IRegisterCustomer>>
	{
		public Task Consume(ConsumeContext<Fault<IRegisterCustomer>> context)
		{
			IRegisterCustomer originalFault = context.Message.Message;
			ExceptionInfo[] exceptions = context.Message.Exceptions;

			return Task.FromResult(originalFault);
		}
	}
}