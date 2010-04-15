namespace NServiceBus.MessageSinks
{
	using System;
	using System.Collections.Generic;
	using Unicast.Transport;

	public partial class MessageSinkTransport : ITransport
	{
		[ThreadStatic]
		private static ProcessingStatus status;

		private readonly ITransport inner;
		private readonly Func<IEnumerable<IMessageSink>> messageSinks;

		public MessageSinkTransport(ITransport inner, Func<IEnumerable<IMessageSink>> messageSinks)
		{
			this.inner = inner;
			this.messageSinks = messageSinks;
			this.Subscribe();
		}
		public void Dispose()
		{
			this.inner.Dispose();
			this.Unsubscribe();
		}

		private void Subscribe()
		{
			this.inner.StartedMessageProcessing += this.OnStartedProcessing;
			this.inner.TransportMessageReceived += this.OnMessageReceived;
			this.inner.FinishedMessageProcessing += this.OnFinishedProcessing;
			this.inner.FailedMessageProcessing += this.OnFailedProcessing;
		}
		private void Unsubscribe()
		{
			this.inner.TransportMessageReceived -= this.OnMessageReceived;
			this.inner.StartedMessageProcessing -= this.OnStartedProcessing;
			this.inner.FinishedMessageProcessing -= this.OnFinishedProcessing;
			this.inner.FailedMessageProcessing -= this.OnFailedProcessing;
		}

		private void OnStartedProcessing(object sender, EventArgs e)
		{
			status = new ProcessingStatus(this.messageSinks());
			status.Initialize();
			this.NotifyListeners(this.StartedMessageProcessing, () => status.Fail());
		}
		private void OnFailedProcessing(object sender, EventArgs e)
		{
			this.NotifyListeners(this.FailedMessageProcessing, () => status.Fail());
			status.Fail();
		}
		private void OnFinishedProcessing(object sender, EventArgs e)
		{
			status.Succeed();
			this.NotifyListeners(this.FinishedMessageProcessing, () => status.Teardown());
		}
		private void OnMessageReceived(object sender, TransportMessageReceivedEventArgs e)
		{
			var handlers = this.TransportMessageReceived;
			if (handlers == null)
				return;

			try
			{
				handlers(this, e);
			}
			catch (Exception)
			{
				status.Fail();
				throw;
			}
		}
		private void NotifyListeners(EventHandler handlers, Action onException)
		{
			if (handlers == null)
				return;

			try
			{
				handlers(this, EventArgs.Empty);
			}
			catch (Exception)
			{
				onException();
				throw;
			}
		}

		public void ReceiveMessageLater(TransportMessage m)
		{
			if (status != null)
				status.Succeed();

			this.inner.ReceiveMessageLater(m);
		}
		public void AbortHandlingCurrentMessage()
		{
			status.Fail();
			this.inner.AbortHandlingCurrentMessage();
		}
	}
}