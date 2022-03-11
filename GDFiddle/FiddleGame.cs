using GDFiddle.Ecs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle
{
    internal class FiddleGame
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IEcsScene _scene;
        private readonly Texture2D _texture;
        private readonly SpriteBatch _sb;

        public FiddleGame(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _scene = new EcsScene(new EcsConfig { InitialEntityCapacity = 100, MaxArchetypeCount = 100 });
            _texture = Texture2D.FromFile(_graphicsDevice, "hero.png");
            _sb = new SpriteBatch(_graphicsDevice);
        }

        public void Update(Time time)
        {
            _scene.Tick(time);
        }

        public void Draw()
        {
            _sb.Begin();
            _sb.Draw(_texture, new Vector2(10, 10), Color.White);
            _sb.End();
        }
    }
}
