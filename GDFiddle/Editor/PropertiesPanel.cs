using GDFiddle.UI.Controls;
using Microsoft.Xna.Framework;

namespace GDFiddle.Editor
{
    internal class PropertiesPanel : ContentControl
    {
        public PropertiesPanel()
        {
            Content = new ItemsControl
            {
                Items =
                {
                    new TextBlock { Text = "Hello world" },
                    new TextBlock { Text = "Hello world 2" },
                    new TextBlock { Text = "Hello world 3" },
                    new TextBlock { Text = "Hello world 4" },
                    new TextBlock { Text = "Hello world 5" },
                    new TextBlock { Text = "Hello world 6" },
                    new TextBlock { Text = "Hello world 7" }
                }
            };
        }
    }
}
