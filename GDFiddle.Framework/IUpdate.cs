using GDFiddle.Ecs;

namespace GDFiddle.Framework
{
    public interface IUpdate : IService
    {
        void Update(Time time);
    }
}
