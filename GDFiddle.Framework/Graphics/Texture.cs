using System.Numerics;

namespace GDFiddle.Framework.Graphics
{
    public class Texture
    {
        public Texture(object platformTexture, Vector2 sizeTx)
        {
            PlatformTexture = platformTexture;
            SizeTx = sizeTx;
        }

        public readonly object PlatformTexture;
        public readonly Vector2 SizeTx;
    }
}
