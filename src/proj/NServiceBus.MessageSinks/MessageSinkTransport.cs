namespace NServiceBus.MessageSinks
{
	using System;
	using Unicast.Transport;

	public class MessageSinkTransport : ITransport
	{
		private readonly ITransport transport;
		private readonly Func<IMessageSink> masterSinkFactory;

		[ThreadStatic]
		private static IMessageSink masterSink;
		private IMessageSink MasterSink
		{
			get { return masterSink = masterSink ?? this.masterSinkFactory(); }
		}

		public MessageSinkTransport(ITransport transport, Func<IMessageSink> masterSinkFactory)
		{
			this.transport = transport;
			this.masterSinkFactory = masterSinkFactory;
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
			this.transport.StartedMessageProcessing -= this.OnStartedProcessing;
			this.transport.TransportMessageReceived -= this.OnMessageReceived;
			this.transport.FinishedMessageProcessing -= this.OnFinishedProcessing;
			this.transport.FailedMessageProcessing -= this.OnFailedProcessing;
		}

		private void OnStartedProcessing(object sender, EventArgs args)
		{
			masterSink = null;
			this.MasterSink.Initialize();
			this.OnTransportEvent(this.StartedMessageProcessing, this.MasterSink.Failure);
		}
		private void OnMessageReceived(object sender, TransportMessageReceivedEventArgs args)
		{
			EventHandler handler = (s, e) =>
			{
				var observers = this.TransportMessageReceived;
				if (observers != null)
					observers(this, args);
			};

			this.OnTransportEvent(handler, this.MasterSink.Failure);
		}
		private void OnFinishedProcessing(object sender, EventArgs args)
		{
			this.OnTransportEvent(this.FinishedMessageProcessing, this.MasterSink.Failure);
			this.MasterSink.Success();
		}
		private void OnFailedProcessing(object sender, EventArgs args)
		{
			this.OnTransportEvent(this.FailedMessageProcessing, () => { });
			this.MasterSink.Failure();
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
			this.MasterSink.Initialize();
			this.transport.ReceiveMessageLater(m);
			this.MasterSink.Success();
		}
		public void AbortHandlingCurrentMessage()
		{
			this.MasterSink.Failure();
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