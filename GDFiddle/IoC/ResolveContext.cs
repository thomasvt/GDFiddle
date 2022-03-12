
namespace GDFiddle.IoC
{
    public class ResolveContext
    {
        private readonly Container _container;
        private readonly List<Type> _stack;

        public ResolveContext(Container container)
        {
            _container = container;
            _stack = new List<Type>();
        }

        private ResolveContext(Container container, IEnumerable<Type> other)
        {
            _container = container;
            _stack = new List<Type>(other);
        }

        internal ResolveContext Clone()
        {
            return new ResolveContext(_container, _stack);
        }

        public T Resolve<T>()
        {
            return (T) Resolve(typeof(T));
        }

        internal object Resolve(Type type)
        {
            _stack.Add(type);
            if (_stack.Count(t => t == type) > 1)
                throw new Exception($"Circular dependency detected while resolving \"{_stack.First().FullName}\" at dependency \"{type.FullName}\". Full chain: {string.Join(" -> ", _stack.Select(s => s.Name))}");

            return _container.ResolveInternal(type, this);
        }
    }
}
