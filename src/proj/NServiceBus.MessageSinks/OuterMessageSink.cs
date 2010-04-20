namespace NServiceBus.MessageSinks
{
	using System;

	public class OuterMessageSink : IMessageSink
	{
		private readonly Action dispose;
		private bool disposed;

		public OuterMessageSink(Action dispose)
		{
			this.dispose = dispose ?? (() => { });
		}

		public void Initialize()
		{
		}
		public void Success()
		{
		}
		public void Failure()
		{
		}
		public void Dispose()
		{
			if (!this.disposed)
				this.dispose();

			this.disposed = true;
		}
	}
}