using System.Drawing;
using System.Numerics;

namespace KKKickers
{
    public class BounceWall : Wall
    {
        private static Image _sprite;
        public override Image Sprite => _sprite ??= _sprite = Properties.Resources.BounceWall;
        public BounceWall(PointF position, Size size)
            : base(position, size) { }

        public override void HandleCollision(Player player)
        {
            player.JumpsRemaining = 2;
            WallCollisionHelper.HandleWallCollision(player, Bounds, true);
        }
    }
}