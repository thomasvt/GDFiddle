namespace GDFiddle.Ecs.Systems
{
    public interface IEcsSystem
    {
        void Initialize();
        void Update(Time time);
    }
}