using System;
namespace KKKickers
{
    public class Spikes : Wall
    {
        private static Image _sprite;
        public override Image Sprite => _sprite ??= _sprite = Properties.Resources.Spikes;

        public Spikes(PointF position, Size size)
            : base(position, size) { }

        public override void HandleCollision(Player player) => player.Die();
    }
}
