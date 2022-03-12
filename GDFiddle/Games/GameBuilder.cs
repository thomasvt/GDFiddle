using System.Reflection;
using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;
using GDFiddle.IoC;
using Microsoft.Xna.Framework.Graphics;
using TestGame;

namespace GDFiddle.Games
{
    internal class GameBuilder
    {
        private readonly GraphicsDevice _graphicsDevice;

        public GameBuilder(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public GDFiddleGame Build(Assembly gameAssembly)
        {
            var types = gameAssembly.GetExportedTypes();
            var container = BuildContainer(types);

            return new GDFiddleGame
            {
                RenderSystem = container.Resolve<RenderSystem>()
            };
        }

        private Container BuildContainer(Type[] gameAssemblyTypes)
        {
            var container = new Container();
            container.RegisterInstance(_graphicsDevice);
            container.Register<IRenderer, SpriteBatchRenderer>();
            container.Register<ITextureStore, TextureStore>();

            container.Register<RenderSystem>();
            var interfaces = typeof(StartTestGame).GetInterfaces();
            if (interfaces.Contains(typeof(IInitialize))
                container.Register<StartTestGame>();

                container.Resolve<>()

            return container;
        }
    }
}
