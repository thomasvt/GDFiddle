using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs.Querying
{
    public class EntityQuery<TC1, TC2> : EntityQuery where TC1 : struct where TC2 : struct
    {
        private readonly int _componentId1, _componentId2;

        internal EntityQuery(EcsQueryManager queryManager, ComponentSetPool[] pools, byte componentId1, byte componentId2)
        : base(queryManager, pools, (1ul << componentId1) | (1ul << componentId2))
        {
            _componentId1 = componentId1;
            _componentId2 = componentId2;
        }

        /// <summary>
        /// Visits all matching entities. Your bulkAction will be called 0 or more times, each time receiving the requested components of another block of entities that match the query.
        /// All Component-spans in one bulkAction call are guaranteed to have the same Length.
        /// If RunInParallel is true, bulkAction is called for all matching entity blocks in parallel.
        /// </summary>
        public void VisitAll(EntityCallbackBulk2<TC1, TC2> bulkAction)
        {
            using (QueryManager.ClaimQueryLock(QueryMask))
            {
                if (RunInParallel)
                {
                    Parallel.ForEach(GetApplicablePools(), pool =>
                    {
                        VisitPoolBulk(pool, bulkAction);
                    });
                }
                else
                {
                    foreach (var pool in GetApplicablePools())
                    {
                        VisitPoolBulk(pool, bulkAction);
                    }
                }
            }
        }

        private void VisitPoolBulk(ComponentSetPool pool, EntityCallbackBulk2<TC1, TC2> bulkAction)
        {
            var itemCount = pool.Count;
            if (itemCount == 0)
                return;

            var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_componentId1];
            var componentArray2 = (ComponentArray<TC2>)pool.ComponentArraysPerComponentId[_componentId2];
            bulkAction(new Span<EntityId>(pool.EntityIds, 0, itemCount), new Span<TC1>(componentArray1.Records, 0, itemCount), new Span<TC2>(componentArray2.Records, 0, itemCount));
        }

        /// <summary>
        /// Perform an action on the (expected) only matching entity for this query or nothing if no matches found. Throws if match-count > 1.
        /// </summary>
        /// <returns>True if a matching entity was found, false if no matches were found.</returns>
        public void VisitSingle(EntityCallback2<TC1, TC2> action)
        {
            // single needs no querylock because there is no enumeration to change.
            var total = 0;
            foreach (var pool in GetApplicablePools())
            {
                if (pool.Count == 0)
                    continue;

                total += pool.Count;
                if (total > 1)
                    throw new InvalidOperationException("Query matches more than one entity.");
                var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_componentId1];
                var componentArray2 = (ComponentArray<TC2>)pool.ComponentArraysPerComponentId[_componentId2];

                action(pool.EntityIds[0], ref componentArray1.Records[0], ref componentArray2.Records[0]);
            }

            if (total < 1)
                throw new InvalidOperationException("Query matches no entities.");
        }

        /// <summary>
        /// Returns the queried components of the single entity matching this query. Throws if there are more than one entities matching this query.
        /// </summary>
        public Tuple<EntityId, TC1, TC2> GetSingle()
        {
            Tuple<EntityId, TC1, TC2>? result = default;
            VisitSingle((EntityId id, ref TC1 component1, ref TC2 component2) =>
            {
                result = new Tuple<EntityId, TC1, TC2>(id, component1, component2);
            });
            return result ?? throw new InvalidOperationException("Query matches no entities.");
        }
    }
}
