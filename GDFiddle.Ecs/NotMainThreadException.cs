namespace GDFiddle.Ecs
{
    public class NotMainThreadException
    : Exception
    {
        public NotMainThreadException(string message)
        : base(message)
        {
            
        }
    }
}
