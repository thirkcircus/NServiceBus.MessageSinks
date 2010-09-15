namespace NServiceBus.MessageSinks.AutofacConfiguration
{
	using System;
	using System.Collections.Generic;
	using Autofac;

	internal sealed class ThreadScopedContainer : IContainer
	{
		[ThreadStatic]
		private static IContainer threadScoped;
		private readonly IContainer parent;

		public ThreadScopedContainer(IContainer parentContainer)
		{
			if (ReferenceEquals(parentContainer, threadScoped))
				parentContainer = parentContainer.OuterContainer;

			this.parent = parentContainer;
		}
		public void Dispose()
		{
			if (threadScoped == null)
				return;

			threadScoped.Dispose();
			threadScoped = null;
		}

		private IContainer Decorated
		{
			get { return threadScoped ?? (threadScoped = this.parent.CreateInnerContainer()); }
		}

		public void RegisterComponent(IComponentRegistration registration)
		{
			this.parent.RegisterComponent(registration);
		}

		#region - Unused -

		public event EventHandler<ComponentRegisteredEventArgs> ComponentRegistered
		{
			add { this.Decorated.ComponentRegistered += value; }
			remove { this.Decorated.ComponentRegistered -= value; }
		}
		public TService Resolve<TService>(params Parameter[] parameters)
		{
			return this.Decorated.Resolve<TService>(parameters);
		}
		public TService Resolve<TService>(string serviceName, params Parameter[] parameters)
		{
			return this.Decorated.Resolve<TService>(serviceName, parameters);
		}
		public object Resolve(Type serviceType, params Parameter[] parameters)
		{
			return this.Decorated.Resolve(serviceType, parameters);
		}
		public object Resolve(string serviceName, params Parameter[] parameters)
		{
			return this.Decorated.Resolve(serviceName, parameters);
		}
		public object Resolve(Service service, params Parameter[] parameters)
		{
			return this.Decorated.Resolve(service, parameters);
		}
		public bool TryResolve<TService>(out TService instance, params Parameter[] parameters)
		{
			return this.Decorated.TryResolve(out instance, parameters);
		}
		public bool TryResolve(Type serviceType, out object instance, params Parameter[] parameters)
		{
			return this.Decorated.TryResolve(serviceType, out instance, parameters);
		}
		public bool TryResolve(string componentName, out object instance, params Parameter[] parameters)
		{
			return this.Decorated.TryResolve(componentName, out instance, parameters);
		}
		public bool TryResolve(Service service, out object instance, params Parameter[] parameters)
		{
			return this.Decorated.TryResolve(service, out instance, parameters);
		}
		public TService ResolveOptional<TService>(params Parameter[] parameters)
		{
			return this.Decorated.ResolveOptional<TService>(parameters);
		}
		public TService Resolve<TService>(IEnumerable<Parameter> parameters)
		{
			return this.Decorated.Resolve<TService>(parameters);
		}
		public TService Resolve<TService>(string serviceName, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.Resolve<TService>(serviceName, parameters);
		}
		public object Resolve(Type serviceType, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.Resolve(serviceType, parameters);
		}
		public object Resolve(string serviceName, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.Resolve(serviceName, parameters);
		}
		public object Resolve(Service service, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.Resolve(service, parameters);
		}
		public bool TryResolve<TService>(out TService instance, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.TryResolve(out instance, parameters);
		}
		public bool TryResolve(Type serviceType, out object instance, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.TryResolve(serviceType, out instance, parameters);
		}
		public bool TryResolve(string componentName, out object instance, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.TryResolve(componentName, out instance, parameters);
		}
		public bool TryResolve(Service service, out object instance, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.TryResolve(service, out instance, parameters);
		}
		public TService ResolveOptional<TService>(IEnumerable<Parameter> parameters)
		{
			return this.Decorated.ResolveOptional<TService>(parameters);
		}
		public TService ResolveOptional<TService>(string serviceName, IEnumerable<Parameter> parameters)
		{
			return this.Decorated.ResolveOptional<TService>(serviceName, parameters);
		}
		public TService ResolveOptional<TService>(string serviceName, params Parameter[] parameters)
		{
			return this.Decorated.ResolveOptional<TService>(serviceName, parameters);
		}
		public bool IsRegistered(Type serviceType)
		{
			return this.Decorated.IsRegistered(serviceType);
		}
		public bool IsRegistered(string serviceName)
		{
			return this.Decorated.IsRegistered(serviceName);
		}
		public bool IsRegistered(Service service)
		{
			return this.Decorated.IsRegistered(service);
		}
		public bool IsRegistered<TService>()
		{
			return this.Decorated.IsRegistered<TService>();
		}
		public T InjectProperties<T>(T instance)
		{
			return this.Decorated.InjectProperties(instance);
		}
		public T InjectUnsetProperties<T>(T instance)
		{
			return this.Decorated.InjectUnsetProperties(instance);
		}
		public IContainer CreateInnerContainer()
		{
			return this.Decorated.CreateInnerContainer();
		}
		public void AddRegistrationSource(IRegistrationSource source)
		{
			this.Decorated.AddRegistrationSource(source);
		}
		public bool TryGetDefaultRegistrationFor(Service service, out IComponentRegistration registration)
		{
			return this.Decorated.TryGetDefaultRegistrationFor(service, out registration);
		}
		public void TagWith<T>(T tag)
		{
			this.Decorated.TagWith(tag);
		}
		public IDisposer Disposer
		{
			get { return this.Decorated.Disposer; }
		}
		public IContainer OuterContainer
		{
			get { return this.Decorated.OuterContainer; }
		}
		public IEnumerable<IComponentRegistration> ComponentRegistrations
		{
			get { return this.Decorated.ComponentRegistrations; }
		}

		#endregion
	}
}