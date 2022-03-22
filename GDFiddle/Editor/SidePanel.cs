using GDFiddle.Framework.Messaging;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Controls.Grids;
using Microsoft.Xna.Framework;

namespace GDFiddle.Editor
{
    public class SidePanel : ContentControl
    {
        public SidePanel(IMessageBus messageBus)
        {
            Content = new Grid
            {
                Background = new Color(51, 51, 61),
                Children =
                {
                    { new PropertiesPanel(messageBus), new GridProperties(0, 0) }
                }
            };
        }
    }
}
