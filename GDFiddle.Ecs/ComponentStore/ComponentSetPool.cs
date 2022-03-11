namespace GDFiddle.Ecs.ComponentStore
{
    /// <summary>
    /// Maintains an object pool for tuples/sets of components (structs only). Has fast alloc and free without any fragmentation, but alloc'd data's order is undefined.
    /// </summary>
    internal class ComponentSetPool
    {
        private const int MaxCapacity = 2147483647 - 1;

        public readonly Archetype Archetype;
        private int _capacity;
        internal ComponentArray[] ComponentArraysPerComponentId;
        internal EntityId[] EntityIds;

        public ComponentSetPool(Archetype archetype, int capacity)
        {
            if (capacity == 0 || capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacity), $"{nameof(ComponentSetPool)}'s capacity should be within [1, {MaxCapacity}]");
            Archetype = archetype;
            _capacity = capacity;
            EntityIds = new EntityId[capacity];
            ComponentArraysPerComponentId = CraeteComponentArrays(archetype);
            Clear();
        }

        private ComponentArray[] CraeteComponentArrays(Archetype archetype)
        {
            var highestComponentId = archetype.GetHighestComponentId();
            var componentArraysPerComponentId = new ComponentArray[highestComponentId + 1];
            foreach (var component in archetype.GetComponentDefinitions())
            {
                componentArraysPerComponentId[component.ComponentId] = component.CreateComponentArray(_capacity);
            }

            return componentArraysPerComponentId;
        }

        public void Clear()
        {
            Count = 0;
        }

        /// <summary>
        /// Allocates a new ComponentSet on the pool and returns the index in the pool. The components are initialized to the defaults of their type.
        /// </summary>
        public int Alloc(EntityId entityId, bool initialize = true)
        {
            if (Count == _capacity)
                Grow(Count*2);

            var idxInPool = Count;
            EntityIds[idxInPool] = entityId;
            if (initialize)
                foreach (var componentId in Archetype.GetComponentIds())
                {
                    ComponentArraysPerComponentId[componentId].Clear(idxInPool);
                }

            return Count++;
        }

        /// <summary>
        /// Allocates multiple new ComponentSets for an uninterrupted sequence of EntityIds on the pool and returns the index of the first new set in the pool. The components are initialized to the defaults of their type.
        /// </summary>
        public int Alloc(EntityId firstEntityId, int count)
        {
            if (Count + count > _capacity)
                Grow(Count + count);

            for (var i = 0; i < count; i++)
            {
                var entityId = new EntityId(firstEntityId.Id + i);
                EntityIds[Count + i] = entityId;
            }
            foreach (var componentId in Archetype.GetComponentIds())
            {
                ComponentArraysPerComponentId[componentId].Clear(Count, count);
            }

            var firstIdx = Count;
            Count += count;

            return firstIdx;
        }

        private void Grow(int neededCapacity)
        {
            while (_capacity < neededCapacity)
            {
                _capacity <<= 1;
            }
            Array.Resize(ref EntityIds, _capacity);
            foreach (var componentId in Archetype.GetComponentIds())
            {
                ComponentArraysPerComponentId[componentId].Grow(_capacity);
            }
        }

        /// <summary>
        /// Frees the allocated space for a ComponentSet.
        /// </summary>
        public void Free(int idx)
        {
            if (idx >= Count)
                throw new ArgumentOutOfRangeException(nameof(idx), "Idx is not pointing to an allocated item in the pool.");

            if (idx < Count-1) // not the last record?
            {
                foreach (var componentId in Archetype.GetComponentIds())
                {
                    ComponentArraysPerComponentId[componentId].Copy(Count - 1, idx);
                }
                var movedEntityId = EntityIds[idx] = EntityIds[Count - 1];
                EntityMoved?.Invoke(this, new EntityMovedEventArgs(movedEntityId, idx));
            }

            Count--;
        }

        /// <summary>
        /// Removes a chunk of ComponentSets.
        /// </summary>
        public void FreeChunk(int startIdx, int count)
        {
            var afterChunkIdx = startIdx + count;
            if (afterChunkIdx > Count)
                throw new ArgumentOutOfRangeException(nameof(startIdx), "Idx + count is not within allocated items of the pool.");

            if (afterChunkIdx < Count) // chunk is not entirely at the end?
            {
                // move a chunk of other items to fill the gap
                var moveStartIdx = Count - count;
                var moveCount = count;
                if (moveStartIdx < startIdx + count)
                {
                    moveStartIdx = afterChunkIdx;
                    moveCount = Count - afterChunkIdx;
                }
                foreach (var componentId in Archetype.GetComponentIds())
                {
                    ComponentArraysPerComponentId[componentId].CopyChunk(moveStartIdx, startIdx, moveCount);
                }
                for (var i = 0; i < moveCount; i++)
                {
                    var toIdx = startIdx + i;
                    var movedEntityId = EntityIds[toIdx] = EntityIds[moveStartIdx + i];
                    EntityMoved?.Invoke(this, new EntityMovedEventArgs(movedEntityId, toIdx));
                }
            }

            Count -= count;
        }

        public string GetMemoryReport()
        {
            return $"{Count}/{_capacity} {Archetype}";
        }

        public int Count { get; private set; }

        /// <summary>
        /// Triggered when removing an Entity causes an existing Entity to take the removed one's place to prevent fragmentation.
        /// </summary>
        public event EventHandler<EntityMovedEventArgs>? EntityMoved;
        
        public void TriggerComponentCallbacks(Dictionary<byte, object> callbacks, EntityId entityId, int indexInPool)
        {
            foreach (var (componentId, callback) in callbacks)
            {
                if ((Archetype.Mask & (1ul << componentId)) != 0)
                    ComponentArraysPerComponentId[componentId].TriggerCallback(callback, entityId, indexInPool); // we need to delegate the call to the ComponentArray so it can call the generic Callback.
            }
        }
    }
}
