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
        /// <summary>
        /// Size appointed by latest DoArrange call.
        /// </summary>
        public Size ActualSize { get; private set; }
        
        public virtual void Render(GuiRenderer guiRenderer, Size size)
        {
            if (Background.HasValue)
            {
                guiRenderer.DrawRectangle(new Vector2(0, 0), new Vector2(size.Width, size.Height), Background.Value, null);
            }
        }

        public virtual Control? GetControlAt(Vector2 position)
        {
            return this;
        }
        
        /// <summary>
        /// Makes the control arrange itself (and children). Returns the size that it actually needs, which can be smaller or larger.
        /// </summary>
        public Size DoArrange(Size size)
        {
             ActualSize = Arrange(size);
             return ActualSize;
        }

        protected virtual Size Arrange(Size size)
        {
            return size;
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
