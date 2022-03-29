using GDFiddle.UI;
using GDFiddle.UI.Controls;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.Editor
{
    /// <summary>
    /// Property control showing a label and a value of 2 fields.
    /// </summary>
    internal class PropertiesPanelItem2 : Control
    {
        private const float LabelWidthPercentage = 0.4f;
        private const int FieldCount = 2;
        private const float FieldWidthPercentage = (1 - LabelWidthPercentage) / FieldCount;

        private readonly TextBox _textBox1;
        private readonly TextBox _textBox2;
        private readonly TextBlock _label;

        public PropertiesPanelItem2(string label, TextBoxMode mode)
        {
            _label = new TextBlock {Text = label, Parent = this};
            _textBox1 = new TextBox { Parent = this, Mode = mode };
            _textBox1.InputCompleted += s => Value1Edited?.Invoke(s);
            _textBox2 = new TextBox { Parent = this , Mode = mode };
            _textBox2.InputCompleted += s => Value2Edited?.Invoke(s);
        }

        protected override Vector2 Measure(Vector2 availableSize)
        {
            _label.DoMeasure(new Vector2(availableSize.X * LabelWidthPercentage, availableSize.Y));
            _textBox1.DoMeasure(new Vector2(availableSize.X * FieldWidthPercentage, availableSize.Y));
            _textBox2.DoMeasure(new Vector2(availableSize.X * FieldWidthPercentage, availableSize.Y));

            return new Vector2(availableSize.X, MathF.Max(MathF.Max(_label.DesiredSize.Y, _textBox1.DesiredSize.Y), _textBox2.DesiredSize.Y));
        }

        protected override void Arrange(Vector2 assignedSize)
        {
            _label.DoArrange(new RectangleF(0, 0, assignedSize.X * LabelWidthPercentage, _label.DesiredSize.Y));
            _textBox1.DoArrange(new RectangleF(assignedSize.X * LabelWidthPercentage, 0, assignedSize.X * FieldWidthPercentage, _textBox1.DesiredSize.Y));
            _textBox2.DoArrange(new RectangleF(assignedSize.X * (LabelWidthPercentage + FieldWidthPercentage), 0, assignedSize.X * FieldWidthPercentage, _textBox2.DesiredSize.Y));
        }
        
        protected override IEnumerable<Control> GetVisibleChildren()
        {
            yield return _label;
            yield return _textBox1;
            yield return _textBox2;
        }

        public string Value1 { get => _textBox1.Text; set => _textBox1.Text = value; }
        public string Value2 { get => _textBox2.Text; set => _textBox2.Text = value; }

        /// <summary>
        /// The user has finished changing the value.
        /// </summary>
        public event Action<string> Value1Edited;
        /// <summary>
        /// The user has finished changing the value.
        /// </summary>
        public event Action<string> Value2Edited;
    }
}
