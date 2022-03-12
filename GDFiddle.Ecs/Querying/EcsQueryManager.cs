using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs.Querying
{
    public class EcsQueryManager : IEcsQueryManager
    {
        private readonly ComponentRegistry _componentRegistry;
        private readonly ComponentSetPool[] _pools;
        private readonly int _creationTheadId;
        internal bool IsQueryInProgress;

        internal EcsQueryManager(Scene scene)
        {
            _componentRegistry = scene.ComponentRegistry;
            _pools = scene.Pools;
            _creationTheadId = Thread.CurrentThread.ManagedThreadId;
        }

        public EntityQuery<TC> DefineQuery<TC>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC : struct
        {
            ThrowIfNotMainThread();
            var componentId1 = _componentRegistry.GetOrRegisterComponentId<TC>();
            var query = new EntityQuery<TC>(this, _pools, componentId1);
            ApplyOptions(query, criteriaBuilder);
            return query;
        }

        public EntityQuery<TC1, TC2> DefineQuery<TC1, TC2>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC1 : struct
            where TC2 : struct
        {
            ThrowIfNotMainThread();
            var componentId1 = _componentRegistry.GetOrRegisterComponentId<TC1>();
            var componentId2 = _componentRegistry.GetOrRegisterComponentId<TC2>();

            var query = new EntityQuery<TC1, TC2>(this, _pools, componentId1, componentId2);
            ApplyOptions(query, criteriaBuilder);
            return query;
        }

        public EntityQuery<TC1, TC2, TC3> DefineQuery<TC1, TC2, TC3>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC1 : struct
            where TC2 : struct
            where TC3 : struct
        {
            ThrowIfNotMainThread();
            var componentId1 = _componentRegistry.GetOrRegisterComponentId<TC1>();
            var componentId2 = _componentRegistry.GetOrRegisterComponentId<TC2>();
            var componentId3 = _componentRegistry.GetOrRegisterComponentId<TC3>();
            var query = new EntityQuery<TC1, TC2, TC3>(this, _pools, componentId1, componentId2, componentId3);
            ApplyOptions(query, criteriaBuilder);
            return query;
        }

        public EntityQuery<TC1, TC2, TC3, TC4> DefineQuery<TC1, TC2, TC3, TC4>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC1 : struct
            where TC2 : struct
            where TC3 : struct
            where TC4 : struct
        {
            ThrowIfNotMainThread();
            var componentId1 = _componentRegistry.GetOrRegisterComponentId<TC1>();
            var componentId2 = _componentRegistry.GetOrRegisterComponentId<TC2>();
            var componentId3 = _componentRegistry.GetOrRegisterComponentId<TC3>();
            var componentId4 = _componentRegistry.GetOrRegisterComponentId<TC4>();
            var query = new EntityQuery<TC1, TC2, TC3, TC4>(this, _pools, componentId1, componentId2, componentId3, componentId4);
            ApplyOptions(query, criteriaBuilder);
            return query;
        }

        public EntityQuery<TC1, TC2, TC3, TC4, TC5> DefineQuery<TC1, TC2, TC3, TC4, TC5>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC1 : struct
            where TC2 : struct
            where TC3 : struct
            where TC4 : struct
            where TC5 : struct
        {
            ThrowIfNotMainThread();
            var componentId1 = _componentRegistry.GetOrRegisterComponentId<TC1>();
            var componentId2 = _componentRegistry.GetOrRegisterComponentId<TC2>();
            var componentId3 = _componentRegistry.GetOrRegisterComponentId<TC3>();
            var componentId4 = _componentRegistry.GetOrRegisterComponentId<TC4>();
            var componentId5 = _componentRegistry.GetOrRegisterComponentId<TC5>();
            var query = new EntityQuery<TC1, TC2, TC3, TC4, TC5>(this, _pools, componentId1, componentId2, componentId3, componentId4, componentId5);
            ApplyOptions(query, criteriaBuilder);
            return query;
        }

        private void ApplyOptions(EntityQuery query, Action<QueryCriteria>? criteriaBuilder)
        {
            if (criteriaBuilder == null)
                return;

            var criteria = new QueryCriteria(_componentRegistry);
            criteriaBuilder(criteria);

            query.BlackQueryMask = criteria.BlackListMask;
            query.WhiteQueryMask = criteria.WhiteListMask;
        }

        private void ThrowIfNotMainThread()
        {
            if (_creationTheadId != Thread.CurrentThread.ManagedThreadId)
                throw new Exception(
                    "Only call this method on same thread as where EcsManager was created. If called from a ECSQuery, set the query.IsParallel to false, or use the off-thread alternative methods.");
        }

        public IDisposable ClaimQueryLock(ulong queryMask)
        {
            if (IsQueryInProgress) 
                throw new Exception("Don't nest ECS queries.");
            IsQueryInProgress = true;
            return new QueryLock(this); // cannot be smarter and check querymask flag collisions because 2 queries with non-colliding mask could still be in the same archetype = pool => no deletes/relocations possible.
        }

        public void ReleaseQueryLock()
        {
            IsQueryInProgress = false;
        }
    }
}
