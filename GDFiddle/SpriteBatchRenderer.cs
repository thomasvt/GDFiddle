using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle
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

        public void BeginFrame()
        {
            var viewMatrix = Matrix.CreateTranslation(_graphicsDevice.Viewport.Width * 0.5f, _graphicsDevice.Viewport.Height * 0.5f, 0);
            _sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, viewMatrix);
        }

        public void EndFrame()
        {
            _sb.End();
        }

        public void Draw(Sprite sprite, Vector2 position)
        {
            _sb.Draw((Texture2D)sprite.Texture.InnerTexture, position.ToXna(), Color.White);
        }
    }
}
