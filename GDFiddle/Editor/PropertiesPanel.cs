using System.Drawing;
using GDFiddle.Ecs;
using GDFiddle.Framework.Messaging;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.Editor
{
    internal class PropertiesPanel : ContentControl
    {
        private IScene? _scene;
        private readonly Dictionary<Type, List<ComponentField>> _fieldsPerComponentType;
        private EntityId? _entityId;

        public PropertiesPanel(IMessageBus messageBus)
        {
            _fieldsPerComponentType = new Dictionary<Type, List<ComponentField>>(64);
            Content = new ItemsControl();
            ConfigureMessages(messageBus);
        }

        private void ConfigureMessages(IMessageBus messageBus)
        {
            messageBus.Subscribe<EntitySelected>(e => EntityId = e.EntityId);
            messageBus.Subscribe<GameOpened>(e => _scene = e.Game.Scene);
        }
        
        public EntityId? EntityId
        {
            get => _entityId;
            set
            {
                _entityId = value;
                BuildPropertyList(value);
            }
        }

        private void BuildPropertyList(EntityId? entityId)
        {
            var itemsControl = Content as ItemsControl;
            itemsControl!.Items.Clear();

            if (!entityId.HasValue || _scene == null)
                return;

            foreach (var component in _scene.GetComponents(entityId.Value))
            {
                var type = component.GetType();
                if (!_fieldsPerComponentType.TryGetValue(type, out var propertyList))
                {
                    propertyList = type.GetFields().Select(f => new ComponentField(f.Name, f.GetValue)).ToList();
                    _fieldsPerComponentType.Add(type, propertyList);
                }

                itemsControl.Items.Add(new TextBlock { Text = component.GetType().Name, Foreground = Color.Gray });
                foreach (var property in propertyList)
                {
                    itemsControl.Items.Add(new LiveProperty(property.Label, () => property.GetValue(component)));
                }
            }
        }
    }
}
