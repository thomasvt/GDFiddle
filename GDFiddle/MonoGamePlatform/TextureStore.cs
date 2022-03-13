using System.Numerics;
using GDFiddle.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Texture = GDFiddle.Framework.Graphics.Texture;

namespace GDFiddle.MonoGamePlatform
{
    internal class TextureStore : ITextureStore
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly Dictionary<string, Texture> _textures;

        public TextureStore(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _textures = new Dictionary<string, Texture>();
        }

        public Texture GetTexture(string path)
        {
            var pathLower =  path.ToLower();
            if (!_textures.TryGetValue(pathLower, out var texture))
            {
                var platformTexture = Texture2D.FromFile(_graphicsDevice, path);
                texture = new Texture(platformTexture, new Vector2(platformTexture.Width, platformTexture.Height));
                _textures.Add(pathLower, texture);
            }
            return texture;
        }
    }
}
