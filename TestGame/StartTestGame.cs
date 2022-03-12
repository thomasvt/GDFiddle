using System.Numerics;
using GDFiddle.Ecs;
using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;

namespace TestGame
{
    public class StartTestGame : IInitialize
    {
        private readonly IScene _scene;
        private readonly ITextureStore _textureStore;

        public StartTestGame(IScene scene, ITextureStore textureStore)
        {
            _scene = scene;
            _textureStore = textureStore;
        }

        public void Initialize()
        {
            var sprite = new Sprite(_textureStore.GetTexture("hero.png"));
            _scene.CreateEntity(id => new PositionComponent { Position = new Vector2(100, 100) }, id => new SpriteComponent { Sprite = sprite });
        }
    }
}
