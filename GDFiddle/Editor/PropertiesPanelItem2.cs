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

        public PropertiesPanelItem2(string label)
        {
            _label = new TextBlock {Text = label, Parent = this};
            _textBox1 = new TextBox { Parent = this };
            _textBox2 = new TextBox { Parent = this };
        }

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            var labelSize = _label.DoArrange(new RectangleF(0, 0, parentAvailableSize.X * LabelWidthPercentage, parentAvailableSize.Y));
            var textbox1Size = _textBox1.DoArrange(new RectangleF(parentAvailableSize.X * LabelWidthPercentage, 0, parentAvailableSize.X * FieldWidthPercentage, parentAvailableSize.Y));
            var textbox2Size = _textBox2.DoArrange(new RectangleF(parentAvailableSize.X * (LabelWidthPercentage + FieldWidthPercentage), 0, parentAvailableSize.X * FieldWidthPercentage, parentAvailableSize.Y));

            parentAvailableSize.Y = MathF.Max(MathF.Max(textbox1Size.Y, textbox2Size.Y), labelSize.Y);
            return parentAvailableSize;
        }

        public override Control? GetControlAt(Vector2 position)
        {
            if (position.X < ArrangedSize.X * LabelWidthPercentage)
                return null;
            if (position.X < ArrangedSize.X * (LabelWidthPercentage + FieldWidthPercentage))
                return _textBox1;
            return _textBox2;
        }

        protected override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            _label.DoRender(guiRenderer);
            _textBox1.DoRender(guiRenderer);
            _textBox2.DoRender(guiRenderer);
        }

        public string Value1 { get => _textBox1.Text; set => _textBox1.Text = value; }
        public string Value2 { get => _textBox2.Text; set => _textBox2.Text = value; }
    }
}
