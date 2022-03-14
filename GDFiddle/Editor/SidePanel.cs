using GDFiddle.UI.Controls;
using GDFiddle.UI.Controls.Grids;
using Microsoft.Xna.Framework;

namespace GDFiddle.Editor
{
    public class SidePanel : ContentControl
    {
        public SidePanel()
        {
            Content = new Grid
            {
                Background = new Color(51, 51, 61),
                Children =
                {
                    {
                        new PropertiesPanel(),
                        new GridProperties {Column = 0, Row = 0}
                    }
                }
            };
        }
    }
}
