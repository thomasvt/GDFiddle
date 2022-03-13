using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs.Querying
{
    public class EntityQuery<TC> : EntityQuery
        where TC : struct
    {
        private readonly int _componentId;

        internal EntityQuery(EcsQueryManager queryManager, ComponentSetPool[] pools, byte componentId)
        : base(queryManager, pools, 1ul << componentId)
        {
            _componentId = componentId;
        }

        /// <summary>
        /// Visits all matching entities. Your bulkAction will be called 0 or more times, each time receiving the requested components of another block of entities that match the query.
        /// All Component-spans in one bulkAction call are guaranteed to have the same Length.
        /// If RunInParallel is true, bulkAction is called for all matching entity blocks in parallel.
        /// </summary>
        public void VisitAll(EntityCallbackBulk1<TC> bulkAction)
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

        private void VisitPoolBulk(ComponentSetPool pool, EntityCallbackBulk1<TC> bulkAction)
        {
            var itemCount = pool.Count;
            if (itemCount == 0)
                return;

            var componentArray1 = (ComponentArray<TC>)pool.ComponentArraysPerComponentId[_componentId];
            bulkAction(new Span<EntityId>(pool.EntityIds, 0, itemCount), new Span<TC>(componentArray1.Records, 0, itemCount));
        }

        /// <summary>
        /// Perform an action on the (expected) only matching entity for this query or nothing if no matches found. Throws if match-count > 1.
        /// </summary>
        public void VisitSingle(EntityCallback1<TC> action)
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
                var componentArray1 = (ComponentArray<TC>)pool.ComponentArraysPerComponentId[_componentId];

                action(pool.EntityIds[0], ref componentArray1.Records[0]);
            }

            if (total < 1)
                throw new InvalidOperationException("Query matches no entities.");
        }

        /// <summary>
        /// Returns the only entity matching this query for readonly purposes. Throws if there are more than one entities matching this query.
        /// </summary>
        public TC GetSingle()
        {
            TC result = default;
            VisitSingle((EntityId id, ref TC component1) =>
            {
                result = component1;
            });
            return result;
        }
    }
}
