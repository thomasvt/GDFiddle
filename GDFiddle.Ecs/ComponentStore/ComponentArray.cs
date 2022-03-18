namespace GDFiddle.Ecs.ComponentStore
{
    internal abstract class ComponentArray
    {
        public abstract void Copy(int fromIdx, int toIdx);

        public abstract void CopyChunk(int fromIdx, int toIdx, int count);
        public abstract void Clear(int idx);
        public abstract void Clear(int idx, int count);

        public abstract void Relocate(int idx, ComponentArray destination, int destinationIdx);

        public abstract void Grow(int capacity);

        public abstract void TriggerCallback(object callback, EntityId entityId, int idx);
        public abstract object GetByIdx(int index);
    }

    internal class ComponentArray<T>
        : ComponentArray
        where T : struct
    {
        internal T[] Records;

        public ComponentArray(int capacity)
        {
            Records = new T[capacity];
        }

        public override void Copy(int fromIdx, int toIdx)
        {
            Records[toIdx] = Records[fromIdx];
        }

        public override void CopyChunk(int fromIdx, int toIdx, int count)
        {
            Array.ConstrainedCopy(Records, fromIdx, Records, toIdx, count);
        }

        public override void Clear(int idx)
        {
            Records[idx] = default;
        }

        public override void Clear(int idx, int count)
        {
            for (var i = 0; i < count; i++)
                Records[idx + i] = default;
        }

        public override void Relocate(int idx, ComponentArray destination, int destinationIdx)
        {
            ((ComponentArray<T>) destination).Records[destinationIdx] = Records[idx];
        }

        public override void Grow(int capacity)
        {
            if (Records.Length < capacity)
                Array.Resize(ref Records, capacity);
        }

        public override void TriggerCallback(object callback, EntityId entityId, int idx)
        {
            ((EntityCallback1<T>)callback).Invoke(entityId, ref Records[idx]);
        }

        public override object GetByIdx(int index)
        {
            return Records[index];
        }
    }
}
