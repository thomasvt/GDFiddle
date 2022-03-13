using System.Reflection;
using GDFiddle.Ecs;
using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;
using GDFiddle.IoC;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle.Games
{
    internal class GameBuilder
    {
        private readonly GraphicsDevice _graphicsDevice;

        public GameBuilder(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public GDFiddleGame Build(EcsConfig ecsConfig, params Assembly[] gameAssemblies)
        {
            var container = BuildContainerWithFrameworkServices(ecsConfig);
            AddGameSpecificServices(container, gameAssemblies);

            var renderSystem = container.ResolveAllWithBaseType<IRenderSystem>().Single();
            var initializables = container.ResolveAllWithBaseType<IInitialize>();
            var updatables = container.ResolveAllWithBaseType<IUpdate>();

            return new GDFiddleGame(renderSystem, initializables, updatables);
        }

        private static void AddGameSpecificServices(SingletonContainer container, Assembly[] gameAssemblies)
        {
            var serviceTypes = gameAssemblies.SelectMany(a => a.GetExportedTypes()).Where(t => typeof(IService).IsAssignableFrom(t));
            foreach (var serviceType in serviceTypes)
            {
                container.RegisterAsSelf(serviceType);
            }
        }

        private SingletonContainer BuildContainerWithFrameworkServices(EcsConfig ecsConfig)
        {
            var container = new SingletonContainer();
            container.RegisterInstance(_graphicsDevice);
            container.Register<IRenderer, SpriteBatchRenderer>();
            container.Register<ITextureStore, TextureStore>();
            container.RegisterInstance(ecsConfig);
            container.Register<IScene, Scene>();

            return container;
        }
    }
}
