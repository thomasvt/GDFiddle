using GDFiddle.Ecs;
using GDFiddle.Editor;
using GDFiddle.Framework.Messaging;
using GDFiddle.Games;
using GDFiddle.MonoGamePlatform;
using GDFiddle.UI;
using GDFiddle.UI.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TestGame;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = System.Drawing.Rectangle;
using Vector2 = System.Numerics.Vector2;

namespace GDFiddle
{
    internal class GDFiddleApp : Game
    {
        private readonly GraphicsDeviceManager _gdm;
        private GUI? _gui;
        private readonly IMessageBus _messageBus;
        private MouseState _previousMouseState;
        private GDFiddleGame _game;
        private CommandRenderer _commandRenderer;

        private KeyboardState _previousKeyboardState;

        public GDFiddleApp()
        {
            _gdm = new GraphicsDeviceManager(this);
            _messageBus = new MessageBus();
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            Window.TextInput += (sender, args) => _gui?.ProcessTextInput(args.Key, args.Character);
            Window.KeyUp += (sender, args) => _gui?.ProcessKeyUp(args.Key);
            Window.KeyDown += (sender, args) => _gui?.ProcessKeyDown(args.Key);
        }

        private void OnResize(object? sender, EventArgs e)
        {
            _gdm.PreferredBackBufferWidth = Window.ClientBounds.Width;
            _gdm.PreferredBackBufferHeight = Window.ClientBounds.Height;
            _gdm.ApplyChanges();
        }

        protected override void Initialize()
        {
            _gdm.PreferredBackBufferWidth = 1600;
            _gdm.PreferredBackBufferHeight = 900;
            _gdm.ApplyChanges();

            _commandRenderer = new CommandRenderer(GraphicsDevice);

            CreateEditorGui();
            CreateGame();

            base.Initialize();
        }

        private void CreateGame()
        {
            var config = new EcsConfig { InitialEntityCapacity = 100, MaxArchetypeCount = 64 };
            _game = new GameBuilder(GraphicsDevice).Build(config, typeof(StartTestGame).Assembly);
            _messageBus.Publish(new GameOpened(_game));
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var mouseState = Mouse.GetState(Window);
            var keyboardState = Keyboard.GetState();
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            var mouseWentDown = mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
            var mouseWentUp = mouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

            var time = new Time((float) gameTime.TotalGameTime.TotalSeconds, (float) gameTime.ElapsedGameTime.TotalSeconds);
            _game?.Update(time);
            _gui!.Update(GetViewArea(), mousePosition, mouseWentDown, mouseWentUp);
            _messageBus.Publish(new GameLogicUpdated());
            Mouse.SetCursor(_gui.MouseCursor);

            _previousMouseState = mouseState;
            _previousKeyboardState = keyboardState;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 10, 10));

            var renderCommands = _gui!.Render(GetViewArea());
            _commandRenderer.Render(renderCommands);
        }

        private Rectangle GetViewArea()
        {
            var viewport = GraphicsDevice.Viewport;
            var viewArea = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
            return viewArea;
        }

        private void CreateEditorGui()
        {
            var font = Font.FromBMFontFile(GraphicsDevice, "SegoeUI12_0.png", "SegoeUI12.fnt");
            _gui = new GUI(new GuiRenderer(), font)
            {
                Root = new EditorShell(GraphicsDevice, _messageBus)
            };
        }

        protected override void Dispose(bool disposing)
        {
            _commandRenderer.Dispose();
        }
    }
}
