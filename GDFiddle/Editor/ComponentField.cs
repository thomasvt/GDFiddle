
namespace GDFiddle.Editor
{
    /// <summary>
    /// Inspecion/reflection data about a component field. Used by the Editor's <see cref="PropertiesPanel"/>.
    /// </summary>
    internal class ComponentField
    {
        public ComponentField(Type type, string label, Func<object, object?> getter, Action<object, object?> setter)
        {
            Type = type;
            Label = label;
            Getter = getter;
            Setter = setter;
        }

        public Type Type { get; }
        public string Label { get; }
        /// <summary>
        /// Getter delegate, to get the value of the given instance.
        /// </summary>
        public Func<object, object?> Getter { get; }

        public Action<object, object?> Setter { get; }
    }
}
