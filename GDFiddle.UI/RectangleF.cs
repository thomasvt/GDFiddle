using System.Numerics;

namespace GDFiddle.UI
{
    public readonly struct RectangleF
    {
        public readonly Vector2 Location;
        public readonly Vector2 Size;

        public RectangleF(float x, float y, float width, float height)
        {
            Location = new Vector2(x, y);
            Size = new Vector2(width, height);
        }

        public RectangleF(in Vector2 location, in Vector2 size)
        {
            Location = location;
            Size = size;
        }

        public bool Contains(Vector2 position)
        {
            return position.X >= Location.X && position.Y >= Location.Y && position.X - Location.X < Size.X && position.Y - Location.Y < Size.Y;
        }
    }
}
