namespace GDFiddle.Framework.Graphics
{
    public class Sprite
    {
        public Sprite(Texture texture, Aabb aabb)
        {
            Texture = texture;
            Aabb = aabb;
        }

        public readonly Texture Texture;
        /// <summary>
        /// The rectangle in worldspace that defines the size and origin-placement of the sprite.
        /// </summary>
        public Aabb Aabb;
    }
}
