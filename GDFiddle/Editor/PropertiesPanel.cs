using System.Globalization;
using System.Numerics;
using GDFiddle.Ecs;
using GDFiddle.Framework.Messaging;
using GDFiddle.UI.Controls;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.Editor
{
    internal class PropertiesPanel : ContentControl
    {
        private IScene? _scene;
        private readonly Dictionary<Type, List<ComponentField>> _fieldsPerComponentType;
        private EntityId? _entityId;
        private readonly Dictionary<ComponentField, PropertiesPanelItem2> _propertiesPanelItemsPerField;

        public PropertiesPanel(IMessageBus messageBus)
        {
            _fieldsPerComponentType = new Dictionary<Type, List<ComponentField>>(64);
            _propertiesPanelItemsPerField = new Dictionary<ComponentField, PropertiesPanelItem2>(64);
            Content = new ItemsControl { Spacing = 8f };
            Padding = new Vector2(12, 8);
            ConfigureMessages(messageBus);
        }

        private void ConfigureMessages(IMessageBus messageBus)
        {
            messageBus.Subscribe<EntitySelected>(e => EntityId = e.EntityId);
            messageBus.Subscribe<GameOpened>(e => _scene = e.Game.Scene);
            messageBus.Subscribe<GameLogicUpdated>(e => UpdatePropertyValues());
        }

        public void UpdatePropertyValues()
        {
            if (!EntityId.HasValue || _scene == null)
                return;

            foreach (var component in _scene.GetComponents(EntityId.Value))
            {
                var type = component.GetType();
                var fields = _fieldsPerComponentType[type];

                foreach (var field in fields)
                {
                    UpdatePropertiesPanelItem(field, component);
                }
            }
        }

        private void UpdatePropertiesPanelItem(ComponentField field, object component)
        {
            if (!_propertiesPanelItemsPerField.TryGetValue(field, out var propertiesPanelItem))
                return;

            if (field.Type == typeof(Vector2))
            {
                var v = (Vector2)field.Getter(component)!;
                propertiesPanelItem.Value1 = v.X.ToString(CultureInfo.InvariantCulture);
                propertiesPanelItem.Value2 = v.Y.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void BuildPropertyList(EntityId? entityId)
        {
            var itemsControl = Content as ItemsControl;
            itemsControl!.Items.Clear();
            _propertiesPanelItemsPerField.Clear();

            if (!entityId.HasValue || _scene == null)
                return;

            foreach (var component in _scene.GetComponents(entityId.Value))
            {
                var type = component.GetType();
                if (!_fieldsPerComponentType.TryGetValue(type, out var fieldList))
                {
                    fieldList = type.GetFields().Select(f => new ComponentField(f.FieldType, f.Name, f.GetValue, f.SetValue)).ToList();
                    _fieldsPerComponentType.Add(type, fieldList);
                }

                itemsControl.Items.Add(new TextBlock { Text = component.GetType().Name, Foreground = Color.Gray });
                foreach (var field in fieldList)
                {
                    var propertiesPanelItem = BuildPropertiesPanelItem(field, entityId.Value, _scene.GetComponentId(type));
                    if (propertiesPanelItem == null)
                        continue;
                    _propertiesPanelItemsPerField.Add(field, propertiesPanelItem);
                    itemsControl.Items.Add(propertiesPanelItem);
                }
            }
        }

        private PropertiesPanelItem2? BuildPropertiesPanelItem(ComponentField property, EntityId entityId, byte componentId)
        {
            if (property.Type == typeof(Vector2))
            {
                var item = new PropertiesPanelItem2(property.Label, TextBoxMode.Float);
                item.Value1Edited += s =>
                {
                    var component = _scene.GetComponent(entityId, componentId);
                    var value = (Vector2) property.Getter(component);
                    value.X = float.Parse(s, CultureInfo.InvariantCulture);
                    property.Setter(component, value);
                    _scene.SetComponentDynamic(entityId, component);
                };
                item.Value2Edited += s =>
                {
                    var component = _scene.GetComponent(entityId, componentId);
                    var value = (Vector2)property.Getter(component);
                    value.Y = float.Parse(s, CultureInfo.InvariantCulture);
                    property.Setter(component, value);
                    _scene.SetComponentDynamic(entityId, component);
                };
                return item;
            }

            return null;
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
    }
}

