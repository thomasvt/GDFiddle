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
        }

        protected override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            guiRenderer.DrawRectangle(new Vector2(0,0), ArrangedSize, null, IsFocused ? Color.Yellow : Color.White);
            if (IsFocused)
            {
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
                case Keys.Left: CaretIndex--; break; // arrows are rejected in TextInput code in MonoGame, they should though :(
                case Keys.Right: CaretIndex++; break; // arrows don't work
                case Keys.Escape: CancelInput(); break;
                case Keys.Delete: if (CaretIndex < InputText.Length) InputText = InputText.Remove(CaretIndex, 1); break;

                case Keys.Back:
                    if (CaretIndex > 0)
                    {
                        var caret = CaretIndex-1;
                        InputText = InputText.Remove(caret, 1);
                        CaretIndex = caret; // use temp var to bypass coercion in property setters and shift 2 position to left when caret at end of string.
                    }
                    break;
                default:
                    InputText = InputText.Insert(CaretIndex, typedCharacter.ToString());
                    CaretIndex++;
                    break;
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
            _isCancelingInput = false;
            InputText = Text;
            CaretIndex = InputText.Length;
        }

        internal override void OnKeyDown(Keys pressedKey)
        {
            // All keys should be processed in OnTextInput, so they support the OS driven repeat delay,
            // but some keys are filtered out by monogame in the TextInput logic, so we have to do a poor man's solution here:
            switch (pressedKey)
            {
                case Keys.Left: CaretIndex--; _carretBlinkStopwatch.Restart(); break;
                case Keys.Right: CaretIndex++; _carretBlinkStopwatch.Restart(); break;
                case Keys.Home: CaretIndex = 0; _carretBlinkStopwatch.Restart(); break;
                case Keys.End: CaretIndex = InputText.Length; _carretBlinkStopwatch.Restart(); break;
                case Keys.Enter: ConfirmInput(); break;
            }
        }

        protected override void OnFocus()
        {
            StartInput();
        }

        protected override void OnUnfocus()
        {
            if (!_isCancelingInput)
            {
                ReportConfirmedInput();
            }
        }

        private void ReportConfirmedInput()
        {
            if (!IsValidInput(InputText))
            {
                return;
            }

            Text = InputText;
            InputCompleted?.Invoke(InputText);
        }

        private bool IsValidInput(string text)
        {
            return true;
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

        public event Action<string>? InputCompleted;
    }
}
