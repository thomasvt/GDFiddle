using System.Drawing;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.UI.Controls
{
    public abstract class Control
    {
        private Control? _parent;
        private GUI? _gui;

        public virtual void Render(Renderer renderer, Size size)
        {
            if (Background.HasValue)
            {
                renderer.FillRectangle(new Vector2(0, 0), new Vector2(size.Width, size.Height), Background.Value);
            }
        }

        public virtual Control? GetControlAt(Vector2 position)
        {
            return this;
        }

        public virtual void Arrange(Size size)
        {
        }

        public virtual void NotifyMouseDown(Vector2 mousePosition)
        {
        }

        public virtual void NotifyMouseUp(Vector2 mousePosition)
        {
        }

        public virtual void NotifyMouseMove(Vector2 oldPosition, Vector2 newPosition)
        {
        }

        public bool IsMouseOver { get; internal set; }

        /// <summary>
        /// What should the mouse look like when it is hovering over this control?
        /// </summary>
        public MouseCursor MouseCursor { get; set; } = MouseCursor.Arrow;

        public Color? Background { get; set; }

        public Control? Parent
        {
            get => _parent;
            internal set
            {
                if (_parent == value)
                    return;

                _parent = value;
                ParentChanged?.Invoke(value);
            }
        }

        public GUI? GUI
        {
            get => _gui ?? _parent!.GUI;
            internal set => _gui = value;
        }

        public event Action<Control?>? ParentChanged;
    }
}
