namespace NServiceBus.MessageSinks
{
	using System;
	using Unicast.Transport;

	public partial class MessageSinkTransport
	{
		public void Start()
		{
			this.inner.Start();
		}
		public void ChangeNumberOfWorkerThreads(int targetNumberOfWorkerThreads)
		{
			this.inner.ChangeNumberOfWorkerThreads(targetNumberOfWorkerThreads);
		}
		public void Send(TransportMessage m, string destination)
		{
			this.inner.Send(m, destination);
		}
		public int GetNumberOfPendingMessages()
		{
			return this.inner.GetNumberOfPendingMessages();
		}
		public int NumberOfWorkerThreads
		{
			get { return this.inner.NumberOfWorkerThreads; }
		}
		public string Address
		{
			get { return this.inner.Address; }
		}

		public event EventHandler<TransportMessageReceivedEventArgs> TransportMessageReceived;
		public event EventHandler StartedMessageProcessing;
		public event EventHandler FinishedMessageProcessing;
		public event EventHandler FailedMessageProcessing;
	}
}