namespace NServiceBus.MessageSinks
{
	using System.Collections.Generic;
	using System.Linq;

	internal class ProcessingStatus
	{
		private readonly IEnumerable<IMessageSink> sinks;

		private bool initialized;
		private bool succeeded;
		private bool failed;
		private bool disposed;

		public ProcessingStatus(IEnumerable<IMessageSink> sinks)
		{
			this.sinks = sinks;
		}

		public void Initialize()
		{
			if (this.initialized || this.failed || this.disposed)
				return;
			this.initialized = true;

			foreach (var sink in this.sinks)
				sink.Initialize();
		}
		public void Succeed()
		{
			if (!this.initialized || this.succeeded || this.failed || this.disposed)
				return;
			this.succeeded = true;

			foreach (var sink in this.sinks.Reverse())
				sink.Success();

			this.Teardown();
		}
		public void Fail()
		{
			if (!this.initialized || this.failed || this.disposed)
				return;
			this.failed = true;

			foreach (var sink in this.sinks.Reverse())
				sink.Failure();

			this.Teardown();
		}
		public void Teardown()
		{
			if (this.disposed)
				return;

			this.disposed = true;

			foreach (var sink in this.sinks.Reverse())
				sink.Dispose();
		}
	}
}