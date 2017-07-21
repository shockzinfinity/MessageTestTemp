using System;

namespace MessageTestTemp.Messaging
{
	public interface IRegisterCustomer
	{
		Guid Id { get; }
		DateTime RegisteredUtc { get; }
		int Type { get; }
		string Name { get; }
		bool Preferred { get; }
		decimal DefaultDiscount { get; }
		string Address { get; }
	}
}