namespace NServiceBus.MessageSinks
{
	using System;

	public interface IMessageSink : IDisposable
	{
		void Initialize();
		void Success();
		void Failure();
	}
}