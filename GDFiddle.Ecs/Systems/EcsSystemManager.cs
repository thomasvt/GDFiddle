using System.Diagnostics;

namespace GDFiddle.Ecs.Systems
{
    internal class EcsSystemManager : IEcsSystemManager
    {
        private readonly Dictionary<object, PerformanceCounter> _performanceCounters;
        private readonly HashSet<IEcsSystem> _systems;
        private bool _isInitialized = false;

        public EcsSystemManager()
        {
            _systems = new HashSet<IEcsSystem>();
            _performanceCounters = new Dictionary<object, PerformanceCounter>();
            Clear();
        }
        
        public void Add(IEcsSystem ecsSystem)
        {
            if (_isInitialized)
                throw new Exception("Cannot add more Systems after InitializeAll() was called.");
            if (_systems.Contains(ecsSystem))
                throw new DuplicateSystemException($"A ecsSystem of type {ecsSystem.GetType().FullName} is already registered.");
            _systems.Add(ecsSystem);
            _performanceCounters.Add(ecsSystem, new PerformanceCounter(ecsSystem.GetType().Name));
        }

        public void AddRange(params IEcsSystem[] systems)
        {
            AddRange((IEnumerable<IEcsSystem>)systems);
        }

        public void AddRange(IEnumerable<IEcsSystem> ecsSystems)
        {
            foreach (var ecsSystem in ecsSystems)
            {
                Add(ecsSystem);
            }
        }

        public void InitializeAll()
        {
            foreach (var system in _systems)
            {
                system.Initialize();
            }

            _isInitialized = true;
        }

        public void UpdateAll(Time time)
        {
            var swTotal = Stopwatch.StartNew();
            foreach (var system in _systems)
            {
                var sw = Stopwatch.StartNew();
                system.Update(time);
                _performanceCounters[system].AddMeasurement(sw.Elapsed.TotalSeconds);
            }
            _performanceCounters[this].AddMeasurement(swTotal.Elapsed.TotalSeconds);
        }

        public string GetPerformanceReport()
        {
            return string.Join(Environment.NewLine, _performanceCounters.Values.OrderByDescending(c => c.Value).Select(pc => pc.ToString()));
        }

        public void Clear()
        {
            _systems.Clear();
            _performanceCounters.Clear();
            _performanceCounters.Add(this, new PerformanceCounter("SystemTotal"));
        }
    }
}
