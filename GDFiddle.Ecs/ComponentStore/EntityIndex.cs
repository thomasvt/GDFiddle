namespace GDFiddle.Ecs.ComponentStore
{
    internal class EntityIndex
    {
        private int _entityCount;
        private int _nextEntityId = 1; // entity ids are 1 based, so that 0 can be used as null.
        internal readonly Dictionary<int, EntityIndexRecord> Records;

        public EntityIndex(int initialCapacity)
        {
            _entityCount = 0;
            Records = new Dictionary<int, EntityIndexRecord>(initialCapacity);
        }

        public void SetEntityInfo(EntityId entityId, int poolIdx, int idxInPool)
        {
            Records[entityId.Id] = new EntityIndexRecord(poolIdx, idxInPool);
        }

        /// <summary>
        /// Sets all EntityIndex records for an uninterrupted sequence of entityId + idxInPools.
        /// </summary>
        public void SetEntityInfoSequence(EntityId firstEntityId, int poolIdx, int firstIdxInPool, int count)
        {
            for (var i = 0; i < count; i++)
                Records[firstEntityId.Id + i] = new EntityIndexRecord(poolIdx, firstIdxInPool + i);
        }

        public EntityId Alloc()
        {
            var entityId = _nextEntityId++;
            _entityCount++;
            Records.Add(entityId, new EntityIndexRecord());
            return new EntityId(entityId);
        }

        /// <summary>
        /// Allocated multiple EntityIndex records in one go. Returns the first id, the other ids are garanteed to follow without gaps.
        /// </summary>
        public EntityId Alloc(int count)
        {
            var firstNewEntityId = _nextEntityId;
            _nextEntityId += count;
            _entityCount += count;
            for (var i = 0; i < count; i++)
                Records.Add(firstNewEntityId + i, new EntityIndexRecord());
            return new EntityId(firstNewEntityId);
        }

        public void Free(EntityId entityId)
        {
            Records.Remove(entityId.Id);
            _entityCount--;
        }

        public int EntityCount => _entityCount;

        public void Clear()
        {
            _entityCount = 0;
            Records.Clear();
        }
    }
}
