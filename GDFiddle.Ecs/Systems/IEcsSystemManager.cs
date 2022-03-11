namespace GDFiddle.Ecs.Systems
{
    public interface IEcsSystemManager
    {
        void Add(IEcsSystem ecsSystem);

        void AddRange(params IEcsSystem[] ecsSystems);
        void AddRange(IEnumerable<IEcsSystem> ecsSystems);
        string GetPerformanceReport();
    }
}