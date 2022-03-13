using GDFiddle.Framework;
using Microsoft.Xna.Framework;

namespace GDFiddle.MonoGamePlatform
{
    internal static class XnaExtensions
    {
        public static Rectangle ToXna(this System.Drawing.RectangleF r)
        {
            return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }

        public static Rectangle ToXna(this Aabb aabb)
        {
            return new Rectangle((int)aabb.TopLeft.X, (int)aabb.TopLeft.Y, (int)aabb.Size.X, (int)aabb.Size.Y);
        }

        public static Vector2 ToXna(this System.Numerics.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
    }
}
