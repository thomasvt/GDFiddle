namespace GDFiddle.Ecs
{
    public class InvalidArchetypeException
    : Exception
    {
        public InvalidArchetypeException(string message)
        : base(message)
        {
        }
    }
}
