using System.Drawing;
using System.Numerics;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Text;
using Microsoft.Xna.Framework.Input;
using Rectangle = System.Drawing.Rectangle;

namespace GDFiddle.UI
{
    public class GUI
    {
        private readonly GuiRenderer _guiRenderer;
        private Control? _mouseOverControl;
        private Control? _mouseCapturer;
        private Vector2 _previousMousePosition;
        private Control? _root;

        public GUI(GuiRenderer guiRenderer, Font defaultFont)
        {
            _guiRenderer = guiRenderer;
            DefaultFont = defaultFont;
        }

        public void Update(Rectangle viewArea, Vector2 mousePosition, bool mouseWentDown, bool mouseWentUp)
        {
            Root?.DoArrange(new RectangleF(viewArea.X, viewArea.Y, viewArea.Width, viewArea.Height));
            ProcessMouse(viewArea, mousePosition, mouseWentDown, mouseWentUp);
        }

        public void ProcessTextInput(Keys pressedKey, char typedCharacter)
        {
            if (FocusedControl is {IsAttached: false })
                SwitchFocusTo(null);

            FocusedControl?.OnTextInput(pressedKey, typedCharacter);
        }

        public void ProcessKeyDown(Keys pressedKey)
        {
            if (FocusedControl is { IsAttached: false })
                SwitchFocusTo(null);

            FocusedControl?.OnKeyDown(pressedKey);
        }

        public void ProcessKeyUp(Keys pressedKey)
        {
            if (FocusedControl is { IsAttached: false })
                SwitchFocusTo(null);

            FocusedControl?.OnKeyUp(pressedKey);
        }

        private void ProcessMouse(Rectangle viewArea, Vector2 mousePosition, bool mouseWentDown, bool mouseWentUp)
        {
            if (_mouseCapturer != null)
            {
                LetControlProcessMouse(_mouseCapturer, mousePosition, mouseWentDown, mouseWentUp);
                _previousMousePosition = mousePosition;
                return;
            }

            var mouseOverControl = viewArea.Contains(new Point((int) mousePosition.X, (int) mousePosition.Y))
                ? Root?.GetControlAt(mousePosition)
                : null;
            if (mouseOverControl != _mouseOverControl)
            {
                if (_mouseOverControl != null) _mouseOverControl.IsMouseOver = false;
                if (mouseOverControl != null) mouseOverControl.IsMouseOver = true;
                _mouseOverControl = mouseOverControl;
            }

            if (_mouseOverControl != null)
                LetControlProcessMouse(_mouseOverControl, mousePosition, mouseWentDown, mouseWentUp);
            else
                ProcesMouseOutsideControls(mousePosition, mouseWentDown, mouseWentDown);

            _previousMousePosition = mousePosition;
        }

        private void ProcesMouseOutsideControls(Vector2 mousePosition, bool mouseWentDown, bool mouseWentUp)
        {
            if (mouseWentDown)
                SwitchFocusTo(null);
            MouseCursor = MouseCursor.Arrow;
        }

        private void LetControlProcessMouse(Control control, Vector2 mousePosition, bool mouseWentDown, bool mouseWentUp)
        {
            if (mouseWentDown)
            {
                SwitchFocusTo(control.IsFocusable ? control : null);
                control.OnMouseDown(mousePosition);
            }
            if (mouseWentUp) control.OnMouseUp(mousePosition);
            if (_previousMousePosition != mousePosition) control.OnMouseMove(_previousMousePosition, mousePosition);
            MouseCursor = control.MouseCursor;
        }

        public RenderData Render(Rectangle viewArea)
        {
            _guiRenderer.BeginFrame(new RectangleF(viewArea.X, viewArea.Y, viewArea.Width, viewArea.Height));
            Root?.DoRender(_guiRenderer);
            return _guiRenderer.GetRenderData();
        }

        public void CaptureMouse(Control capturer)
        {
            _mouseCapturer = capturer;
            if (capturer != _mouseOverControl && _mouseOverControl != null)
            {
                _mouseOverControl.IsMouseOver = false;
                _mouseOverControl = null;
            };
        }

        public void ReleaseMouse()
        {
            _mouseCapturer = null;
        }

        public bool HasMouseCapture(Control control)
        {
            return _mouseCapturer == control;
        }

        public Control? Root
        {
            get => _root;
            set
            {
                _root = value ?? throw new ArgumentNullException(nameof(value));
                _root.GUI = this;
            }
        }

        public void SwitchFocusTo(Control? control)
        {
            if (control == FocusedControl)
                return;

            if (control is {IsFocusable: false})
                throw new ArgumentException(nameof(control), "control is not focusable.");

            FocusedControl?.UnfocusInternal();
            FocusedControl = control;
            FocusedControl?.FocusInternal();
        }

        /// <summary>
        /// Defines what the mouse cursor currently should look like.
        /// </summary>
        public MouseCursor MouseCursor { get; set; } = MouseCursor.Arrow;

        public Font DefaultFont { get; }

        public Control? FocusedControl { get; internal set; }
    }
}
