using System.Numerics;
using GDFiddle.Ecs;
using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;
using GDFiddle.IoC;

namespace GDFiddle
{
    /// <summary>
    /// Contains all user created game services.
    /// </summary>
    internal class GDFiddleGame
    {
        private readonly SingletonContainer _container;
        private IRenderSystem _renderSystem;
        private List<IUpdate> _updatables;

        public GDFiddleGame(SingletonContainer container)
        {
            _container = container;
            Start();
        }

        public void Render()
        {
            _renderSystem.Render();
        }

        public void Start()
        {
            _renderSystem = _container.ResolveAllWithBaseType<IRenderSystem>().Single();
            var initializables = _container.ResolveAllWithBaseType<IInitialize>();
            _updatables = _container.ResolveAllWithBaseType<IUpdate>().ToList();

            // initialize game services:
            foreach (var initializable in initializables)
                initializable.Initialize();
        }

        public void Update(Time time)
        {
            _updatables.ForEach(u => u.Update(time));
        }

        public EntityId? GetEntityAt(Vector2 screenPosition)
        {
            return _renderSystem.GetEntityAt(screenPosition);
        }
    }
}
