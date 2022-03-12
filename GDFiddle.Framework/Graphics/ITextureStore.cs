namespace GDFiddle.Framework.Graphics
{
    /// <summary>
    /// Delivers videoram textures from image files. Loads each file only once in videoram.
    /// </summary>
    public interface ITextureStore
    {
        Texture GetTexture(string path);
    }
}