using System.Numerics;
using GDFiddle.Ecs;
using GDFiddle.Framework.Messaging;
using GDFiddle.UI.Controls;

namespace GDFiddle.Editor
{
    internal class EntitiesPanel : ContentControl
    {
        private IScene _scene;

        public EntitiesPanel(IMessageBus messageBus)
        {
            Content = new ItemsControl { Spacing = 8f };
            Padding = new Vector2(12, 8);
            ConfigureMessages(messageBus);
        }

        private void ConfigureMessages(IMessageBus messageBus)
        {
            messageBus.Subscribe<GameOpened>(e => _scene = e.Game.Scene);
            //messageBus.Subscribe<GameLogicUpdated>(e => UpdateList());
        }
    }
}
