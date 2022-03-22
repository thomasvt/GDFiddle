using GDFiddle.UI.Text;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// The lowest baseclass for UI controls.
    /// </summary>
    public abstract class Control
    {
        private Control? _parent;
        private GUI? _gui;
        private Font? _font;

        public virtual void Render(GuiRenderer guiRenderer)
        {
            if (Background.HasValue)
            {
                guiRenderer.DrawRectangle(Vector2.Zero, ArrangedSize, Background.Value, null);
            }
        }

        public virtual Control? GetControlAt(Vector2 position)
        {
            return this;
        }

        /// <summary>
        /// Makes the control arrange itself (and its children). Returns the size that it actually needs, which can be smaller or larger than the size provided by its parent.
        /// </summary>
        public Vector2 DoArrange(RectangleF areaInParent)
        {
            var availableSize = areaInParent.Size;
            var size = Arrange(availableSize);
            
            ArrangedSize = size;
            OffsetFromParent = areaInParent.Location;
            return size;
        }

        protected virtual Vector2 Arrange(Vector2 parentAvailableSize)
        {
            return parentAvailableSize;
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
            set
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

        public Vector2 ArrangedSize { get; private set; }
        internal Vector2 OffsetFromParent { get; private set; }
        internal RectangleF ArrangeArea => new(OffsetFromParent, ArrangedSize);

        public event Action<Control?>? ParentChanged;

        /// <summary>
        /// Reset font to GUI's default.
        /// </summary>
        public void ResetFont()
        {
            _font = null;
        }

        /// <summary>
        /// The font used by this control. If not set explicitly, it returns the owning GUI's Font.
        /// </summary>
        public Font Font
        {
            get => _font ?? GUI!.DefaultFont;
            set => _font = value;
        }
    }
}

