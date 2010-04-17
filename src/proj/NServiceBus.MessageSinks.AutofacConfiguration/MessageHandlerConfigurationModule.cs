namespace NServiceBus.MessageSinks.AutofacConfiguration
{
	using System;
	using Autofac;
	using Autofac.Builder;

	internal class MessageHandlerConfigurationModule : Module
	{
		private readonly Func<IContainer> containerFactory;

		[ThreadStatic]
		private static bool alreadyPrepared;

		public MessageHandlerConfigurationModule(Func<IContainer> containerFactory)
		{
			this.containerFactory = containerFactory;
		}

		protected override void AttachToComponentRegistration(
			IContainer container, IComponentRegistration registration)
		{
			base.AttachToComponentRegistration(container, registration);

			if (registration.Descriptor.BestKnownImplementationType.IsAMessageHandler())
				registration.Preparing += this.PrepareEventHandler;
		}
		private void PrepareEventHandler(object sender, PreparingEventArgs args)
		{
			if (alreadyPrepared)
				return;

			alreadyPrepared = true;

			var handlerType = args.Component.Descriptor.BestKnownImplementationType;

			// this causes a recursive call, so we flag alreadyPrepared
			args.Instance = this.containerFactory().Resolve(handlerType);

			alreadyPrepared = false;
		}
	}
}