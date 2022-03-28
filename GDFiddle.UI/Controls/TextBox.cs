using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.UI.Controls
{
    /// <summary>
    /// Like a common Textbox, but behaves a little different by ignoring the Text property while in edit mode (keyboard focus), even though the app may be writing to Text each game frame.
    /// </summary>
    public class TextBox : Control
    {
        private readonly Stopwatch _carretBlinkStopwatch;
        private int _caretIndex;
        private int _selectionStart;
        private int _selectionLength;
        private string _inputText;
        private bool _isCancelingInput;
        public const float Padding = 2f;

        public TextBox()
        {
            IsFocusable = true;
            Text = string.Empty;
            _inputText = string.Empty;
            _caretIndex = 0;
            _carretBlinkStopwatch = new Stopwatch();
            MouseCursor = MouseCursor.IBeam;
            IsMouseInteractive = true;
        }

        protected override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            guiRenderer.DrawRectangle(new Vector2(0, 0), ArrangedSize, null, IsFocused ? Color.Yellow : Color.White);
            if (IsFocused)
            {
                if (_selectionLength > 0)
                {
                    var selectionStart = Font.Measure(InputText.Substring(0, _selectionStart)).X;
                    var selectionLength = Font.Measure(InputText.Substring(0, _selectionStart + _selectionLength)).X - selectionStart;
                    guiRenderer.DrawRectangle(new Vector2(selectionStart + Padding, Padding), new Vector2(selectionLength, Font.LineHeight), new Color(Color.Yellow, 0.3f), null);
                }
                guiRenderer.DrawText(Padding, Padding, InputText, Color.White, Font);
                if (_carretBlinkStopwatch.ElapsedMilliseconds % 1000 < 500)
                {
                    var carretX = Font.Measure(InputText.AsSpan(0, CaretIndex)).X + Padding;
                    guiRenderer.DrawLine(new Vector2(carretX, Padding), new Vector2(carretX, ArrangedSize.Y - Padding), 1f, Color.White);
                }
            }
            else
            {
                guiRenderer.DrawText(Padding, Padding, Text, Color.White, Font);
            }
        }

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            return new Vector2(parentAvailableSize.X, Font.LineHeight + Padding * 2);
        }

        internal override void OnTextInput(Keys pressedKey, char typedCharacter)
        {
            switch (pressedKey)
            {
                case Keys.Escape: CancelInput(); break;
                case Keys.Delete:
                    if (_selectionLength > 0)
                        DeleteSelection();
                    else if (CaretIndex < InputText.Length) 
                        InputText = InputText.Remove(CaretIndex, 1); 
                    break;

                case Keys.Back:
                    if (_selectionLength > 0)
                        DeleteSelection();
                    else if (CaretIndex > 0)
                    {
                        var caret = CaretIndex - 1;
                        InputText = InputText.Remove(caret, 1);
                        CaretIndex = caret; // use temp var to bypass coercion in property setters and shift 2 position to left when caret at end of string.
                    }
                    break;
                default:
                    if (_selectionLength > 0)
                        DeleteSelection();
                    InputText = InputText.Insert(CaretIndex, typedCharacter.ToString());
                    CaretIndex++;
                    break;
            }
        }

        private void DeleteSelection()
        {
            InputText = InputText.Remove(_selectionStart, _selectionLength);
            CaretIndex = _selectionStart;
            _selectionLength = 0;
        }

        internal override void OnKeyDown(Keys pressedKey)
        {
            // All keys should be processed in OnTextInput, so they support the OS driven repeat delay,
            // but some keys are filtered out by monogame in the TextInput logic, so we have to do a poor man's solution here:
            switch (pressedKey)
            {
                case Keys.Left: MoveCaret(_selectionLength != 0 ? _selectionStart : CaretIndex-1); break;
                case Keys.Right: MoveCaret(_selectionLength != 0 ? _selectionStart + _selectionLength : CaretIndex + 1); break;
                case Keys.Home: MoveCaret(0); break;
                case Keys.End: MoveCaret(InputText.Length); break;
                case Keys.Enter: ConfirmInput(); break;
            }
        }

        private void CancelInput()
        {
            _isCancelingInput = true;
            Unfocus();
        }

        private void ConfirmInput()
        {
            _isCancelingInput = false;
            Unfocus();
        }

        private void StartInput()
        {
            _carretBlinkStopwatch.Start();
            InputText = Text;
            Select(0, InputText.Length);
        }

        private void Select(int start, int length)
        {
            _selectionStart = start;
            _selectionLength = length;
            CaretIndex = start + length;
        }

        private void MoveCaret(int destination)
        {
            CaretIndex = destination;
            _selectionLength = 0;
            _selectionStart = CaretIndex;
            _carretBlinkStopwatch.Restart();
        }

        protected override void OnFocus()
        {
            StartInput();
        }

        protected override void OnUnfocus()
        {
            if (_isCancelingInput || !ReportConfirmedInput())
            {
            }
        }

        private bool ReportConfirmedInput()
        {
            if (InputText == string.Empty && Mode is TextBoxMode.Int or TextBoxMode.Float)
                InputText = "0";

            if (!IsValidInput(InputText))
            {
                return false;
            }

            Text = InputText;
            InputCompleted?.Invoke(InputText);
            return true;
        }

        private bool IsValidInput(string text)
        {
            switch (Mode)
            {
                case TextBoxMode.String:
                    return true;
                case TextBoxMode.Int:
                    return int.TryParse(text, out _);
                case TextBoxMode.Float:
                    return float.TryParse(text, out _);
                default:
                    throw new NotSupportedException($"Unknown TextBoxMode: '{Mode}'.");
            }
        }

        public string Text { get; set; }

        public string InputText
        {
            get => _inputText;
            private set
            {
                if (_inputText == value)
                    return;

                _inputText = value;
                CoerceCaretIndex();
            }
        }

        public int CaretIndex
        {
            get => _caretIndex;
            set
            {
                if (_caretIndex == value)
                    return;

                _caretIndex = value;
                CoerceCaretIndex();
            }
        }

        private void CoerceCaretIndex()
        {
            if (CaretIndex < 0)
                CaretIndex = 0;
            else if (CaretIndex > InputText.Length)
                CaretIndex = InputText.Length;
        }

        public TextBoxMode Mode { get; set; }

        /// <summary>
        /// Triggered when a user confirmed an input typed in the <see cref="TextBox"/>. Cancels the ending of the user input if you return false.
        /// </summary>
        public event Action<string>? InputCompleted;
    }
}
