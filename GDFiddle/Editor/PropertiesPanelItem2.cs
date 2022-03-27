using System.Numerics;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Controls.Grids;

namespace GDFiddle.Editor
{
    /// <summary>
    /// Property control showing a label and a value of 2 fields.
    /// </summary>
    internal class PropertiesPanelItem2 : Grid
    {
        public readonly string Label;
        private readonly TextBox _textBox1;
        private readonly TextBox _textBox2;

        public PropertiesPanelItem2(string label)
        {
            Label = label;
            ColumnDefinitions.Add(GridLength.Star(4));
            ColumnDefinitions.Add(GridLength.Star(3));
            ColumnDefinitions.Add(GridLength.Star(3));
            Children.Add(new TextBlock { Text = label  }, new GridProperties(0, 0));
            Children.Add(_textBox1 = new TextBox(), new GridProperties(1, 0));
            Children.Add(_textBox2 = new TextBox(), new GridProperties(2, 0));
        }

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            parentAvailableSize.Y = Math.Max(_textBox1.Font.LineHeight, _textBox2.Font.LineHeight) + 4;
            return base.Arrange(parentAvailableSize);
        }

        public string Value1 { get => _textBox1.Text; set => _textBox1.Text = value; }
        public string Value2 { get => _textBox2.Text; set => _textBox2.Text = value; }
    }
}
