using GDFiddle.Ecs;
using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;

namespace GDFiddle
{
    /// <summary>
    /// Contains all user created game services.
    /// </summary>
    internal class GDFiddleGame
    {
        private bool _initialized;

        public GDFiddleGame(IRenderSystem renderSystem, IEnumerable<IInitialize> initializables, IEnumerable<IUpdate> updatables)
        {
            RenderSystem = renderSystem;
            Initializables = initializables.ToList();
            Updatables = updatables.ToList();
        }

        public IRenderSystem RenderSystem { get; }
        public List<IInitialize> Initializables { get; }
        public List<IUpdate> Updatables { get; }

        public void Render()
        {
            RenderSystem.Render();
        }

        public void Update(Time time)
        {
            if (!_initialized)
            {
                Initializables.ForEach(i => i.Initialize());
                _initialized = true;
            }
            Updatables.ForEach(u => u.Update(time));
        }
    }
}
