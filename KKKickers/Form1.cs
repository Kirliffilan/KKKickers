using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using static System.Windows.Forms.AxHost;

namespace KKKickers
{
    public partial class Form1 : Form
    {
        private GameManager gameManager;
        private bool isMenu = true;
        private string buttonText = "Õ¿◊¿“‹";
        private string labelText = "KKKICKERS";
        private readonly Image buttonImage;
        private Rectangle buttonRect;

        private readonly List<Image> backgroundImages = [];
        private Image backgroundImage => backgroundImages[curImage];
        private int curImage = 0;

        private readonly Font mainFont;
        private readonly Font headerFont;
        private readonly Font tipFont;

        private PrivateFontCollection pfc;
        private readonly System.Windows.Forms.Timer gameTimer;
        private readonly System.Windows.Forms.Timer bgTimer;
        private readonly System.Windows.Forms.Timer spikeTimer;

        public Form1()
        {
            InitializeComponent();

            DoubleBuffered = true;
            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                   ControlStyles.AllPaintingInWmPaint |
                   ControlStyles.UserPaint, true);

            buttonImage = Properties.Resources.Button;
            backgroundImages.Add(Properties.Resources.BG1);
            backgroundImages.Add(Properties.Resources.BG2);
            backgroundImages.Add(Properties.Resources.BG3);

            mainFont = LoadEmbeddedFont(46);
            headerFont = LoadEmbeddedFont(84);
            tipFont = LoadEmbeddedFont(20);

            gameManager = new GameManager(Properties.Resources.BG1, new RectangleF(0, 0, Width, Height));

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 30;
            gameTimer.Tick += GameTimer_Tick;

            bgTimer = new System.Windows.Forms.Timer();
            bgTimer.Interval = 300;
            bgTimer.Tick += BGTimer_Tick;

            spikeTimer = new System.Windows.Forms.Timer();
            spikeTimer.Interval = 3000;
            spikeTimer.Tick += SpikeTimer_Tick;

            InitializeButton();
            EndGame();
            labelText = "KKKICKERS";
            buttonText = "Õ¿◊¿“‹";
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
            isMenu = false;
            gameManager.Reset();
            gameTimer.Start();
            spikeTimer.Start();
            bgTimer.Start();
        }

        private void EndGame()
        {
            labelText = $"¬‡¯ Ò˜ÂÚ: {gameManager.Score}";
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
                foreach (var bg in gameManager.bgBounds)
                    e.Graphics.DrawImage(backgroundImage, bg);

                foreach (var wall in gameManager.CurrentWalls.Where(w => w.IsActive))
                    wall.Draw(e.Graphics);

                foreach (var wall in gameManager.NextWalls.Where(w => w.IsActive))
                    wall.Draw(e.Graphics);

                gameManager.Player.Draw(e.Graphics);

                DrawCenteredText(e.Graphics, gameManager.Score.ToString(), mainFont, Brushes.Black, 22);
                DrawCenteredText(e.Graphics, gameManager.Score.ToString(), mainFont, Brushes.White, 20);

                var highScoreTextSize = e.Graphics.MeasureString(gameManager.HighScore.ToString(), mainFont);
                e.Graphics.DrawString(gameManager.HighScore.ToString(), mainFont, Brushes.Goldenrod, Width - highScoreTextSize.Width - 20, 20);
            }
            else
            {
                DrawCenteredText(e.Graphics, labelText, headerFont, Brushes.Black, 22);
                DrawCenteredText(e.Graphics, labelText, headerFont, Brushes.White, 20);

                e.Graphics.DrawImage(buttonImage, buttonRect);

                var buttonTextSize = e.Graphics.MeasureString(buttonText, mainFont);
                float x = (Width - buttonTextSize.Width + 10) / 2;
                float y = (Height - buttonTextSize.Height + 10) / 2;
                e.Graphics.DrawString(buttonText, mainFont, Brushes.Black, x, y + 2);
                e.Graphics.DrawString(buttonText, mainFont, Brushes.White, x, y);

                if (labelText == "KKKICKERS") gameManager.Player.Draw(e.Graphics);
                else
                {
                    if (gameManager.HighScore < gameManager.Score) gameManager.currentTip = "ÕÓ‚˚È ÂÍÓ‰!\nÕÓ‚ÓÂ ÔÓ‡ÊÂÌËÂ..";
                    DrawCenteredText(e.Graphics, gameManager.currentTip, tipFont, Brushes.GhostWhite, 150);
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
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) throw new FileNotFoundException(resourceName);

                    byte[] fontData = new byte[stream.Length];
                    stream.Read(fontData, 0, fontData.Length);

                    IntPtr ptr = Marshal.AllocCoTaskMem(fontData.Length);
                    Marshal.Copy(fontData, 0, ptr, fontData.Length);
                    pfc.AddMemoryFont(ptr, fontData.Length);
                    Marshal.FreeCoTaskMem(ptr);
                }
            }

            return new Font(pfc.Families[0], size);
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            gameManager.Update();
            if (gameManager.OutOfBounds) EndGame();
            Invalidate();
        }

        private void SpikeTimer_Tick(object sender, EventArgs e) => gameManager.ToggleSpikes();
        private void BGTimer_Tick(object sender, EventArgs e)
        {
            curImage += 1;
            if (curImage >= backgroundImages.Count) curImage = 0;
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                if (isMenu) StartGame();
                else if (gameManager.Player.LetJump)
                {
                    gameManager.Player.Jump();
                    gameManager.Player.LetJump = false;
                }
            }

            else if (e.KeyCode == Keys.Escape) Close();
            else if (e.KeyCode == Keys.R) StartGame();
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (buttonRect.Contains(e.Location))
                if (isMenu) StartGame();
            else if (!isMenu) gameManager.Player.Jump();
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (gameManager == null) return;
            gameManager.FormRectangle = new RectangleF(0, 0, Width, Height);
            gameManager.InitializeBackground();
            InitializeButton();
            Invalidate();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space) gameManager.Player.LetJump = true;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.HighScore = gameManager.HighScore;
            Properties.Settings.Default.Save();
        }
    }
}