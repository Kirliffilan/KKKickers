namespace KKKickers.GameObjects
{
    public class Player
    {
        public static void Initialize(float startmoveBgY)
        {
            if (_instance != null)
                throw new InvalidOperationException("Player already initialized.");
            _instance = new(startmoveBgY);
        }

        private static Player _instance;
        public static Player Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("Player not initialized. Call Initialize() first.");
                return _instance;
            }
        }

        private const int _maxJumps = 2;
        private const float _defaultGravity = 0.8f;
        private const float _defaultJumpForce = -16f;
        private const int _defaultMoveSpeed = 10;
        private readonly PointF DefaultPosition = new(420, 531);
        private readonly float _startMoveBgY;

        private const float _deadGravity = 4f;
        private const float _deadJumpVelocity = -30f;

        private readonly Image _slideSprite;
        private readonly Image _fallSprite;
        private readonly Image _jumpSprite;
        private readonly Image _deadSprite;
        private readonly Image _deadSpriteUp;

        public float StartMoveBgY => _startMoveBgY;
        public PointF Position { get; set; }
        public Image Sprite { get; private set; }
        public RectangleF Bounds => new(Position, new Size(Sprite.Width * 2, Sprite.Height * 2));
        public bool IsFacingRight { get; set; } = true;
        public float VerticalVelocity { get; set; }
        public float Gravity { get; set; } = _defaultGravity;
        public float JumpForce { get; set; } = _defaultJumpForce;
        public int MoveSpeed { get; set; } = _defaultMoveSpeed;
        public float SlideSpeed { get; set; } = 0.3f;
        public float RotationProgress { get; set; }
        public float RotationSpeed { get; set; } = 15f;
        public int JumpsRemaining { get; set; }
        public bool LetJump { get; set; }
        public bool IsDead { get; set; }
        public bool IsMoving { get; set; }
        public bool IsSliding { get; set; }
        public bool IsLastSliding = false;
        private Player(float startMoveBgY)
        {
            _slideSprite = Properties.Resources.Player;
            _fallSprite = Properties.Resources.PlayerFall;
            _jumpSprite = Properties.Resources.PlayerUp;
            _deadSprite = Properties.Resources.PlayerDead;
            _deadSpriteUp = Properties.Resources.PlayerDeadUp;

            Sprite = _slideSprite;
            _startMoveBgY = startMoveBgY;
            Reset();
            Position = new PointF(0, 0);
        }

        public void Reset()
        {
            Position = DefaultPosition;
            Gravity = _defaultGravity;
            JumpForce = _defaultJumpForce;
            JumpsRemaining = _maxJumps;
            IsDead = false;
            VerticalVelocity = 0;
            IsSliding = false;
            IsMoving = false;
            MoveSpeed = _defaultMoveSpeed;
            IsFacingRight = true;
            RotationProgress = 0f;
        }

        public void Draw(Graphics g)
        {
            var state = g.Save();

            g.TranslateTransform(
                Bounds.X + Bounds.Width / 2,
                Bounds.Y + Bounds.Height / 2);
            g.RotateTransform(RotationProgress);

            var drawWidth = IsFacingRight ? Bounds.Width : -Bounds.Width;
            var drawX = IsFacingRight ? -Bounds.Width / 2 : -Bounds.Width / 2 + Bounds.Width;

            g.DrawImage(
                Sprite,
                drawX,
                -Bounds.Height / 2,
                drawWidth,
                Bounds.Height);

            g.Restore(state);
        }

        public void Jump()
        {
            if (JumpsRemaining <= 0) return;

            if (JumpsRemaining == _maxJumps) IsMoving = true;
            else ReverseDirection();

            VerticalVelocity = JumpForce;
            IsSliding = false;
            JumpsRemaining--;
        }

        public void BumpHead(RectangleF wallRect)
        {
            if (!IsLastSliding) Sprite = _deadSprite;
            IsSliding = false;
            Position = new PointF(Position.X, wallRect.Bottom + 15);
            VerticalVelocity = 0;
        }

        public void RotateJump()
        {
            if (JumpsRemaining == 0) RotationProgress += IsFacingRight ? RotationSpeed : -RotationSpeed;
            else RotationProgress = 0f;
        }

        public void ReverseJump()
        {
            VerticalVelocity = -JumpForce;
            IsSliding = false;
            JumpsRemaining--;
        }

        public void ReverseDirection() => IsFacingRight = !IsFacingRight;

        public void Slide()
        {
            IsSliding = true;
            VerticalVelocity = 0;
            JumpsRemaining = _maxJumps;
            IsMoving = false;
        }

        public void Die()
        {
            Gravity = _deadGravity;
            RotationProgress = 0f;
            JumpsRemaining = 1;
            Jump();
            ReverseDirection();
            VerticalVelocity = _deadJumpVelocity;
            IsDead = true;
        }
        public void ApplyGravity()
        {
            if (IsDead)
            {
                HandleDeadState();
                return;
            }

            if (IsSliding) HandleSlideState();
            else HandleJumpFallState();

            RotateJump();
        }

        private void HandleDeadState()
        {
            Sprite = VerticalVelocity > 0f ? _deadSprite : _deadSpriteUp;
            Position = new PointF(Position.X, Position.Y + VerticalVelocity);
            VerticalVelocity += Gravity;
        }

        private void HandleSlideState()
        {
            Position = new PointF(Position.X, Position.Y + SlideSpeed);
            Sprite = _slideSprite;
        }

        private void HandleJumpFallState()
        {
            Sprite = VerticalVelocity >= 0 || JumpsRemaining == 0 ? _fallSprite : _jumpSprite;
            if (Bounds.Top > _startMoveBgY || VerticalVelocity > 0) Position = new PointF(Position.X, Position.Y + VerticalVelocity);
            VerticalVelocity += Gravity;
        }

        public void HandleMovement()
        {
            Position = new PointF(
                Position.X + MoveSpeed * (IsFacingRight ? 1 : -1),
                Position.Y
            );
        }
    }
}