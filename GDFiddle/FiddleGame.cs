using GDFiddle.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

            _gui = new GUI(Font.FromBMFontFile("SegoeUI32.png"));
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _effect = new BasicEffect(GraphicsDevice);
            ConvertGrayValueToTransparency();
        }

        private void ConvertGrayValueToTransparency()
        {
            _fontTexture = Texture2D.FromFile(GraphicsDevice, "SegoeUI32.png");
            var pixels = new Color[_fontTexture.Width * _fontTexture.Height];
            _fontTexture.GetData(pixels);
            for (var i = 0; i < pixels.Length; i++)
            {
                ref var c = ref pixels[i];
                c.A = c.R;
                c.R = 255;
                c.G = 255;
                c.B = 255;
            }

            _fontTexture.SetData(pixels);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            BuildGui();
            var renderData = _gui.ProduceRenderData();

            _effect.World = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0.1f, 5);
            _effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), Vector3.UnitY);
            _effect.CurrentTechnique.Passes[0].Apply();
            
            _effect.Texture = _fontTexture;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            foreach (var command in renderData.RenderCommands)
            {
                _effect.TextureEnabled = command.Texture != null;
                _effect.VertexColorEnabled = !_effect.TextureEnabled;
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, renderData.Vertices, command.VertexOffset, command.TriangleCount);
            }
        }

        private void BuildGui()
        {
            _gui.BeginFrame();
            _gui.DrawText(0,0, "Hello¾▲◙", Color.Black);
        }

        protected override void Dispose(bool disposing)
        {
            _effect?.Dispose();
        }
    }
}
