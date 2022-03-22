using GDFiddle.Ecs;
using GDFiddle.Framework.Messaging;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Controls.Grids;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle.Editor
{
    internal class EditorShell : ContentControl
    {
        public EditorShell(GraphicsDevice graphicsDevice, IMessageBus messageBus)
        {
            Content = new Grid
            {
                ColumnDefinitions = {GridLength.Star(3), GridLength.Pixels(3), GridLength.Star()},
                Children =
                {
                    { new GameView(graphicsDevice, messageBus) { Background = new Color(66, 66, 80) }, new GridProperties(0, 0) },
                    { new SidePanel(messageBus), new GridProperties(2, 0)},
                    { new GridSplitter {Background = new Color(58, 58, 70)}, new GridProperties(1, 0)}
                }
            };
        }
    }
}
