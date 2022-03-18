namespace GDFiddle.Editor
{
    internal class ComponentField
    {
        public ComponentField(string label, Func<object, object?> getValue)
        {
            Label = label;
            GetValue = getValue;
        }

        public string Label { get; }
        public Func<object, object?> GetValue { get; }
    }
}
