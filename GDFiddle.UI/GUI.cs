using System.Drawing;
using System.Numerics;
using GDFiddle.UI.Controls;
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

        public GUI(GuiRenderer guiRenderer)
        {
            _guiRenderer = guiRenderer;
        }

        public void Update(Rectangle viewArea, Vector2 mousePosition, bool mouseWentDown, bool mouseWentUp)
        {
            Root?.DoArrange(viewArea.Size);
            ProcessMouse(viewArea, mousePosition, mouseWentDown, mouseWentUp);
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
                MouseCursor = MouseCursor.Arrow;

            _previousMousePosition = mousePosition;
        }

        private void LetControlProcessMouse(Control control, Vector2 mousePosition, bool mouseWentDown, bool mouseWentUp)
        {
            if (mouseWentDown) control.NotifyMouseDown(mousePosition);
            if (mouseWentUp) control.NotifyMouseUp(mousePosition);
            if (_previousMousePosition != mousePosition) control.NotifyMouseMove(_previousMousePosition, mousePosition);
            MouseCursor = control.MouseCursor;
        }

        public RenderData Render(Rectangle viewArea)
        {
            _guiRenderer.BeginFrame(viewArea);
            Root?.Render(_guiRenderer, viewArea.Size);
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

        /// <summary>
        /// Defines what the mouse cursor currently should look like.
        /// </summary>
        public MouseCursor MouseCursor { get; set; } = MouseCursor.Arrow;
    }
}
