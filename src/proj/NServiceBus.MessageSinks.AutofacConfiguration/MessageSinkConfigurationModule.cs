namespace NServiceBus.MessageSinks.AutofacConfiguration
{
	using System;
	using System.Collections.Generic;
	using Autofac;
	using Autofac.Builder;
	using Autofac.Modules;
	using Unicast.Transport;

	public class MessageSinkConfigurationModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule(new ImplicitCollectionSupportModule());

			builder
				.Register(c => new ThreadScopedContainer(c.Resolve<IContainer>()))
				.As<ThreadScopedContainer>()
				.SingletonScoped()
				.ExternallyOwned();

			builder
				.Register(c => new MasterSink(c.Resolve<IEnumerable<IMessageSink>>()))
				.As<MasterSink>()
				.ContainerScoped()
				.ExternallyOwned();

			builder
				.Register(c => new OuterMessageSink(c.Resolve<ThreadScopedContainer>().Dispose))
				.As<IMessageSink>()
				.ContainerScoped()
				.ExternallyOwned();
		}

		protected override void AttachToComponentRegistration(
			IContainer container, IComponentRegistration registration)
		{
			base.AttachToComponentRegistration(container, registration);

			var registeredType = registration.Descriptor.BestKnownImplementationType;
			if (IsConfiguredTransport(registeredType))
				DecorateTransport(container, registeredType);
		}
		private static bool IsConfiguredTransport(Type typeToEvaluate)
		{
			return typeof(ITransport).IsAssignableFrom(typeToEvaluate)
			       && typeof(MessageSinkTransport) != typeToEvaluate
			       && !typeToEvaluate.IsInterface;
		}
		private static void DecorateTransport(IContainer container, Type transportType)
		{
			var builder = new ContainerBuilder();
			builder.Register(c => GetDecoratedTransport(c, transportType)).As<ITransport>().SingletonScoped();
			builder.Build(container);
		}
		private static ITransport GetDecoratedTransport(IContext context, Type transportType)
		{
			var container = context.Resolve<ThreadScopedContainer>();
			return new MessageSinkTransport(
				context.Resolve(transportType) as ITransport, () => container.Resolve<MasterSink>());
		}
	}
}