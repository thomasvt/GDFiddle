using Microsoft.Xna.Framework;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle.UI.Controls.Tabs
{
    public class TabControl : Control
    {
        private ItemWithMetaData<TabProperties>? _selectedTabPage;
        private readonly Vector2 _headerButtonPadding = new(4f, 4f);

        public TabControl()
        {
            TabPages = new ItemWithMetaCollection<TabProperties>(this, () => new TabHeaderButton());
            IsMouseInteractive = true;
        }

        protected override Vector2 Measure(Vector2 availableSize)
        {
            foreach (var tabPage in TabPages)
            {
                tabPage.Control.DoMeasure(new Vector2(availableSize.X, TabButtonHeight));
            }

            SelectedTabPage?.MetaData?.Body.DoMeasure(new Vector2(availableSize.X, availableSize.Y - TabButtonHeight));

            return availableSize;
        }

        protected override void Arrange(Vector2 parentAvailableSize)
        {
            var tabButtonX = 0f;
            foreach (var (control, _) in TabPages)
            {
                control.DoArrange(new RectangleF(tabButtonX, 0, control.DesiredSize.X, TabButtonHeight));
                tabButtonX += control.DesiredSize.X;
            }

            SelectedTabPage?.MetaData?.Body.DoArrange(new RectangleF(0, TabButtonHeight, parentAvailableSize.X, parentAvailableSize.Y - TabButtonHeight));
        }

        protected override void Render(GuiRenderer guiRenderer)
        {
            foreach (var tabPage in TabPages)
            {
                guiRenderer.DrawRectangle(tabPage.Control.ArrangedOffset, tabPage.Control.ArrangedSize, tabPage == SelectedTabPage ? SelectedTabBackground : Background, null);
            }

            if (SelectedTabPage?.MetaData == null)
                return;

            guiRenderer.DrawRectangle(new Vector2(0, TabButtonHeight), new Vector2(ArrangedSize.X, ArrangedSize.Y - TabButtonHeight), SelectedTabBackground, null);
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

        public override void OnMouseDown(Vector2 mousePosition)
        {
            var localMousePosition = ToLocalPosition(mousePosition);
            foreach (var tabPage in TabPages)
            {
                if (tabPage.Control.ArrangedArea.Contains(localMousePosition))
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
