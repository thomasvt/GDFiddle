using GDFiddle.Ecs;
using GDFiddle.Games;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Controls.Grids;
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
        private MouseState _previousMouseState;
        private readonly Scene _scene;
        private GameView _gameView;

        public GDFiddleApp(int maxArchetypeCount)
        {
            _gdm = new GraphicsDeviceManager(this);
            _scene = new Scene(new EcsConfig { InitialEntityCapacity = 100, MaxArchetypeCount = maxArchetypeCount });
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
            var game = new GameBuilder(GraphicsDevice).Build(typeof(StartTestGame).Assembly);
            _gameView.Game = game;
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            var mouseState = Mouse.GetState(Window);
            var mousePosition = new Vector2(mouseState.X, mouseState.Y);
            var mouseWentDown = mouseState.LeftButton == ButtonState.Pressed && _previousMouseState.LeftButton == ButtonState.Released;
            var mouseWentUp = mouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed;

            var time = new Time((float) gameTime.TotalGameTime.TotalSeconds, (float) gameTime.ElapsedGameTime.TotalSeconds);
            _scene.Tick(time);
            _gui!.Update(GetViewArea(), mousePosition, mouseWentDown, mouseWentUp);
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
            GraphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            };
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            foreach (var command in renderCommands.RenderCommands)
            {
                GraphicsDevice.ScissorRectangle = command.ScissorRectangle.ToXna();
                _effect.World = Matrix.CreateTranslation(command.ScissorRectangle.X, command.ScissorRectangle.Y, 0f);
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

                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, renderCommands.Vertices, command.VertexOffset,
                    command.TriangleCount);
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
            _gameView = new GameView(GraphicsDevice) { Background = new Color(66, 66, 80) };
            _gui = new GUI(new GuiRenderer(GraphicsDevice, Font.FromBMFontFile("SegoeUI14_0.png", "SegoeUI14.fnt")))
                {
                    Root = new Grid
                    {
                        ColumnDefinitions = { GridLength.Star(3), GridLength.Pixels(3), GridLength.Star() },
                        Children = {
                            { _gameView, new GridProperties {Row = 0, Column = 0} },
                            {
                                new Grid {
                                    Background = new Color(51, 51, 61),
                                    Children = {
                                        { new TextBlock { Text = "The quick brown fox jumps over the lazy dog!", Foreground = Color.White }, new GridProperties { Column = 0, Row = 0 } }
                                    }
                                }, new GridProperties { Column = 2, Row = 0 }
                            },
                            { new GridSplitter { Background = new Color(58,58, 70) }, new GridProperties { Column = 1, Row = 0 } }
                        }
                    }
                };
        }

        protected override void Dispose(bool disposing)
        {
            _effect?.Dispose();
        }
    }
}
