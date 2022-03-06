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

            _gui = new GUI(Font.FromBMFontFile("SegoeUI14_0.png", "SegoeUI14.fnt"));
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _effect = new BasicEffect(GraphicsDevice);
            _fontTexture = Texture2D.FromFile(GraphicsDevice, _gui.Font.TextureFilename);
            ConvertGrayValueToTransparency(_fontTexture);
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

            BuildGui();
            var renderData = _gui.ProduceRenderData();

            _effect.World = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0.1f, 5);
            _effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), Vector3.UnitY);
            _effect.VertexColorEnabled = true;
            _effect.Texture = _fontTexture;
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            _effect.CurrentTechnique.Passes[0].Apply();

            foreach (var command in renderData.RenderCommands)
            {
                _effect.TextureEnabled = command.Texture != null;
                
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, renderData.Vertices, command.VertexOffset, command.TriangleCount);
            }
        }

        private void BuildGui()
        {
            _gui.BeginFrame();
            _gui.DrawText(0,0, "Recent", Color.White);
        }

        protected override void Dispose(bool disposing)
        {
            _effect?.Dispose();
        }
    }
}
