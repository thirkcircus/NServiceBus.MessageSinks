namespace NServiceBus.MessageSinks.AutofacConfiguration
{
	using Autofac;

	public static class ContainerBuilderExtensions
	{
		public static IContainer ThreadScoped(this IContainer container)
		{
			return new ThreadScopedContainer(container);
		}
	}
}