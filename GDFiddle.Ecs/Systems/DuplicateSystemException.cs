namespace GDFiddle.Ecs.Systems
{
    public class DuplicateSystemException
    : Exception
    {
        public DuplicateSystemException(string message)
        : base(message)
        {
        }
    }
}