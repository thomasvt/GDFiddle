namespace GDFiddle.Ecs.ComponentStore
{
    internal readonly struct EntityIndexRecord
    {
        public EntityIndexRecord(int poolIdx, int idxInPool)
        {
            PoolIdx = poolIdx;
            IdxInPool = idxInPool;
        }

        /// <summary>
        /// The index of the archetype pool.
        /// </summary>
        public readonly int PoolIdx;
        /// <summary>
        /// The entity's itemindex inside the archetype pool.
        /// </summary>
        public readonly int IdxInPool;
    }
}
