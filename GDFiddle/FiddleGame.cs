using System.Drawing;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using GDFiddle.UI.Controls.Grids;
using GDFiddle.UI.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace GDFiddle
{
    internal class FiddleGame : Game
    {
        private BasicEffect? _effect;
        private readonly GraphicsDeviceManager _gdm;
        private GUI _gui;
        private Texture2D _fontTexture;

        public FiddleGame()
        {
            _gdm = new GraphicsDeviceManager(this);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _gdm.PreferredBackBufferWidth = 1600;
            _gdm.PreferredBackBufferHeight = 900;
            _gdm.ApplyChanges();

            var defaultFont = Font.FromBMFontFile("SegoeUI14_0.png", "SegoeUI14.fnt");
            _gui = new GUI(defaultFont);
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _effect = new BasicEffect(GraphicsDevice);
            _fontTexture = Texture2D.FromFile(GraphicsDevice, _gui.DefaultFont.TextureFilename);
            ConvertGrayValueToTransparency(_fontTexture);
            BuildGui();
        }

        private static void ConvertGrayValueToTransparency(Texture2D texture)
        {
            var pixels = new Color[texture.Width * texture.Height];
            texture.GetData(pixels);
            for (var i = 0; i < pixels.Length; i++)
            {
                ref var c = ref pixels[i];
                c.A = c.R;
                c.R = 255;
                c.G = 255;
                c.B = 255;
            }

            texture.SetData(pixels);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 10, 10));


            var viewport = GraphicsDevice.Viewport;
            var renderData = _gui.Render(new System.Drawing.Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height));

            _effect.World = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0.1f, 5);
            _effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), Vector3.UnitY);
            _effect.VertexColorEnabled = true;
            _effect.Texture = _fontTexture;
            GraphicsDevice.RasterizerState = new RasterizerState
            {
                CullMode = CullMode.None,
                ScissorTestEnable = true
            };
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            foreach (var command in renderData.RenderCommands)
            {
                GraphicsDevice.ScissorRectangle = command.ScissorRectangle.ToXna();
                _effect.World = Matrix.CreateTranslation(command.ScissorRectangle.X, command.ScissorRectangle.Y, 0f);
                _effect.TextureEnabled = command.Texture != null;
                _effect.CurrentTechnique.Passes[0].Apply();

                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, renderData.Vertices, command.VertexOffset, command.TriangleCount);
            }
        }

        private void BuildGui()
        {
            _gui.RootControl = new Grid
            {
                Children =
                {
                    { new TextBlock { Text = "The quick brown fox jumps over the lazy dog!", Foreground = Color.White }, new GridProperties { Column = 1, Row = 1 } }
                },
                ColumnDefinitions = { GridLength.Star(), GridLength.Star(2) },
                RowDefinitions = { GridLength.Star(2), GridLength.Star() }
            };
        }

        protected override void Dispose(bool disposing)
        {
            _effect?.Dispose();
        }
    }
}
