using System.Drawing;
using System.Numerics;

namespace KKKickers
{
    public class BrokenWall : Wall
    {
        private static Image _sprite;
        public override Image Sprite => _sprite ??= _sprite = Properties.Resources.BrokenWall;
        public bool IsBroken { get; private set; } = false;
        public BrokenWall(PointF position, Size size)
            : base(position, size) { }

        public override void HandleCollision(Player player)
        {
            IsBroken = true;
            WallCollisionHelper.HandleWallCollision(player, Bounds);
        }
    }
}