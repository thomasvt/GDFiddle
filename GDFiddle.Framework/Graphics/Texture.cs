namespace GDFiddle.Framework.Graphics
{
    public class Texture
    {
        public Texture(object platformTexture)
        {
            PlatformTexture = platformTexture;
        }

        public object PlatformTexture;
    }
}
