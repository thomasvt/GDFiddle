using System.Collections;
using GDFiddle.Ecs.Bulk;
using GDFiddle.Ecs.ComponentStore;
using GDFiddle.Ecs.Querying;
using GDFiddle.Ecs.Scheduling;
using GDFiddle.Ecs.Systems;

namespace GDFiddle.Ecs
{
    /// <summary>
    /// The main accesspoint to all ECS functionality. Use the sub-managers in the properties for specialized functionality such as scheduling, bulk operations and querying.
    /// </summary>
    public partial class Scene
    : IScene
    {
        public const int MaxComponentTypeCount = 64;

        private readonly EcsConfig _config;
        internal readonly ComponentRegistry ComponentRegistry;
        internal readonly ComponentSetPool[] Pools;
        private readonly Dictionary<ulong, int> _poolIdxByArchetype;
        internal readonly EntityIndex EntityIndex;
        private readonly int _creationTheadId;
        private readonly EcsQueryManager _querying;
        private readonly EcsBulkManager _bulk;
        private readonly Scheduler _afterFrame;
        private readonly Dictionary<byte, object> _componentRemoveCallbacks;
        private ulong _componentRemoveCallbackMask;

        public Scene(EcsConfig config)
        {
            _config = config;
            _componentRemoveCallbacks = new Dictionary<byte, object>(MaxComponentTypeCount);
            _poolIdxByArchetype = new Dictionary<ulong, int>(config.MaxArchetypeCount);
            Pools = new ComponentSetPool[config.MaxArchetypeCount];
            ComponentRegistry = new ComponentRegistry(MaxComponentTypeCount);
            EntityIndex = new EntityIndex(config.InitialEntityCapacity);
            _creationTheadId = Thread.CurrentThread.ManagedThreadId;
            _querying = new EcsQueryManager(this);
            _bulk = new EcsBulkManager(this);
            _afterFrame = new Scheduler(this);
        }

        /// <summary>
        /// Creates a new entity of the given <see cref="Archetype"/>. If called from within a query iteration, the new entity may be visited in the same iteration if its archetype matches it. Although this is not guaranteed.
        /// </summary>
        public EntityId CreateEntity(Archetype archetype)
        {
            ThrowIfNotMainThread();
            var poolIdx = GetOrCreatePool(archetype);
            var pool = Pools[poolIdx];
            var entityId = EntityIndex.Alloc();
            var idxInPool = pool.Alloc(entityId);
            EntityIndex.SetEntityInfo(entityId, poolIdx, idxInPool);
            return entityId;
        }

        /// <summary>
        /// Remove the entity immediately. You can not use this during a Query or off-thread. Use AfterFrame scheduling in that case.
        /// </summary>
        public void Remove(EntityId entityId)
        {
            ThrowIfNotSafe();
            RemoveInternal(entityId);
        }

        internal void RemoveInternal(EntityId entity)
        {
            if (!EntityIndex.Records.TryGetValue(entity.Id, out var entityIndexRecord))
                return; // job done
            var pool = Pools[entityIndexRecord.PoolIdx];
            if ((pool.Archetype.Mask & _componentRemoveCallbackMask) != 0)
                pool.TriggerComponentCallbacks(_componentRemoveCallbacks, entity, entityIndexRecord.IdxInPool);
            pool.Free(entityIndexRecord.IdxInPool);
            EntityIndex.Free(entity);
        }

        /// <summary>
        /// Removes all entities, but not known Archetypes and Components.
        /// </summary>
        public void ClearScene()
        {
            ThrowIfNotSafe();
            foreach (var pool in Pools)
            {
                pool?.Clear();
            }

            EntityIndex.Clear();
        }

        public void RegisterComponentRemoveCallback<TComponent>(EntityCallback1<TComponent> callback) where TComponent : struct
        {
            var componentId = ComponentRegistry.GetOrRegisterComponentId<TComponent>();
            if (!_componentRemoveCallbacks.TryAdd(componentId, callback))
                throw new Exception("Only 1 callback per component type can be registered.");
            _componentRemoveCallbackMask |= 1ul << componentId;
        }

        public IEnumerable GetComponents(EntityId entityId)
        {
            var entityIndexRecord = EntityIndex.Records[entityId];
            var pool = Pools[entityIndexRecord.PoolIdx];
            foreach (var componentId in pool.Archetype.GetComponentIds())
            {
                var componentArray = pool.ComponentArraysPerComponentId[componentId];
                yield return componentArray.GetByIdx(entityIndexRecord.IdxInPool);
            }
        }

        public void AddComponent<TComponent>(EntityId entityId) where TComponent : struct
        {
            ThrowIfNotSafe();
            AddComponentInternal<TComponent>(entityId);
        }

        public void AddComponent<TComponent>(EntityId entityId, TComponent value) where TComponent : struct
        {
            ThrowIfNotSafe();
            AddComponentInternal<TComponent>(entityId);
            SetComponent(entityId, value);
        }

        internal void AddComponentInternal<TComponent>(EntityId entityId) where TComponent : struct
        {
            var entityIndexRecord = EntityIndex.Records[entityId.Id];
            var oldPool = Pools[entityIndexRecord.PoolIdx];
            var newArchetype = oldPool.Archetype.With<TComponent>();
            RelocateEntity(entityId, newArchetype, oldPool, entityIndexRecord.IdxInPool);
        }

        public void RemoveComponent<TComponent>(EntityId entityId) where TComponent : struct
        {
            ThrowIfNotSafe();
            RemoveComponentInternal<TComponent>(entityId);
        }

        internal void RemoveComponentInternal<TComponent>(EntityId entityId) where TComponent : struct
        {
            var entityIndexRecord = EntityIndex.Records[entityId.Id];
            var oldPool = Pools[entityIndexRecord.PoolIdx];
            var newArchetype = oldPool.Archetype.Without<TComponent>();

            var componentId = ComponentRegistry.GetComponentId<TComponent>();
            if (((1ul << componentId) & _componentRemoveCallbackMask) != 0)
                oldPool.ComponentArraysPerComponentId[componentId].TriggerCallback(_componentRemoveCallbacks[componentId], entityId, entityIndexRecord.IdxInPool);
            RelocateEntity(entityId, newArchetype, oldPool, entityIndexRecord.IdxInPool);
        }

        public void SetComponent<TComponent>(EntityId entityId, in TComponent component) where TComponent : struct
        {
            var entityIndexRecord = EntityIndex.Records[entityId];
            if (!ComponentRegistry.ComponentIdsByType.TryGetValue(typeof(TComponent), out var componentId))
                throw new Exception($"Entity \"{entityId}\" does not have a {typeof(TComponent).Name}. It is not even a registered component.");
            var pool = Pools[entityIndexRecord.PoolIdx];
            if (!pool.Archetype.Contains(componentId))
                throw new Exception($"Entity \"{entityId}\" does not have a {typeof(TComponent).Name}.");
            var componentArray = (ComponentArray<TComponent>)pool.ComponentArraysPerComponentId[componentId];
            componentArray.Records[entityIndexRecord.IdxInPool] = component;
        }

        public ref TComponent GetComponentRef<TComponent>(EntityId entityId) where TComponent : struct
        {
            var entityIndexRecord = EntityIndex.Records[entityId];
            if (!ComponentRegistry.ComponentIdsByType.TryGetValue(typeof(TComponent), out var componentId))
                throw new Exception($"Entity \"{entityId}\" does not have a {typeof(TComponent).Name}.");
            var pool = Pools[entityIndexRecord.PoolIdx];
            if (!pool.Archetype.Contains(componentId))
                throw new Exception($"Entity \"{entityId}\" does not have a {typeof(TComponent).Name}.");
            var componentArray = (ComponentArray<TComponent>)pool.ComponentArraysPerComponentId[componentId];
            return ref componentArray.Records[entityIndexRecord.IdxInPool];
        }

        public bool HasComponent<TComponent>(EntityId entityId) where TComponent : struct
        {
            var entityIndexRecord = EntityIndex.Records[entityId.Id];
            var pool = Pools[entityIndexRecord.PoolIdx];
            return ComponentRegistry.ComponentIdsByType.TryGetValue(typeof(TComponent), out var componentId) && pool.Archetype.Contains(componentId);
        }

        public Archetype GetArchetype(EntityId entityId)
        {
            var entityIndexRecord = EntityIndex.Records[entityId.Id];
            return Pools[entityIndexRecord.PoolIdx].Archetype;
        }

        public bool Exists(EntityId entityId)
        {
            return EntityIndex.Records.ContainsKey(entityId.Id);
        }

        public Archetype DefineArchetype()
        {
            return new Archetype(ComponentRegistry);
        }

        /// <summary>
        /// Physically moves the components of one entity to the pool of its newArchetype.
        /// </summary>
        internal void RelocateEntity(EntityId entityId, Archetype newArchetype, ComponentSetPool oldPool, int oldIdxInPool)
        {
            var newPoolIdx = GetOrCreatePool(newArchetype);
            var newPool = Pools[newPoolIdx];
            var idxInNewPool = newPool.Alloc(entityId);
            foreach (var componentId in oldPool.Archetype.GetComponentIds())
            {
                if (!newArchetype.Contains(componentId))
                    continue; // only move components that are still there in the new archetype. (and new components will have default values)
                var oldComponentArray = oldPool.ComponentArraysPerComponentId[componentId];
                var newComponentArray = newPool.ComponentArraysPerComponentId[componentId];
                oldComponentArray.Relocate(oldIdxInPool, newComponentArray, idxInNewPool);
            }

            oldPool.Free(oldIdxInPool);
            EntityIndex.SetEntityInfo(entityId, newPoolIdx, idxInNewPool);
        }

        internal int GetOrCreatePool(Archetype archetype)
        {
            var mask = archetype.Mask;
            if (_poolIdxByArchetype.TryGetValue(mask, out var poolIdx))
                return poolIdx;

            poolIdx = _poolIdxByArchetype.Count;
            if (poolIdx == _config.MaxArchetypeCount)
                throw new Exception("MaxArchetypeCount exceeded.");
            _poolIdxByArchetype.Add(mask, poolIdx);
            var pool = Pools[poolIdx] = new ComponentSetPool(archetype, 4);
            pool.EntityMoved += (sender, args) => EntityIndex.SetEntityInfo(args.EntityId, poolIdx, args.NewIdx);
            return poolIdx;
        }

        internal void ThrowIfNotSafe()
        {
            if (_querying.IsQueryInProgress)
                throw new Exception($"You cannot do this while visiting Query results. Schedule your action using {nameof(AfterFrame)}.");
            if (_creationTheadId != Thread.CurrentThread.ManagedThreadId)
                throw new NotMainThreadException(
                    $"Only call this method on the same thread as where {nameof(Scene)} was constructed. Use {nameof(AfterFrame)} for scheduling options.");
        }

        internal void ThrowIfNotMainThread()
        {
            if (_creationTheadId != Thread.CurrentThread.ManagedThreadId)
                throw new NotMainThreadException(
                    $"Only call this method on the same thread as where {nameof(Scene)} was constructed. Use {nameof(AfterFrame)} for scheduling options.");
        }

        public IEcsQueryManager Querying => _querying;

        public IEcsBulkManager Bulk => _bulk;

        public IScheduler AfterFrame => _afterFrame;

        public int EntityCount => EntityIndex.EntityCount;
        public int ComponentCount => ComponentRegistry.ComponentCount;
    }
}

