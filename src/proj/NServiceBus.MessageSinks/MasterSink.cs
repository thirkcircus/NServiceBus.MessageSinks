namespace NServiceBus.MessageSinks
{
	using System.Collections.Generic;
	using System.Linq;

	public class MasterSink : IMessageSink
	{
		private readonly IEnumerable<IMessageSink> sinks;

		private bool initialized;
		private bool succeeded;
		private bool failed;
		private bool disposed;

		public MasterSink(IEnumerable<IMessageSink> sinks)
		{
			this.sinks = sinks;
		}

		public virtual void Initialize()
		{
			if (this.initialized || this.failed || this.disposed)
				return;
			this.initialized = true;

			foreach (var sink in this.sinks)
				sink.Initialize();
		}
		public virtual void Success()
		{
			if (!this.initialized || this.succeeded || this.failed || this.disposed)
				return;
			this.succeeded = true;

			foreach (var sink in this.sinks.Reverse())
				sink.Success();

			this.Dispose();
		}
		public virtual void Failure()
		{
			if (!this.initialized || this.failed || this.disposed)
				return;
			this.failed = true;

			foreach (var sink in this.sinks.Reverse())
				sink.Failure();

			this.Dispose();
		}
		public virtual void Dispose()
		{
			if (this.disposed)
				return;

			this.disposed = true;

			foreach (var sink in this.sinks.Reverse())
				sink.Dispose();
		}
	}
}