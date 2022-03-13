
namespace GDFiddle.IoC
{
    public class ResolveContext
    {
        private readonly SingletonContainer _singletonContainer;
        private readonly List<Type> _stack;

        public ResolveContext(SingletonContainer singletonContainer)
        {
            _singletonContainer = singletonContainer;
            _stack = new List<Type>();
        }

        private ResolveContext(SingletonContainer singletonContainer, IEnumerable<Type> other)
        {
            _singletonContainer = singletonContainer;
            _stack = new List<Type>(other);
        }

        internal ResolveContext Clone()
        {
            return new ResolveContext(_singletonContainer, _stack);
        }

        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        public object Resolve(Type type)
        {
            _stack.Add(type);
            if (_stack.Count(t => t == type) > 1)
                throw new Exception($"Circular dependency detected while resolving \"{_stack.First().FullName}\" at dependency \"{type.FullName}\". Full chain: {string.Join(" -> ", _stack.Select(s => s.Name))}");

            return _singletonContainer.ResolveInternal(type, this);
        }
    }
}
