using System;
using System.Collections.Generic;
using System.Linq;

namespace gw2lav {

	public class IocContainer {

		private readonly Dictionary<Type, Type> types = new Dictionary<Type, Type>();
		private Dictionary<Type, object> instances = new Dictionary<Type, object>();

		public void Register<TInterface, TImplementation>() where TImplementation : TInterface {
			types[typeof(TInterface)] = typeof(TImplementation);
		}

		public T Create<T>() {
			return (T)Create(typeof(T));
		}

		private object Create(Type type) {
			var concreteType = types[type];

			// get cached instance if available
			if (instances.ContainsKey(concreteType)) {
				return instances[concreteType];
			}

			// find a default constructor using reflection
			var defaultConstructor = concreteType.GetConstructors()[0];
			// verify if the default constructor requires params
			var defaultParams = defaultConstructor.GetParameters();
			// instantiate all constructor parameters using recursion
			var parameters = defaultParams.Select(param => Create(param.ParameterType)).ToArray();

			// create new instance and cache it
			object instance = defaultConstructor.Invoke(parameters);
			instances.Add(concreteType, instance);
			return instance;
		}

	}

}
