using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.UI.Controls.Tabs
{
    public class TabControl : Control
    {
        private ItemWithMetaData<TabProperties>? _selectedTabPage;
        private float _tabHeaderHeight;

        public TabControl()
        {
            TabPages = new ItemWithMetaCollection<TabProperties>(this, () =>
            {
                var button = new TabHeaderButton();
                button.Click += () => SelectedTabPage = TabPages!.Get(button);
                return button;
            });
        }

        protected override Vector2 Measure(Vector2 availableSize)
        {
            _tabHeaderHeight = 0f;
            foreach (var tabPage in TabPages)
            {
                tabPage.Control.DoMeasure(availableSize);
                if (tabPage.Control.DesiredSize.Y > _tabHeaderHeight)
                    _tabHeaderHeight = tabPage.Control.DesiredSize.Y;
            }

            SelectedTabPage?.MetaData?.Body.DoMeasure(new Vector2(availableSize.X, availableSize.Y - _tabHeaderHeight));

            return availableSize;
        }

        protected override void Arrange(Vector2 parentAvailableSize)
        {
            var tabButtonX = 0f;
            foreach (var (control, _) in TabPages)
            {
                control.DoArrange(new RectangleF(tabButtonX, 0, control.DesiredSize.X, _tabHeaderHeight));
                tabButtonX += control.DesiredSize.X;
            }

            SelectedTabPage?.MetaData?.Body.DoArrange(new RectangleF(0, _tabHeaderHeight, parentAvailableSize.X, parentAvailableSize.Y - _tabHeaderHeight));
        }

        protected override void Render(GuiRenderer guiRenderer)
        {
            foreach (var tabPage in TabPages)
            {
                guiRenderer.DrawRectangle(tabPage.Control.ArrangedOffset, tabPage.Control.ArrangedSize, tabPage == SelectedTabPage ? SelectedTabBackground : Background, null);
            }

            if (SelectedTabPage?.MetaData == null)
                return;

            guiRenderer.DrawRectangle(new Vector2(0, _tabHeaderHeight), new Vector2(ArrangedSize.X, ArrangedSize.Y - _tabHeaderHeight), SelectedTabBackground, null);
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
    }
}
