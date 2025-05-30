using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using KKKickers.GameObjects;
using KKKickers.GameLogic;

namespace KKKickers
{
    public partial class Form1 : Form
    {
        private bool isMenu = true;
        private string buttonText = "Õ¿◊¿“‹";
        private string labelText = "KKKICKERS";
        private readonly Image buttonImage;
        private Rectangle buttonRect;

        private readonly List<Image> backgroundImages = [
            Properties.Resources.BG1,
            Properties.Resources.BG2,
            Properties.Resources.BG3,
            ];
        private Image backgroundImage => backgroundImages[curImage];
        private int curImage = 0;

        private readonly Font _mainFont;
        private readonly Font _headerFont;
        private readonly Font _tipFont;

        private PrivateFontCollection pfc;
        private readonly System.Windows.Forms.Timer gameTimer;
        private readonly System.Windows.Forms.Timer bgTimer;
        private readonly System.Windows.Forms.Timer spikeTimer;

        private StructureGenerator _structureGenerator;
        private BackgroundUpdator _backgroundUpdator;
        private CollisionHandler _collisionHandler;
        private SpikesActivator _spikesActivator;
        private ScoreHandler _scoreHandler;
        private UIHandler _uiHandler;
        private Player _player;

        public Form1()
        {
            InitializeComponent();
            InitLogic();

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                   ControlStyles.AllPaintingInWmPaint |
                   ControlStyles.UserPaint, true);

            buttonImage = Properties.Resources.Button;

            _mainFont = LoadEmbeddedFont(46);
            _headerFont = LoadEmbeddedFont(84);
            _tipFont = LoadEmbeddedFont(20);

            gameTimer = new() { Interval = 30 };
            gameTimer.Tick += GameTimer_Tick;

            bgTimer = new() { Interval = 300 };
            bgTimer.Tick += BGTimer_Tick;
            bgTimer.Start();

            spikeTimer = new() { Interval = 3000 };
            spikeTimer.Tick += SpikeTimer_Tick;

            InitializeButton();
            EndGame();
            labelText = "KKKICKERS";
            buttonText = "Õ¿◊¿“‹";
        }

        private void InitLogic()
        {
            ScoreHandler.Initialize();
            _scoreHandler = ScoreHandler.Instance;

            CollisionHandler.Initialize(new RectangleF(0, 0, Width, Height));
            _collisionHandler = CollisionHandler.Instance;

            SpikesActivator.Initialize();
            _spikesActivator = SpikesActivator.Instance;

            BackgroundUpdator.Initialize(backgroundImage.Size, new RectangleF(0, 0, Width, Height));
            _backgroundUpdator = BackgroundUpdator.Instance;

            StructureGenerator.Initialize(Height);
            _structureGenerator = StructureGenerator.Instance;

            Player.Initialize(300);
            _player = Player.Instance;

            UIHandler.Initialize();
            _uiHandler = UIHandler.Instance;
        }

        private void InitializeButton()
        {
            int buttonWidth = buttonImage.Width * 2;
            int buttonHeight = buttonImage.Height * 2;
            buttonRect = new Rectangle(
                (Width - buttonWidth) / 2,
                (Height - buttonHeight) / 2,
                buttonWidth,
                buttonHeight
            );
        }

        private void StartGame()
        {
            _uiHandler.Reset();
            _player.Reset();
            _structureGenerator.Reset();
            _backgroundUpdator.InitializeBackground();
            _spikesActivator.Reset();
            _scoreHandler.Reset();
            _collisionHandler.Reset();

            gameTimer.Start();
            spikeTimer.Start();
            isMenu = false;
            Invalidate();
        }

        private void EndGame()
        {
            labelText = $"¬‡¯ Ò˜ÂÚ: {ScoreHandler.Instance.Score}";
            buttonText = "≈Ÿ≈ –¿«";
            isMenu = true;
            gameTimer.Stop();
            spikeTimer.Stop();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (!isMenu)
            {
                foreach (var bg in _backgroundUpdator.BgBounds)
                    e.Graphics.DrawImage(backgroundImage, bg);

                foreach (var wall in _structureGenerator.CurrentStructure.Concat(_structureGenerator.NextStructure).Concat(_structureGenerator.Spikes).Where(w => w.IsActive))
                    wall.Draw(e.Graphics);

                _player.Draw(e.Graphics);

                DrawCenteredText(e.Graphics, _scoreHandler.Score.ToString(), _mainFont, Brushes.Black, 22);
                DrawCenteredText(e.Graphics, _scoreHandler.Score.ToString(), _mainFont, Brushes.White, 20);

                var highScoreTextSize = e.Graphics.MeasureString(_scoreHandler.HighScore.ToString(), _mainFont);
                e.Graphics.DrawString(_scoreHandler.HighScore.ToString(), _mainFont, Brushes.Goldenrod, Width - highScoreTextSize.Width - 20, 20);
            }
            else
            {
                DrawCenteredText(e.Graphics, labelText, _headerFont, Brushes.Black, 22);
                DrawCenteredText(e.Graphics, labelText, _headerFont, Brushes.White, 20);

                e.Graphics.DrawImage(buttonImage, buttonRect);

                var buttonTextSize = e.Graphics.MeasureString(buttonText, _mainFont);
                float x = (Width - buttonTextSize.Width + 10) / 2;
                float y = (Height - buttonTextSize.Height + 10) / 2;
                e.Graphics.DrawString(buttonText, _mainFont, Brushes.Black, x, y + 2);
                e.Graphics.DrawString(buttonText, _mainFont, Brushes.White, x, y);

                if (labelText == "KKKICKERS") _player.Draw(e.Graphics);
                else
                {
                    if (_scoreHandler.HighScore < _scoreHandler.Score) _uiHandler.ShowHighScoreTip();
                    DrawCenteredText(e.Graphics, _uiHandler.CurrentTip, _tipFont, Brushes.GhostWhite, 150);
                }
            }
        }

        private void DrawCenteredText(Graphics g, string text, Font font, Brush brush, float y)
        {
            string[] lines = text.Split(["\n"], StringSplitOptions.None);
            foreach (string line in lines)
            {
                if (string.IsNullOrEmpty(line))
                    continue;
                SizeF lineSize = g.MeasureString(line, font);
                float x = (Width - lineSize.Width) / 2;
                g.DrawString(line, font, brush, x, y);
                y += lineSize.Height + 2;
            }
        }

        private Font LoadEmbeddedFont(float size)
        {
            if (pfc == null)
            {
                pfc = new PrivateFontCollection();
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "KKKickers.Resources.pixelFont.ttf";
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) return new Font("Verdana", size);

                byte[] fontData = new byte[stream.Length];
                stream.Read(fontData, 0, fontData.Length);

                IntPtr ptr = Marshal.AllocCoTaskMem(fontData.Length);
                Marshal.Copy(fontData, 0, ptr, fontData.Length);
                pfc.AddMemoryFont(ptr, fontData.Length);
                Marshal.FreeCoTaskMem(ptr);
            }
            if (pfc.Families.Length == 0) return new Font("Verdana", size);
            return new Font(pfc.Families[0], size);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (_collisionHandler == null) return;
            MovePlayer();

            if (_collisionHandler.OutOfBounds) EndGame();
            Invalidate();
        }

        private void MovePlayer()
        {
            if (_player == null) return;

            _player.ApplyGravity();
            _collisionHandler.CheckOutOfBounds();
            _structureGenerator.UpdateSaws();
            if (_player.IsDead) return;

            if (_player.IsMoving) _player.HandleMovement();
            _collisionHandler.CheckCollisions();
            _backgroundUpdator.Move();
            _structureGenerator.Move();
        }

        private void SpikeTimer_Tick(object sender, EventArgs e)
        {
            if (_spikesActivator == null) return;
            _spikesActivator.ToggleSpikes();
        }
        private void BGTimer_Tick(object sender, EventArgs e)
        {
            curImage += 1;
            if (curImage >= backgroundImages.Count) curImage = 0;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (_player == null) return;
            if (e.KeyCode == Keys.Space)
            {
                if (isMenu) StartGame();
                else if (_player.LetJump)
                {
                    _player.Jump();
                    _player.LetJump = false;
                }
            }

            else if (e.KeyCode == Keys.Escape) Close();
            else if (e.KeyCode == Keys.R) StartGame();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_player == null) return;
            _player.LetJump = true;
            if (buttonRect.Contains(e.Location))
                if (isMenu) StartGame();
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (_player == null) return;
            if (_player.LetJump)
            {
                _player.Jump();
                _player.LetJump = false;
            }
        }
        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (_backgroundUpdator == null || _collisionHandler == null || _structureGenerator == null) return;

            _backgroundUpdator.GetFormRectangle(new RectangleF(0, 0, Width, Height));
            _collisionHandler.GetFormRectangle(new RectangleF(0, 0, Width, Height));
            _structureGenerator.GetFormHeight(Height);

            InitializeButton();
            Invalidate();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (_player == null) return;
            if (e.KeyCode == Keys.Space) _player.LetJump = true;
        }

    }
}