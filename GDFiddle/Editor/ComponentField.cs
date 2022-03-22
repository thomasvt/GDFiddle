namespace GDFiddle.Editor
{
    internal class ComponentField
    {
        public ComponentField(Type type, string label, Func<object, object?> getValue)
        {
            Type = type;
            Label = label;
            GetValue = getValue;
        }

        public Type Type { get; }
        public string Label { get; }
        public Func<object, object?> GetValue { get; }
    }
}
