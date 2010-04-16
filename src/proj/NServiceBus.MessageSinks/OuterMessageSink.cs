namespace NServiceBus.MessageSinks
{
	using System;

	public class OuterMessageSink : IMessageSink
	{
		private readonly Action initialize;
		private readonly Action dispose;

		private bool initialized;
		private bool disposed;

		public OuterMessageSink(Action dispose)
			: this(null, dispose)
		{
		}
		public OuterMessageSink(Action initialize, Action dispose)
		{
			this.initialize = initialize ?? (() => { });
			this.dispose = dispose ?? (() => { });
		}

		public void Initialize()
		{
			if (!this.initialized)
				this.initialize();

			this.initialized = true;
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