namespace NServiceBus.MessageSinks
{
	using System;
	using Unicast.Transport;

	public class MessageSinkTransport : ITransport
	{
		[ThreadStatic]
		private static IMessageSink master;

		private readonly ITransport transport;
		private readonly Func<MasterSink> messageSinkFactory;

		public MessageSinkTransport(ITransport transport, Func<MasterSink> messageSinkFactory)
		{
			this.transport = transport;
			this.messageSinkFactory = messageSinkFactory;
			this.Subscribe();
		}
		public void Dispose()
		{
			this.transport.Dispose();
			this.Unsubscribe();
		}

		private void Subscribe()
		{
			this.transport.StartedMessageProcessing += this.OnStartedProcessing;
			this.transport.TransportMessageReceived += this.OnMessageReceived;
			this.transport.FinishedMessageProcessing += this.OnFinishedProcessing;
			this.transport.FailedMessageProcessing += this.OnFailedProcessing;
		}
		private void Unsubscribe()
		{
			this.transport.TransportMessageReceived -= this.OnMessageReceived;
			this.transport.StartedMessageProcessing -= this.OnStartedProcessing;
			this.transport.FinishedMessageProcessing -= this.OnFinishedProcessing;
			this.transport.FailedMessageProcessing -= this.OnFailedProcessing;
		}

		private IMessageSink RootSink
		{
			get
			{
				if (master == null)
					master = this.messageSinkFactory();

				return master;
			}
		}

		private void OnStartedProcessing(object sender, EventArgs args)
		{
			this.RootSink.Initialize();
			this.OnTransportEvent(this.StartedMessageProcessing, () => this.RootSink.Failure());
		}
		private void OnFailedProcessing(object sender, EventArgs args)
		{
			this.OnTransportEvent(this.FailedMessageProcessing, () => { });
			this.RootSink.Failure();
		}
		private void OnFinishedProcessing(object sender, EventArgs args)
		{
			this.RootSink.Success();
			this.OnTransportEvent(this.FinishedMessageProcessing, () => this.RootSink.Dispose());
		}
		private void OnMessageReceived(object sender, TransportMessageReceivedEventArgs args)
		{
			this.OnTransportEvent((s, e) =>
			{
				var observers = this.TransportMessageReceived;
				if (observers != null)
					observers(this, args);
			}, () => this.RootSink.Failure());
		}

		private void OnTransportEvent(EventHandler observers, Action onException)
		{
			if (observers == null)
				return;

			try
			{
				observers(this, EventArgs.Empty);
			}
			catch
			{
				onException();
				throw;
			}
		}

		public void ReceiveMessageLater(TransportMessage m)
		{
			if (this.RootSink != null)
				this.RootSink.Success();

			this.transport.ReceiveMessageLater(m);
		}
		public void AbortHandlingCurrentMessage()
		{
			this.RootSink.Failure();
			this.transport.AbortHandlingCurrentMessage();
		}

		public void Start()
		{
			this.transport.Start();
		}
		public void ChangeNumberOfWorkerThreads(int targetNumberOfWorkerThreads)
		{
			this.transport.ChangeNumberOfWorkerThreads(targetNumberOfWorkerThreads);
		}
		public void Send(TransportMessage m, string destination)
		{
			this.transport.Send(m, destination);
		}
		public int GetNumberOfPendingMessages()
		{
			return this.transport.GetNumberOfPendingMessages();
		}
		public int NumberOfWorkerThreads
		{
			get { return this.transport.NumberOfWorkerThreads; }
		}
		public string Address
		{
			get { return this.transport.Address; }
		}

		public event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;
		public event EventHandler StartedMessageProcessing;
		public event EventHandler FinishedMessageProcessing;
		public event EventHandler FailedMessageProcessing;
	}
}