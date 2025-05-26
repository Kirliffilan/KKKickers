using System.Drawing;
namespace KKKickers.GameObjects.Walls
{
    public class SpikeWall : Wall
    {
        private static Image _sprite;
        public override Image Sprite => _sprite ??= _sprite = Properties.Resources.SpikeWall;

        public SpikeWall(PointF position, Size size)
            : base(position, size) { }
    }
}