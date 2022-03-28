﻿using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.UI.Controls.Tabs
{
    public class TabControl : Control
    {
        private ItemWithMetaData<TabProperties>? _selectedTabPage;
        private Vector2 HeaderButtonPadding = new(4f, 4f);

        public TabControl()
        {
            TabPages = new ItemWithMetaCollection<TabProperties>(this);
            IsMouseInteractive = true;
        }

        protected override Vector2 Arrange(Vector2 parentAvailableSize)
        {
            var tabButtonX = 0f;
            foreach (var tabPage in TabPages)
            {
                var size = tabPage.Control.DoArrange(new RectangleF(tabButtonX + HeaderButtonPadding.X, HeaderButtonPadding.Y, parentAvailableSize.X - tabButtonX, TabButtonHeight));
                tabButtonX += size.X + HeaderButtonPadding.X * 2;
            }

            SelectedTabPage?.MetaData?.Body.DoArrange(new RectangleF(0, TabButtonHeight, parentAvailableSize.X, parentAvailableSize.Y - TabButtonHeight));

            return parentAvailableSize;
        }

        protected override IEnumerable<Control> GetVisibleChildren()
        {
            foreach (var tabPage in TabPages)
            {
                yield return tabPage.Control;
            }

            if (SelectedTabPage?.MetaData == null)
                yield break;

            yield return SelectedTabPage.MetaData.Body;
        }

        protected override void Render(GuiRenderer guiRenderer)
        {
            foreach (var tabPage in TabPages)
            {
                guiRenderer.DrawRectangle(tabPage.Control.ArrangedOffset - HeaderButtonPadding, tabPage.Control.ArrangedSize + HeaderButtonPadding * 2, tabPage == SelectedTabPage ? SelectedTabBackground : Background, null);
            }

            if (SelectedTabPage?.MetaData == null)
                return;

            guiRenderer.DrawRectangle(new Vector2(0, TabButtonHeight), new Vector2(ArrangedSize.X, ArrangedSize.Y - TabButtonHeight), SelectedTabBackground, null);
        }

        public override void OnMouseDown(Vector2 mousePosition)
        {
            foreach (var tabPage in TabPages)
            {
                if (tabPage.Control.ArrangedArea.Contains(mousePosition))
                {
                    SelectedTabPage = tabPage;
                    return;
                }
            }
        }

        public ItemWithMetaCollection<TabProperties> TabPages { get; }

        public ItemWithMetaData<TabProperties>? SelectedTabPage
        {
            get => _selectedTabPage;
            set
            {
                _selectedTabPage?.MetaData?.Body.Detach();
                _selectedTabPage = value;
                _selectedTabPage?.MetaData?.Body.Attach(this);
            }
        }

        public Color SelectedTabBackground { get; set; } = new(64, 64, 64);

        public int TabButtonHeight { get; set; } = 24;
    }
}
