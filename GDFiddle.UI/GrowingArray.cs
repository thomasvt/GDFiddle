namespace GDFiddle.UI
{
    /// <summary>
    /// An array with limited List features, and optimized to limit memory footprint, and provides direct access to the underlying array.
    /// </summary>
    internal class GrowingArray<T>
    {
        public T[] Array;
        public int Index { get; private set; }

        public GrowingArray(int initialCapacity = 4)
        {
            Array = new T[initialCapacity];
            Index = 0;
        }

        public void EnsureCapacity(int extraCapacity)
        {
            var neededCapacity = Index + extraCapacity;
            if (Array.Length < neededCapacity)
            {
                var tmp = Array;
                Array = new T[neededCapacity * 2];
                System.Array.Copy(tmp, Array, Index);
            }
        }

        /// <summary>
        /// Adds an item. For optimization reasons, you must call EnsureCapacity yourself before calling this method.
        /// </summary>
        public void Add(T t)
        {
            Array[Index++] = t;
        }

        public Span<T> AsSpan()
        {
            return new Span<T>(Array, 0, Index);
        }

        public void Clear()
        {
            Index = 0;
        }
    }
}
