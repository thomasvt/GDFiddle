namespace GDFiddle.Ecs.Querying
{
    internal class QueryLock
    : IDisposable
    {
        private readonly EcsQueryManager _ecsQueryManager;

        public QueryLock(EcsQueryManager ecsQueryManager)
        {
            _ecsQueryManager = ecsQueryManager;
        }

        public void Dispose()
        {
            _ecsQueryManager.ReleaseQueryLock();
        }
    }
}
