using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs.Bulk
{
    internal class EcsBulkManager : IEcsBulkManager
    {
        private readonly EcsScene _sceneManager;

        public EcsBulkManager(EcsScene sceneManager)
        {
            _sceneManager = sceneManager;
        }

        /// <summary>
        /// Creates multiple entities of the archetype in one go. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        public EntityId CreateEntities(Archetype archetype, int count)
        {
            _sceneManager.ThrowIfNotSafe();
            CreateEntitiesInternal(archetype, count, out var firstEntityId, out _);
            return firstEntityId;
        }

        /// <summary>
        /// Creates multiple entities of the archetype in one go and allows to initialize certain components of the new entities. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        public EntityId CreateEntities<TC1>(Archetype archetype, int count, EntityCallback1<TC1> initializer)
            where TC1 : struct
        {
            _sceneManager.ThrowIfNotSafe();
            var pool = CreateEntitiesInternal(archetype, count, out var firstEntityId, out var firstIdxInPool);

            // user initialization
            var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC1)]];

            for (var i = 0; i < count; i++)
            {
                var idx = firstIdxInPool + i;
                initializer(new EntityId(firstEntityId.Id + i), ref componentArray1.Records[idx]);
            }

            return firstEntityId;
        }

        /// <summary>
        /// Creates multiple entities of the archetype in one go and allows to initialize certain components of the new entities. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        public EntityId CreateEntities<TC1, TC2>(Archetype archetype, int count, EntityCallback2<TC1, TC2> initializer)
            where TC1 : struct where TC2 : struct
        {
            _sceneManager.ThrowIfNotSafe();
            var pool = CreateEntitiesInternal(archetype, count, out var firstEntityId, out var firstIdxInPool);

            // user initialization
            var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC1)]];
            var componentArray2 = (ComponentArray<TC2>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC2)]];

            for (var i = 0; i < count; i++)
            {
                var idx = firstIdxInPool + i;
                initializer(new EntityId(firstEntityId.Id + i), ref componentArray1.Records[idx], ref componentArray2.Records[idx]);
            }

            return firstEntityId;
        }

        /// <summary>
        /// Creates multiple entities of the archetype in one go and allows to initialize certain components of the new entities. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        public EntityId CreateEntities<TC1, TC2, TC3>(Archetype archetype, int count, EntityCallback3<TC1, TC2, TC3> initializer)
            where TC1 : struct where TC2 : struct where TC3 : struct
        {
            _sceneManager.ThrowIfNotSafe();
            var pool = CreateEntitiesInternal(archetype, count, out var firstEntityId, out var firstIdxInPool);

            // user initialization
            var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC1)]];
            var componentArray2 = (ComponentArray<TC2>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC2)]];
            var componentArray3 = (ComponentArray<TC3>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC3)]];

            for (var i = 0; i < count; i++)
            {
                var idx = firstIdxInPool + i;
                initializer(new EntityId(firstEntityId.Id + i), ref componentArray1.Records[idx], ref componentArray2.Records[idx], ref componentArray3.Records[idx]);
            }

            return firstEntityId;
        }

        /// <summary>
        /// Creates multiple entities of the archetype in one go and allows to initialize certain components of the new entities. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        public EntityId CreateEntities<TC1, TC2, TC3, TC4>(Archetype archetype, int count, EntityCallback4<TC1, TC2, TC3, TC4> initializer)
            where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct
        {
            _sceneManager.ThrowIfNotSafe();
            var pool = CreateEntitiesInternal(archetype, count, out var firstEntityId, out var firstIdxInPool);

            // user initialization
            var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC1)]];
            var componentArray2 = (ComponentArray<TC2>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC2)]];
            var componentArray3 = (ComponentArray<TC3>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC3)]];
            var componentArray4 = (ComponentArray<TC4>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC4)]];

            for (var i = 0; i < count; i++)
            {
                var idx = firstIdxInPool + i;
                initializer(new EntityId(firstEntityId.Id + i), ref componentArray1.Records[idx], ref componentArray2.Records[idx], ref componentArray3.Records[idx], ref componentArray4.Records[idx]);
            }

            return firstEntityId;
        }

        /// <summary>
        /// Creates multiple entities of the archetype in one go and allows to initialize certain components of the new entities. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        public EntityId CreateEntities<TC1, TC2, TC3, TC4, TC5>(Archetype archetype, int count, EntityCallback5<TC1, TC2, TC3, TC4, TC5> initializer)
            where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct where TC5 : struct
        {
            _sceneManager.ThrowIfNotSafe();
            var pool = CreateEntitiesInternal(archetype, count, out var firstEntityId, out var firstIdxInPool);

            // user initialization
            var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC1)]];
            var componentArray2 = (ComponentArray<TC2>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC2)]];
            var componentArray3 = (ComponentArray<TC3>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC3)]];
            var componentArray4 = (ComponentArray<TC4>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC4)]];
            var componentArray5 = (ComponentArray<TC5>)pool.ComponentArraysPerComponentId[_sceneManager.ComponentRegistry.ComponentIdsByType[typeof(TC5)]];

            for (var i = 0; i < count; i++)
            {
                var idx = firstIdxInPool + i;
                initializer(new EntityId(firstEntityId.Id + i), ref componentArray1.Records[idx], ref componentArray2.Records[idx], ref componentArray3.Records[idx], ref componentArray4.Records[idx], ref componentArray5.Records[idx]);
            }

            return firstEntityId;
        }

        private ComponentSetPool CreateEntitiesInternal(Archetype archetype, int count, out EntityId firstEntityId, out int firstIdxInPool) 
        {
            var poolIdx = _sceneManager.GetOrCreatePool(archetype);
            var pool = _sceneManager.Pools[poolIdx];

            firstEntityId = _sceneManager.EntityIndex.Alloc(count);
            firstIdxInPool = pool.Alloc(firstEntityId, count);
            _sceneManager.EntityIndex.SetEntityInfoSequence(firstEntityId, poolIdx, firstIdxInPool, count);
            
            return pool;
        }
    }
}
