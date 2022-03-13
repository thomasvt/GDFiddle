using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs.Querying
{
    public abstract class EntityQuery
    {
        protected readonly EcsQueryManager QueryManager;
        internal readonly ComponentSetPool[] Pools;
        internal readonly ulong QueryMask;
        internal ulong BlackQueryMask, WhiteQueryMask;

        internal EntityQuery(EcsQueryManager queryManager, ComponentSetPool[] pools, ulong queryMask)
        {
            QueryManager = queryManager;
            Pools = pools;
            QueryMask = queryMask;
            RunInParallel = false;
        }

        /// <summary>
        /// Returns all pools matching the query criteria of included and excluded Components.
        /// </summary>
        /// <returns></returns>
        internal IEnumerable<ComponentSetPool> GetApplicablePools()
        {
            var whiteQueryMask = WhiteQueryMask | QueryMask;
            for (var i = 0; i < Pools.Length; i++)
            {
                var pool = Pools[i];
                if (pool != null && (pool.Archetype.Mask & whiteQueryMask) == whiteQueryMask && (pool.Archetype.Mask & BlackQueryMask) == 0)
                    yield return pool;
            }
        }

        /// <summary>
        /// Fast way to know how many entities match this query.
        /// </summary>
        public int Count
        {
            get
            {
                var sum = 0;
                foreach (var pool in GetApplicablePools())
                {
                    sum += pool.Count;
                }

                return sum;
            }
        } 

        /// <summary>
        /// Calls your processing code in parallel for all matching blocks of entities.
        /// </summary>
        public bool RunInParallel { get; set; }
    }
}