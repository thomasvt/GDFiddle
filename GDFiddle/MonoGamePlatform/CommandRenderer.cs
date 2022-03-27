using GDFiddle.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle.MonoGamePlatform
{
    internal class CommandRenderer : IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly BasicEffect? _effect;
        private readonly RasterizerState _rasterizerState = new()
        {
            CullMode = CullMode.None,
            ScissorTestEnable = true
        };

        public CommandRenderer(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _effect = new BasicEffect(graphicsDevice);
        }

        public void Render(RenderData renderCommands)
        {
            _effect!.World = Matrix.Identity;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, _graphicsDevice.Viewport.Width, _graphicsDevice.Viewport.Height, 0, 0.1f, 5);
            _effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), new Vector3(0, 0, 0), Vector3.UnitY);
            _effect.VertexColorEnabled = true;
            _graphicsDevice.RasterizerState = _rasterizerState;
            _graphicsDevice.BlendState = BlendState.NonPremultiplied;
            _graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            foreach (var command in renderCommands.RenderCommands)
            {
                _graphicsDevice.ScissorRectangle = new Rectangle((int)command.ScissorRectangle.Location.X, (int)command.ScissorRectangle.Location.Y, (int)(Math.Ceiling(command.ScissorRectangle.Size.X)), (int)(Math.Ceiling(command.ScissorRectangle.Size.Y)));
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

                _graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, renderCommands.Vertices, command.VertexOffset, command.TriangleCount);
            }
        }

        public void Dispose()
        {
            _effect?.Dispose();
            _rasterizerState.Dispose();
        }
    }
}
