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

        /// <summary>
        /// Gets the deepest child control at the given position.
        /// </summary>
        /// <param name="mouseInteractiveOnly">Return the deepest control with IsMouseInteractive set to true.</param>
        public Control? GetControlAt(Vector2 position, bool mouseInteractiveOnly)
        {
            foreach (var child in GetVisibleChildren())
            {
                if (child.ArrangedArea.Contains(position))
                {
                    var result = child.GetControlAt(position - child.ArrangedOffset, mouseInteractiveOnly);
                    if (result != null)
                        return result;
                }
            }
            if (!mouseInteractiveOnly || IsMouseInteractive)
                return this;
            return null;
        }

        public Vector2 DoMeasure(Vector2 availableSize)
        {
            DesiredSize = Measure(availableSize);
            return DesiredSize;
        }

        protected virtual Vector2 Measure(Vector2 availableSize)
        {
            return availableSize;
        }

        /// <summary>
        /// Makes the control arrange itself (and its children). Returns the size that it actually needs, which can be smaller or larger than the size provided by its parent.
        /// </summary>
        public void DoArrange(RectangleF assignedArea)
        {
            ArrangedSize = assignedArea.Size;
            ArrangedOffset = assignedArea.Location;
            Arrange(assignedArea.Size);
        }

        protected virtual void Arrange(Vector2 assignedSize)
        {
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
        /// The size of this control in pixels, as assigned by the parent control during the last Arrange phase.
        /// </summary>
        public Vector2 ArrangedSize { get; private set; }

        /// <summary>
        /// The topleft point of the AABB of this control in relation with the parent control, as assigned by the parent during the Arrange phase.
        /// </summary>
        internal Vector2 ArrangedOffset { get; private set; }

        /// <summary>
        /// The Combination of <see cref="ArrangedOffset"/> and <see cref="ArrangedSize"/> into a <see cref="RectangleF"/>.
        /// </summary>
        internal RectangleF ArrangedArea => new(ArrangedOffset, ArrangedSize);

        /// <summary>
        /// The size this control would prefer to be (calculated by Measure)
        /// </summary>
        public Vector2 DesiredSize { get; private set; }

        /// <summary>
        /// Is this control attached to a GUI?
        /// </summary>
        public bool IsAttached => GUI != null;

        public bool IsMouseInteractive { get; set; }

        public event Action<Control?>? ParentChanged;

        /// <summary>
        /// Removes the link to a parent and the owning GUI.
        /// </summary>
        internal void Detach()
        {
            Parent = null;
            GUI = null;
        }

        /// <summary>
        /// Links this control to a parent.
        /// </summary>
        internal void Attach(Control parent)
        {
            Parent = parent;
        }

        /// <summary>
        /// Converts a screen pixel position to a control-local pixel position.
        /// </summary>
        public Vector2 ToLocalPosition(Vector2 position)
        {
            position -= ArrangedOffset;
            return Parent?.ToLocalPosition(position) ?? position;
        }
    }
}

