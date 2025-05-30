using KKKickers.GameObjects;

namespace KKKickers.GameLogic
{
    public class BackgroundUpdator
    {
        public static void Initialize(Size bgSize, RectangleF formRectangle)
        {
            if (_instance != null)
                throw new InvalidOperationException("BackgroundUpdator already initialized.");
            _instance = new(bgSize, formRectangle);
        }

        private static BackgroundUpdator _instance;
        public static BackgroundUpdator Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("BackgroundUpdator not initialized. Call Initialize() first.");
                return _instance;
            }
        }
        public RectangleF[] BgBounds { get; private set; } = new RectangleF[6];
        private RectangleF _formRectangle;
        private readonly Size _bgSize;

        private BackgroundUpdator(Size bgSize, RectangleF formRectangle)
        {
            _bgSize = bgSize;
            _formRectangle = formRectangle;
        }

        public void GetFormRectangle(RectangleF formRectangle) => _formRectangle = formRectangle;

        public void InitializeBackground()
        {
            BgBounds[0] = new RectangleF(200, 0, _bgSize.Width * 2, _bgSize.Height * 2);
            BgBounds[1] = new RectangleF(200, -_formRectangle.Height, _bgSize.Width * 2, _bgSize.Height * 2);
            BgBounds[2] = new RectangleF(_formRectangle.Width / 2, 200, _bgSize.Width * 2, _bgSize.Height * 2);
            BgBounds[3] = new RectangleF(_formRectangle.Width / 2, 200 - _formRectangle.Height, _bgSize.Width * 2, _bgSize.Height * 2);
            BgBounds[4] = new RectangleF(_formRectangle.Width - 200, 400, -_bgSize.Width * 2, _bgSize.Height * 2);
            BgBounds[5] = new RectangleF(_formRectangle.Width - 200, 400 - _formRectangle.Height, -_bgSize.Width * 2, _bgSize.Height * 2);
        }

        public void Move()
        {
            Player player = Player.Instance;
            if (player.Position.Y < player.StartMoveBgY && player.VerticalVelocity < 0)
            {
                float moveAmount = player.VerticalVelocity;
                for (int i = 0; i < 6; i++)
                {
                    var bg = BgBounds[i];
                    BgBounds[i] = new RectangleF(
                        bg.X,
                        bg.Y - moveAmount,
                        bg.Width,
                        bg.Height
                    );
                    if (bg.Y > _formRectangle.Height)
                    {
                        Random rnd = new();
                        BgBounds[i] = new RectangleF(
                            rnd.Next(200, (int)_formRectangle.Width - 200),
                            -_formRectangle.Height,
                            bg.Width,
                            bg.Height
                        );
                    }
                }
            }
        }

    }
}