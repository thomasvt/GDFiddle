using System.Numerics;
using GDFiddle.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.MonoGamePlatform
{
    public class SpriteBatchRenderer : IRenderer
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly SpriteBatch _sb;

        public SpriteBatchRenderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _sb = new SpriteBatch(graphicsDevice);
        }

        public void BeginFrame(Matrix3x2 viewTransform)
        {
            LastFrameSize = new (_graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height);
            _sb.Begin(SpriteSortMode.Texture, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, viewTransform.ToXna());
        }

        public void EndFrame()
        {
            _sb.End();
        }

        public void Draw(Sprite sprite, Vector2 position)
        {
            _sb.Draw((Texture2D)sprite.Texture.PlatformTexture, sprite.Aabb.Translate(position).ToXna(), Color.White);
        }

        public Vector2 LastFrameSize { get; private set; }
    }
}
