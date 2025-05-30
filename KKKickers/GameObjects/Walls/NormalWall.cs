namespace KKKickers.GameObjects.Walls
{
    public class NormalWall : Wall
    {
        private static Image _sprite;
        public override Image Sprite => _sprite ??= _sprite = Properties.Resources.Wall;

        public NormalWall(PointF position, Size size)
            : base(position, size) { }
    }
}