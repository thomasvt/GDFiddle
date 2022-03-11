using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs.Querying
{
    public class EntityQuery<TC1, TC2, TC3, TC4> : EntityQuery
        where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct
    {
        private readonly byte _componentId1, _componentId2, _componentId3, _componentId4;

        internal EntityQuery(EcsQueryManager queryManager, ComponentSetPool[] pools, byte componentId1, byte componentId2, byte componentId3, byte componentId4)
        : base(queryManager, pools, (1ul << componentId1) | (1ul << componentId2) | (1ul << componentId3) | (1ul << componentId4))
        {
            _componentId1 = componentId1;
            _componentId2 = componentId2;
            _componentId3 = componentId3;
            _componentId4 = componentId4;
        }

        public void VisitAll(EntityCallback4<TC1, TC2, TC3, TC4> action)
        {
            using (QueryManager.ClaimQueryLock(QueryMask))
            {
                foreach (var pool in GetApplicablePools())
                {
                    var itemCount = pool.Count;
                    if (itemCount == 0)
                        continue;

                    var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_componentId1];
                    var componentArray2 = (ComponentArray<TC2>)pool.ComponentArraysPerComponentId[_componentId2];
                    var componentArray3 = (ComponentArray<TC3>)pool.ComponentArraysPerComponentId[_componentId3];
                    var componentArray4 = (ComponentArray<TC4>)pool.ComponentArraysPerComponentId[_componentId4];

                    if (RunInParallel && itemCount > 1)
                    {
                        Parallel.For(0, itemCount, i => action(pool.EntityIds[i], ref componentArray1.Records[i], ref componentArray2.Records[i], ref componentArray3.Records[i], ref componentArray4.Records[i]));
                    }
                    else
                    {
                        for (var i = 0; i < itemCount; i++)
                        {
                            action(pool.EntityIds[i], ref componentArray1.Records[i], ref componentArray2.Records[i], ref componentArray3.Records[i], ref componentArray4.Records[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fastest way to visit matching entities. The bulkAction will be called 0 or more times, each time receiving the requested components of another block of entities that match the query.
        /// All Component-spans in one bulkAction call are guaranteed to have the same Length.
        /// If RunInParallel is true, multiple blocks of entities are sent to bulkAction in parallel.
        /// </summary>
        public void VisitAllBulk(EntityCallbackBulk4<TC1, TC2, TC3, TC4> bulkAction)
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

        private void VisitPoolBulk(ComponentSetPool pool, EntityCallbackBulk4<TC1, TC2, TC3, TC4> bulkAction)
        {
            var itemCount = pool.Count;
            if (itemCount == 0)
                return;

            var componentArray1 = (ComponentArray<TC1>)pool.ComponentArraysPerComponentId[_componentId1];
            var componentArray2 = (ComponentArray<TC2>)pool.ComponentArraysPerComponentId[_componentId2];
            var componentArray3 = (ComponentArray<TC3>)pool.ComponentArraysPerComponentId[_componentId3];
            var componentArray4 = (ComponentArray<TC4>)pool.ComponentArraysPerComponentId[_componentId4];
            bulkAction(new Span<EntityId>(pool.EntityIds, 0, itemCount), 
                new Span<TC1>(componentArray1.Records, 0, itemCount), new Span<TC2>(componentArray2.Records, 0, itemCount), 
                new Span<TC3>(componentArray3.Records, 0, itemCount), new Span<TC4>(componentArray4.Records, 0, itemCount));
        }

        /// <summary>
        /// Perform an action on the (expected) only matching entity for this query or nothing if no matches found. Throws if match-count is not 1.
        /// </summary>
        public void VisitSingle(EntityCallback4<TC1, TC2, TC3, TC4> action)
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
                var componentArray3 = (ComponentArray<TC3>)pool.ComponentArraysPerComponentId[_componentId3];
                var componentArray4 = (ComponentArray<TC4>)pool.ComponentArraysPerComponentId[_componentId4];

                action(pool.EntityIds[0], ref componentArray1.Records[0], ref componentArray2.Records[0],
                    ref componentArray3.Records[0], ref componentArray4.Records[0]);
            }

            if (total < 1)
                throw new InvalidOperationException("Query matches no entities.");
        }

        /// <summary>
        /// Returns the only entity matching this query for readonly purposes. Throws if there are more than one entities matching this query.
        /// </summary>
        public Tuple<EntityId, TC1, TC2, TC3, TC4> GetSingle()
        {
            Tuple<EntityId, TC1, TC2, TC3, TC4>? result = default;
            VisitSingle((EntityId id, ref TC1 component1, ref TC2 component2, ref TC3 component3, ref TC4 component4) =>
            {
                result = new Tuple<EntityId, TC1, TC2, TC3, TC4>(id, component1, component2, component3, component4);
            });
            return result ?? throw new InvalidOperationException("Query matches no entities.");
        }
    }
}
