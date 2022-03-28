using System.Reflection.Metadata.Ecma335;
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

        protected virtual void Render(GuiRenderer guiRenderer)
        {
            if (Background.HasValue)
                guiRenderer.DrawRectangle(Vector2.Zero, ArrangedSize, Background.Value, null);
        }

        public void DoRender(GuiRenderer guiRenderer)
        {
            using var scope = guiRenderer.PushSubArea(ArrangedArea);
            Render(guiRenderer);
            foreach (var child in GetVisibleChildren())
            {
                child.DoRender(guiRenderer);
            }
        }

        public Control? GetControlAt(Vector2 position)
        {
            foreach (var child in GetVisibleChildren())
            {
                if (child.ArrangedArea.Contains(position))
                    return child.GetControlAt(position - child.OffsetFromParent);
            }
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

        public virtual void OnMouseDown(Vector2 mousePosition)
        {
        }

        public virtual void OnMouseUp(Vector2 mousePosition)
        {
        }

        public virtual void OnMouseMove(Vector2 oldPosition, Vector2 newPosition)
        {
        }

        internal virtual void OnTextInput(Keys pressedKey, char typedCharacter)
        {
        }

        internal virtual void OnKeyUp(Keys pressedKey)
        {
        }

        internal virtual void OnKeyDown(Keys pressedKey)
        {
        }

        protected virtual IEnumerable<Control> GetVisibleChildren()
        {
            yield break;
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

        /// <summary>
        /// Reset font to GUI's default.
        /// </summary>
        public void ResetFont()
        {
            _font = null;
        }

        public void Unfocus()
        {
            GUI!.SwitchFocusTo(null);
        }

        public void Focus()
        {
            GUI!.SwitchFocusTo(this);
        }

        public void UnfocusInternal()
        {
            IsFocused = false;
            OnUnfocus();
        }

        protected virtual void OnUnfocus()
        {
        }

        public void FocusInternal()
        {
            IsFocused = true;
            OnFocus();
        }

        protected virtual void OnFocus()
        {
        }

        /// <summary>
        /// The font used by this control. If not set explicitly, it returns the owning GUI's Font.
        /// </summary>
        public Font Font
        {
            get => _font ?? GUI!.DefaultFont;
            set => _font = value;
        }

        public bool IsFocusable { get; set; }

        public bool IsFocused { get; set; }

        /// <summary>
        /// The size of this control in pixels, as calculated by the Arrange phase while rendering.
        /// </summary>
        public Vector2 ArrangedSize { get; private set; }
        /// <summary>
        /// The topleft point of the AABB of this control, as calculated by the parent during the Arrange phase while rendering.
        /// </summary>
        internal Vector2 OffsetFromParent { get; private set; }
        /// <summary>
        /// The Combination of <see cref="OffsetFromParent"/> and <see cref="ArrangedSize"/> into a <see cref="RectangleF"/>.
        /// </summary>
        internal RectangleF ArrangedArea => new(OffsetFromParent, ArrangedSize);

        public event Action<Control?>? ParentChanged;
    }
}

