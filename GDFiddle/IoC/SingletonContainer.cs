namespace GDFiddle.IoC
{
    /// <summary>
    /// A singleton-only container with some common container features.
    /// </summary>
    public class SingletonContainer : IDisposable
    {
        /// <summary>
        /// The list of all registrations per servicetype. An implementation registered with more than one servicetype will have multiple entries, each pointing to the same <see cref="Registration"/>.
        /// </summary>
        private readonly Dictionary<Type, Registration> _serviceRegister;
        private readonly Dictionary<Registration, object> _singletons;
        private readonly Stack<IDisposable> _disposables;

        public SingletonContainer()
        {
            _disposables = new Stack<IDisposable>();
            _serviceRegister = new Dictionary<Type, Registration>();
            _singletons = new Dictionary<Registration, object>();
        }

        public object Resolve(Type serviceType)
        {
            var context = new ResolveContext(this);
            return context.Resolve(serviceType);
        }

        public TService Resolve<TService>()
        {
            var context = new ResolveContext(this);
            return context.Resolve<TService>();
        }

        /// <summary>
        /// Resolves all services that have the given type as basetype. (without the need to register them as that service explicitly in the container)
        /// </summary>
        public IEnumerable<TServiceBase> ResolveAllWithBaseType<TServiceBase>()
        {
            var baseType = typeof(TServiceBase);
            var matchingRegistrations = _serviceRegister.Values.Distinct().Where(registration => registration.ServiceTypes.Any(st => baseType.IsAssignableFrom(st)));
            return matchingRegistrations.Select(r =>
            {
                var context = new ResolveContext(this);
                return ResolveInternal(r, context);
            }).Cast<TServiceBase>();
        }


        /// <summary>
        /// Registers an implementation type as itself. (service type == implementation type)
        /// </summary>
        public void RegisterAsSelf<TService>()
        {
            Register(typeof(TService), true);
        }

        /// <summary>
        /// Registers a factory method that will be called upon resolution, with itself as servicetype.
        /// </summary>
        public void RegisterFactoryMethod<TService>(Func<ResolveContext, TService> resolveFunc)
        {
            if (resolveFunc == null)
                throw new ArgumentNullException(nameof(resolveFunc));
            if (_serviceRegister.ContainsKey(typeof(TService)))
                throw new ArgumentException($"A service with name \'{typeof(TService).Name}\' is already registered.");

            _serviceRegister.Add(typeof(TService), CreateRegistration(resolveContext => resolveFunc(resolveContext), typeof(TService)));
        }

        /// <summary>
        /// Registers an instance with itself as servicetype.
        /// </summary>
        public void RegisterInstance<TService>(TService instance)
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));
            if (_serviceRegister.ContainsKey(typeof(TService)))
                throw new ArgumentException($"A service with name \'{typeof(TService).Name}\' is already registered.");

            _serviceRegister.Add(typeof(TService), CreateRegistration(s => instance, typeof(TService)));
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

        /// <summary>
        /// Registers an implementation type as itself. (service type == implementation type)
        /// </summary>
        public void RegisterAsSelf(Type implementation)
        {
            Register(implementation, true);
        }

        public void Register(Type implementation, bool registerSelf, params Type[] serviceTypes)
        {
            var serviceTypeList = new HashSet<Type>(serviceTypes);
            if (registerSelf)
                serviceTypeList.Add(implementation);

            foreach (var serviceType in serviceTypeList)
                if (_serviceRegister.ContainsKey(serviceType))
                    throw new ArgumentException($"A service with name \'{serviceType.Name}\' is already registered.");

            var registration = CreateRegistration(GetFactoryMethod(implementation), serviceTypeList.ToArray());

            foreach (var serviceType in serviceTypeList)
                _serviceRegister.Add(serviceType, registration);
        }

        internal object ResolveInternal(Type type, ResolveContext context)
        {
            var registration = GetRegistration(type);
            return ResolveInternal(registration, context);
        }

        private object ResolveInternal(Registration registration, ResolveContext context)
        {
            if (!_singletons.TryGetValue(registration, out var instance))
            {
                instance = registration.Resolve(context);
                if (instance is IDisposable disposable)
                    _disposables.Push(disposable);
                _singletons.Add(registration, instance);
            }
            return instance;
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
            if (_serviceRegister.TryGetValue(serviceType, out var registration))
                return registration;
            throw new InvalidOperationException($"Service {serviceType.Name} is not registered.");
        }

        public void Dispose()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
        }

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
    }
}
