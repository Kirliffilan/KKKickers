using System.Drawing;
namespace KKKickers
{
    public abstract class Wall
    {
        public RectangleF Bounds { get; set; }
        public abstract Image Sprite { get; }
        public bool IsActive { get; set; } = true;
        protected Wall(PointF position, Size size) => Bounds = new RectangleF(position, size);
        public virtual void Draw(Graphics g)
        {
            if (IsActive && Sprite != null) g.DrawImage(Sprite, Bounds);
        }

        public virtual void HandleCollision(Player player)
        {
            WallCollisionHelper.HandleWallCollision(player, Bounds);
        }

        public virtual void Update()
        {

        }
    }
}