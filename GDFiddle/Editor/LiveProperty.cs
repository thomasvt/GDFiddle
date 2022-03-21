﻿using System.Drawing;
using System.Numerics;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Text;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.Editor
{
    /// <summary>
    /// Property control showing a label and a value that is fetched directly from its datasource through a delegate.
    /// </summary>
    internal class LiveProperty : Control
    {
        public readonly string Label;
        public readonly Func<object?> ValueGetter;

        public LiveProperty(string label, Func<object?> valueGetter)
        {
            Label = label;
            ValueGetter = valueGetter;
        }

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            return new Vector2(parentAvailableSize.X, (Font ?? GUI!.DefaultFont).RowHeight);
        }

        public override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            guiRenderer.DrawText(0,0, Label, LabelColor, GUI!.DefaultFont);
            guiRenderer.DrawText(ArrangedSize.X * 4 / 10, 0, ValueGetter()?.ToString() ?? "<null>", LabelColor, Font ?? GUI!.DefaultFont);
        }

        public Color LabelColor { get; set; } = Color.White;
        public Font? Font { get; set; }
    }
}
