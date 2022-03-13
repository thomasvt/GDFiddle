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
            var texture = _textureStore.GetTexture("hero.png");
            var sprite = new Sprite(texture, new Aabb(-texture.SizeTx * 0.5f, texture.SizeTx));
            _scene.CreateEntity(id => new PositionComponent { Position = new Vector2(0, 0) }, id => new SpriteComponent { Sprite = sprite });
        }
    }
}
