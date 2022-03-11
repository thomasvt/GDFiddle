using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs.Querying
{
    /// <summary>
    /// Defines extra filter criteria for an ECS query.
    /// </summary>
    public class QueryCriteria
    {
        private readonly ComponentRegistry _componentRegistry;
        internal ulong BlackListMask, WhiteListMask;

        internal QueryCriteria(ComponentRegistry componentRegistry)
        {
            _componentRegistry = componentRegistry;
            BlackListMask = 0;
            WhiteListMask = 0;
        }
        
        public QueryCriteria MustNotHave<TComponent>() where TComponent : struct
        {
            var id = _componentRegistry.GetOrRegisterComponentId<TComponent>();
            BlackListMask |= (1ul << id);
            return this;
        }

        public QueryCriteria MustHave<TComponent>() where TComponent : struct
        {
            var id = _componentRegistry.GetOrRegisterComponentId<TComponent>();
            WhiteListMask |= (1ul << id);
            return this;
        }

        public QueryCriteria MustHave<TC1, TC2>() where TC1 : struct where TC2 : struct
        {
            return MustHave<TC1>().MustHave<TC2>();
        }
    }
}
