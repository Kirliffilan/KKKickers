using System.Drawing;
namespace KKKickers.GameObjects.Walls
{
    public class WinWall : Wall
    {
        private static Image _sprite;
        public override Image Sprite => _sprite ??= _sprite = Properties.Resources.WinWall;

        public bool IsTriggered { get; set; }

        public WinWall(PointF position, Size size)
            : base(position, size) { }

        public override void HandleCollision(Player player)
        {
            WallCollisionHelper.HandleWallCollision(player, Bounds);
            player.Position = new PointF(player.Position.X, player.Position.Y - player.SlideSpeed);
        }
    }
}