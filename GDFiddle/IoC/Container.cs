namespace GDFiddle.IoC
{
    /// <summary>
    /// A singleton-only container with some common container features.
    /// </summary>
    public class Container : IDisposable
    {
        private class Registration
        {
            private readonly Func<ResolveContext, object> _resolveFunc;

            public Registration(Func<ResolveContext, object> resolveFunc, Type[] serviceTypes)
            {
                _resolveFunc = resolveFunc;
                ServiceTypes = serviceTypes;
            }

            public object Resolve(ResolveContext context)
            {
                return _resolveFunc.Invoke(context);
            }

            public Type[] ServiceTypes { get; }
        }

        private readonly Dictionary<Type, Registration> _factoriesPerServiceType;
        private readonly Dictionary<Type, object> _singletons;
        private readonly Stack<IDisposable> _disposables;

        public Container()
        {
            _disposables = new Stack<IDisposable>();
            _factoriesPerServiceType = new Dictionary<Type, Registration>();
            _singletons = new Dictionary<Type, object>();
        }
        
        public TService Resolve<TService>()
        {
            var context = new ResolveContext(this);
            return context.Resolve<TService>();
        }

        internal object ResolveInternal(Type type, ResolveContext context)
        {
            if (!_singletons.TryGetValue(type, out var instance))
            {
                var registration = GetRegistration(type);
                instance = registration.Resolve(context);
                if (instance is IDisposable disposable) 
                    _disposables.Push(disposable);
                foreach (var serviceType in registration.ServiceTypes)
                    _singletons.Add(serviceType, instance);
            }
            return instance;
        }

        public void Register<TService>()
        {
            Register(typeof(TService), true);
        }

        public void Register<TService>(Func<ResolveContext, TService> resolveFunc)
        {
            if (resolveFunc == null)
                throw new ArgumentNullException(nameof(resolveFunc));
            if (_factoriesPerServiceType.ContainsKey(typeof(TService)))
                throw new ArgumentException($"A service with name \'{typeof(TService).Name}\' is already registered.");

            _factoriesPerServiceType.Add(typeof(TService), CreateRegistration(resolveContext => resolveFunc(resolveContext), typeof(TService)));
        }

        public void RegisterInstance<TService>(TService instance)
        {
            if (instance == null) 
                throw new ArgumentNullException(nameof(instance));
            if (_factoriesPerServiceType.ContainsKey(typeof(TService)))
                throw new ArgumentException($"A service with name \'{typeof(TService).Name}\' is already registered.");

            _factoriesPerServiceType.Add(typeof(TService), CreateRegistration(s => instance, typeof(TService)));
        }

        public void Register<TService, TImplementation>(bool registerSelf = true) where TImplementation : TService
        {
            Register(typeof(TImplementation), registerSelf, typeof(TService));
        }

        public void Register<TService1, TService2, TImplementation>(bool registerSelf = true) where TImplementation : TService1, TService2
        {
            Register(typeof(TImplementation), registerSelf, typeof(TService1), typeof(TService2));
        }

        public void Register<TService1, TService2, TService3, TImplementation>(bool registerSelf = true) where TImplementation : TService1, TService2, TService3
        {
            Register(typeof(TImplementation), registerSelf, typeof(TService1), typeof(TService2), typeof(TService3));
        }

        public void Register(Type implementation)
        {
            Register(implementation, true);
        }

        public void Register(Type implementation, bool registerSelf, params Type[] serviceTypes)
        {
            var serviceTypeList = new HashSet<Type>(serviceTypes);
            if (registerSelf)
                serviceTypeList.Add(implementation);

            foreach (var serviceType in serviceTypeList)
                if (_factoriesPerServiceType.ContainsKey(serviceType))
                    throw new ArgumentException($"A service with name \'{serviceType.Name}\' is already registered.");

            var registration = CreateRegistration(GetFactoryMethod(implementation), serviceTypeList.ToArray());

            foreach (var serviceType in serviceTypeList)
                _factoriesPerServiceType.Add(serviceType, registration);
        }

        private static Registration CreateRegistration(Func<ResolveContext, object> resolveFunc, params Type[] serviceTypes) 
        {
            return new Registration(resolveFunc, serviceTypes);
        }

        private Func<ResolveContext, object> GetFactoryMethod(Type type)
        {
            var ctors = type.GetConstructors().Where(ci => ci.IsPublic).ToList();
            if (ctors.Count > 1)
                throw new InvalidOperationException($"A Service implementation type must have <= 1 public constructors. {type.Name} has {ctors.Count} constructors.");

            if (ctors.Count == 0)
            {
                var defaultCtor = type.GetConstructor(Type.EmptyTypes);
                if (defaultCtor == null)
                    throw new InvalidOperationException($"Cannot register type \"{type.FullName}\": it has no (default) constructors.");
                return (ResolveContext s) => defaultCtor.Invoke(Array.Empty<object>());
            }

            var ctor = ctors.Single();
            var parameterTypes = ctor.GetParameters().Select(pi => pi.ParameterType).ToArray();

            return (ResolveStack) =>
            {
                var arguments = parameterTypes.Select(t => ResolveStack.Clone().Resolve(t)).ToArray();
                return ctor.Invoke(arguments);
            };
        }

        private Registration GetRegistration(Type serviceType)
        {
            if (_factoriesPerServiceType.TryGetValue(serviceType, out var registration))
                return registration;
            throw new InvalidOperationException($"Service {serviceType.Name} is not registered.");
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }
    }
}
