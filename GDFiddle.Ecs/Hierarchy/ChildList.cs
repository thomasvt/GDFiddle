using System.Buffers;

namespace GDFiddle.Ecs.Hierarchy
{
    internal class ChildList : IDisposable
    {
        internal EntityId[] ChildIds;
        private int _capacity;


        public ChildList(EntityId initialChildId)
        {
            ChildIds = ArrayPool<EntityId>.Shared.Rent(1);
            ChildIds[0] = initialChildId;
            _capacity = ChildIds.Length;
            Count = 1;
        }

        public void Add(EntityId childId)
        {
            if (Count == _capacity)
            {
                var temp = ChildIds;
                _capacity *= 2;
                ChildIds = ArrayPool<EntityId>.Shared.Rent(_capacity);
                Array.Copy(temp, ChildIds, Count);
                ArrayPool<EntityId>.Shared.Return(temp);
            }
            ChildIds[Count++] = childId;
        }

        public void Remove(EntityId childId)
        {
            for (var i = 0; i < Count; i++)
            {
                if (ChildIds[i] != childId) 
                    continue;

                Count--;
                if (Count > i)
                    ChildIds[i] = ChildIds[Count];
                
                return;
            }
        }

        public int Count { get; private set; }

        public void Dispose()
        {
            ArrayPool<EntityId>.Shared.Return(ChildIds);
        }
    }
}
