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
        private BasicEffect? _effect;
        private readonly IMessageBus _messageBus;
        private MouseState _previousMouseState;
        private GDFiddleGame _game;
        private readonly RasterizerState _rasterizerState = new()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        public GDFiddleApp()
        {
            _gdm = new GraphicsDeviceManager(this);
            _messageBus = new MessageBus();
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
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

            _effect = new BasicEffect(GraphicsDevice);

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
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            var mouseWentDown = mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
            var mouseWentUp = mouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

            var time = new Time((float) gameTime.TotalGameTime.TotalSeconds, (float) gameTime.ElapsedGameTime.TotalSeconds);
            _game?.Update(time);
            _gui!.Update(GetViewArea(), mousePosition, mouseWentDown, mouseWentUp);
            _messageBus.Publish(new GameLogicUpdated());
            Mouse.SetCursor(_gui.MouseCursor);
            _previousMouseState = mouseState;
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 10, 10));

            var renderCommands = _gui!.Render(GetViewArea());
            Render(renderCommands);
        }

        private void Render(RenderData renderCommands)
        {
            _effect!.World = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0.1f, 5);
            _effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), Vector3.UnitY);
            _effect.VertexColorEnabled = true;
            GraphicsDevice.RasterizerState = _rasterizerState;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            foreach (var command in renderCommands.RenderCommands)
            {
                GraphicsDevice.ScissorRectangle = new Microsoft.Xna.Framework.Rectangle((int)command.ScissorRectangle.Location.X, (int)command.ScissorRectangle.Location.Y, (int)(Math.Ceiling(command.ScissorRectangle.Size.X)), (int)(Math.Ceiling(command.ScissorRectangle.Size.Y)));
                _effect.World = Matrix.CreateTranslation(command.ScissorRectangle.Location.X, command.ScissorRectangle.Location.Y, 0f);
                if (command.Texture != null)
                {
                    _effect.TextureEnabled = true;
                    _effect.Texture = command.Texture;
                }
                else
                {
                    _effect.TextureEnabled = false;
                }

                _effect.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, renderCommands.Vertices, command.VertexOffset, command.TriangleCount);
            }
        }

        private Rectangle GetViewArea()
        {
            var viewport = GraphicsDevice.Viewport;
            var viewArea = new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
            return viewArea;
        }

        private void CreateEditorGui()
        {
            var font = Font.FromBMFontFile(GraphicsDevice, "SegoeUI14_0.png", "SegoeUI14.fnt");
            _gui = new GUI(new GuiRenderer(), font)
            {
                Root = new EditorShell(GraphicsDevice, _messageBus)
            };
        }

        protected override void Dispose(bool disposing)
        {
            _effect?.Dispose();
        }
    }
}
