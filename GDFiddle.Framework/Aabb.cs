using System.Numerics;

namespace GDFiddle.Framework
{
    public readonly struct Aabb
    {
        public readonly Vector2 TopLeft;
        public readonly Vector2 Size;

        public Aabb(Vector2 topLeft, Vector2 size)
        {
            TopLeft = topLeft;
            Size = size;
        }

        /// <summary>
        /// changes the position of the AABB.
        /// </summary>
        public Aabb Translate(Vector2 translation)
        {
            return new Aabb(TopLeft + translation, Size);
        }

        public bool Contains(Vector2 point)
        {
            return point.X >= TopLeft.X && point.X < TopLeft.X + Size.X && point.Y >= TopLeft.Y && point.Y < TopLeft.Y + Size.Y;
        }
    }
}
