using GDFiddle.Framework.Messaging;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Controls.Grids;
using GDFiddle.UI.Controls.Tabs;
using Microsoft.Xna.Framework;

namespace GDFiddle.Editor
{
    public class SidePanel : ContentControl
    {
        private readonly TabControl _tabControl;

        public SidePanel(IMessageBus messageBus)
        {
            Content = new Grid
            {
                Background = new Color(40,40,40),
                Children =
                {
                    { _tabControl = new TabControl
                        {
                            SelectedTabBackground = new Color(64, 64, 64),
                            TabPages =
                            {
                                { new TextBlock {Text = "Properties" }, new TabProperties(new PropertiesPanel(messageBus)) },
                                { new TextBlock {Text = "Entities" }, new TabProperties(new EntitiesPanel(messageBus)) }
                            }
                        }, 
                        new GridProperties(0, 0)
                    }
                }
            };

            _tabControl.SelectedTabPage = _tabControl.TabPages.First();
        }
    }
}
