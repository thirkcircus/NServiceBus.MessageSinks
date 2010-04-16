namespace NServiceBus.MessageSinks.AutofacConfiguration
{
	using System;
	using System.Collections.Generic;
	using Unicast.Transport;

	internal static class TypeExtensions
	{
		private static readonly Type HandlerInterfaceType = typeof(IMessageHandler<>);
		private static readonly Type TransportInterfaceType = typeof(ITransport);
		private static readonly ICollection<Type> HandlerCache = new HashSet<Type>();

		public static bool IsAMessageHandler(this Type evaluate)
		{
			if (!HandlerCache.Contains(evaluate))
			{
				if (!evaluate.ImplementsInterface(HandlerInterfaceType))
					return false;

				lock (HandlerCache)
					HandlerCache.Add(@evaluate);
			}

			return true;
		}

		public static bool IsATransport(this Type evaluate)
		{
			return evaluate.ImplementsInterface(TransportInterfaceType);
		}

		private static bool ImplementsInterface(this Type evaluate, Type @interface)
		{
			if (!@interface.IsGenericType && @interface.IsAssignableFrom(@evaluate))
				return true;

			if (!@interface.IsGenericType)
				return false;

			var interfaceType = evaluate.GetInterface(@interface.FullName);
			if (interfaceType == null)
				return false;

			return @interface.IsAssignableFrom(interfaceType.GetGenericTypeDefinition());
		}
	}
}