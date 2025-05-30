namespace KKKickers.GameObjects.Walls
{
    public class CircularSaw : Wall
    {
        private static Image _sprite;
        public override Image Sprite => _sprite ??= _sprite = Properties.Resources.CircularSaw;

        public PointF startPosition;
        private readonly float _moveDistanceX;
        private readonly float _moveDistanceY;
        private const float _moveSpeed = 0.04f;
        private float _moveProgress = 0f;
        private bool _movingForward = true;
        private const int _pauseFrames = 20;
        private int _pauseCounter = 0;
        private readonly bool _isStacionary;

        private float _rotationProgress = 0f;
        private const float _rotationSpeed = 0.04f;
        private bool _rotationDirection = true;

        public CircularSaw(PointF position, Size size, float moveX = 0, float moveY = 0)
            : base(position, size)
        {
            _isStacionary = moveX == 0 && moveY == 0;
            startPosition = position;
            _moveDistanceX = moveX;
            _moveDistanceY = moveY;
        }

        public override void Update()
        {
            UpdateRotation();
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            if (_pauseCounter > 0)
            {
                _pauseCounter--;
                return;
            }

            _moveProgress += _movingForward ? _moveSpeed : -_moveSpeed;
            if (_moveProgress >= 1f || _moveProgress <= 0f)
            {
                _moveProgress = Math.Clamp(_moveProgress, 0f, 1f);
                _movingForward = !_movingForward;
                _pauseCounter = _pauseFrames;
                _rotationDirection = !_rotationDirection;
            }

            float easedProgress = EaseInOutQuad(_moveProgress);
            Bounds = new RectangleF(
                startPosition.X + _moveDistanceX * easedProgress,
                startPosition.Y + _moveDistanceY * easedProgress,
                Bounds.Width,
                Bounds.Height
            );
        }

        private void UpdateRotation()
        {
            if (_isStacionary)
            {
                _rotationProgress += _rotationSpeed;
                _rotationProgress %= 1f;
            }
            else
            {
                if (_pauseCounter == 0)
                {
                    _rotationProgress += _rotationSpeed * (_rotationDirection ? 1 : -1);
                    _rotationProgress = Math.Clamp(_rotationProgress, 0f, 1f);
                }
            }
        }

        public override void Draw(Graphics g)
        {
            float halfWidth = Bounds.Width / 2;
            float halfHeight = Bounds.Height / 2;
            float currentRotation = _isStacionary ?
                360f * _rotationProgress :
                360f * EaseInOutQuad(_rotationProgress);

            var state = g.Save();
            g.TranslateTransform(Bounds.X + halfWidth, Bounds.Y + halfHeight);
            g.RotateTransform(currentRotation);
            g.DrawImage(Sprite, -halfWidth, -halfHeight, Bounds.Width, Bounds.Height);
            g.Restore(state);
        }

        private static float EaseInOutQuad(float t) => t < 0.5f ? 2f * t * t : 1f - (float)Math.Pow(-2f * t + 2f, 2) / 2f;

        public override void HandleCollision(Player player) => player.Die();
    }
}