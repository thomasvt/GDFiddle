using System.Numerics;
using GDFiddle.Framework;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

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

        public static Matrix ToXna(this Matrix3x2 m)
        {
            return new Matrix(m.M11, m.M12, 0, 0,
                m.M21, m.M22, 0, 0,
                0, 0, 1, 0,
                m.M31, m.M32, 0, 1);
        }

        public static Vector2 ToXna(this System.Numerics.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
    }
}
