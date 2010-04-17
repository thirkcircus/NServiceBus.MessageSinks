namespace NServiceBus.MessageSinks.AutofacConfiguration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Autofac;
	using Autofac.Builder;
	using Unicast.Transport;

	internal class TransportConfigurationModule : Module
	{
		private readonly Func<IContainer> containerFactory;
		private readonly Action disposeContainer;

		public TransportConfigurationModule(Func<IContainer> containerFactory, Action disposeContainer)
		{
			this.containerFactory = containerFactory;
			this.disposeContainer = disposeContainer;
		}

		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder
				.RegisterCollection<IMessageSink>()
				.As<IEnumerable<IMessageSink>>()
				.FactoryScoped()
				.ExternallyOwned();
		}

		protected override void AttachToComponentRegistration(
			IContainer container, IComponentRegistration registration)
		{
			base.AttachToComponentRegistration(container, registration);

			var registeredType = registration.Descriptor.BestKnownImplementationType;
			if (registeredType.IsATransport() && typeof(MessageSinkTransport) != registeredType)
				this.OverwriteTransportRegistration(container, registeredType);
		}
		private void OverwriteTransportRegistration(IContainer container, Type registeredType)
		{
			var builder = new ContainerBuilder();
			builder
				.Register(c => this.BuildTransportSink(c, registeredType))
				.As<ITransport>()
				.SingletonScoped();

			builder.Build(container);
		}
		private MessageSinkTransport BuildTransportSink(IContext context, Type transportType)
		{
			return new MessageSinkTransport(
				context.Resolve(transportType) as ITransport,
				() => new MasterSink(this.ResolveMessageSinks().ToArray()));
		}
		private IEnumerable<IMessageSink> ResolveMessageSinks()
		{
			yield return new OuterMessageSink(this.disposeContainer);

			var sinks = this.containerFactory().Resolve<IEnumerable<IMessageSink>>();
			foreach (var sink in sinks)
				yield return sink;
		}
	}
}