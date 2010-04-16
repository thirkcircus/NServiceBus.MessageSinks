namespace NServiceBus.MessageSinks.AutofacConfiguration
{
	using System;
	using Autofac;
	using Autofac.Builder;

	public class ThreadSpecificConfigurationModule : Module
	{
		[ThreadStatic]
		private static IContainer threadSpecificContainer;
		private IContainer rootContainer;

		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			builder.RegisterModule(this.GetTransportConfiguration());
			builder.RegisterModule(this.GetHandlerConfiguration());
		}
		private Module GetTransportConfiguration()
		{
			return new TransportConfigurationModule(
				this.GetThreadSpecificContainer, DisposeThreadSpecificContainer);
		}
		private Module GetHandlerConfiguration()
		{
			return new MessageHandlerConfigurationModule(this.GetThreadSpecificContainer);
		}

		private IContainer GetThreadSpecificContainer()
		{
			threadSpecificContainer = threadSpecificContainer ?? this.rootContainer.CreateInnerContainer();
			return threadSpecificContainer;
		}
		private static void DisposeThreadSpecificContainer()
		{
			if (threadSpecificContainer == null)
				return;

			threadSpecificContainer.Dispose();
			threadSpecificContainer = null;
		}

		protected override void AttachToComponentRegistration(IContainer container, IComponentRegistration registration)
		{
			base.AttachToComponentRegistration(container, registration);

			if (this.rootContainer == null)
				this.rootContainer = this.GetRootContainer(container);
		}
		private IContainer GetRootContainer(IContainer container)
		{
			if (container.OuterContainer == null)
				return container;

			return this.GetRootContainer(container.OuterContainer);
		}
	}
}