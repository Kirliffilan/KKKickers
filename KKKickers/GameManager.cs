using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace KKKickers
{
    public class GameManager
    {
        public Player Player { get; }
        public List<Wall> CurrentWalls { get; private set; }
        public List<Wall> NextWalls { get; private set; }
        private Image BG { get; set; }

        private readonly static string[] Tips = [
            "Если нажать кнопку 'ESC', игра закроется, и ты, наконец, обретёшь покой!", 
            "Установнен новый рекорд по количеству потраченных минут жизни!", 
            "А может эта шипованая стена стоит там не просто так?",
            "Каждый неудачный отскок приближает тебя к просветлению!\nИли к нервному срыву. Как посмотреть."
        ];
        public string currentTip = Tips[0];
        private string nextTip;
        public RectangleF[] bgBounds = new RectangleF[6];
        public int HighScore { get; private set; } = Properties.Settings.Default.HighScore;
        public int Score { get; private set; }
        public bool _isSpikesActive { get; set; }
        public bool OutOfBounds { get; set; } = false;
        public RectangleF FormRectangle { get; set; }
        private readonly Size wallSize = new(20, 128);
        private readonly Size sawSize = new(32, 32);
        private readonly float startMoveBgY;
        private float baseY;

        public GameManager(Image bG, RectangleF formRectangle)
        {
            CurrentWalls = [];
            NextWalls = [];
            FormRectangle = formRectangle;
            startMoveBgY = 300;
            Player = new Player(startMoveBgY);
            BG = bG;
            InitializeBackground();
        }
        public void InitializeBackground()
        {
            bgBounds[0] = new RectangleF(200, 0, BG.Width * 2, BG.Height * 2);
            bgBounds[1] = new RectangleF(200, -FormRectangle.Height, BG.Width * 2, BG.Height * 2);
            bgBounds[2] = new RectangleF(FormRectangle.Width / 2, 200, BG.Width * 2, BG.Height * 2);
            bgBounds[3] = new RectangleF(FormRectangle.Width / 2, 200 - FormRectangle.Height, BG.Width * 2, BG.Height * 2);
            bgBounds[4] = new RectangleF(FormRectangle.Width - 200, 400, -BG.Width * 2, BG.Height * 2);
            bgBounds[5] = new RectangleF(FormRectangle.Width - 200, 400 - FormRectangle.Height, -BG.Width * 2, BG.Height * 2);
        }
        public void Reset()
        {
            HighScore = Score > HighScore ? Score : HighScore;
            Score = 0;
            currentTip = Tips[0];
            _isSpikesActive = false;
            OutOfBounds = false;
            Player.Reset();
            CurrentWalls.Clear();
            NextWalls.Clear();
            baseY = 0;
            InitializeBackground();
            InitializeFirstStructure();
            GenerateNextStructure();
        }

        private void InitializeFirstStructure() => CurrentWalls = CreateStructure0();

        private List<Wall> CreateStructure0()
        {
            var walls = new List<Wall>();

            walls.Add(new NormalWall(new PointF(403, 531), wallSize));
            walls.Add(new NormalWall(new PointF(500, 400), wallSize));
            walls.Add(new SpikeWall(new PointF(700, 300), wallSize));
            walls.Add(new CircularSaw(new PointF(430, 300), sawSize, -120, -50));
            walls.Add(new BrokenWall(new PointF(800, 180), wallSize));
            walls.Add(new BrokenWall(new PointF(900, 180), wallSize));
            walls.Add(new BrokenWall(new PointF(1000, 180), wallSize));
            walls.Add(new CircularSaw(new PointF(800, 100), sawSize, -100));
            walls.Add(new CircularSaw(new PointF(1000, 100), sawSize, 100));
            walls.Add(new SpikeWall(new PointF(900, -200), wallSize));
            walls.Add(new WinWall(new PointF(800, -500), wallSize));
            walls.Add(new CircularSaw(new PointF(750, -500), sawSize, 0, 100));

            return walls;
        }

        private void GenerateNextStructure()
        {
            Random rnd = new();
            int structureType = rnd.Next(1, 4);
            nextTip = Tips[structureType];
            baseY = CurrentWalls.Min(w => w.Bounds.Top);
            switch (structureType)
            {
                case 1:
                    NextWalls = CreateStructure1();
                    break;
                case 2:
                    NextWalls = CreateStructure2();
                    break;
                case 3:
                    NextWalls = CreateStructure3();
                    break;
            }
        }

        private List<Wall> CreateStructure1()
        {
            var walls = new List<Wall>();

            walls.Add(new NormalWall(new PointF(500, baseY - 300), wallSize));
            walls.Add(new NormalWall(new PointF(200, baseY - 500), wallSize));
            walls.Add(new NormalWall(new PointF(200, baseY - 800), wallSize));

            for (int i = 0; i < 8; i++)
                walls.Add(new BrokenWall(new PointF(400 + 50 * i, baseY - 950), wallSize));

            for (int i = 0; i < 15; i++)
                walls.Add(new CircularSaw(new PointF(i * 50, baseY - 1000), sawSize));

            walls.Add(new BounceWall(new PointF(1000, baseY - 950), wallSize));
            walls.Add(new BounceWall(new PointF(1000, baseY - 1150), wallSize));
            walls.Add(new BounceWall(new PointF(1000, baseY - 1350), wallSize));

            walls.Add(new NormalWall(new PointF(800, baseY - 1550), wallSize));
            walls.Add(new CircularSaw(new PointF(850, baseY - 1550), sawSize, 0, 100));

            walls.Add(new NormalWall(new PointF(600, baseY - 1750), wallSize));
            walls.Add(new CircularSaw(new PointF(550, baseY - 1750), sawSize, 0, 100));

            walls.Add(new NormalWall(new PointF(850, baseY - 1950), wallSize));
            walls.Add(new CircularSaw(new PointF(900, baseY - 1950), sawSize, 0, 100));

            walls.Add(new NormalWall(new PointF(1100, baseY - 2150), wallSize));
            walls.Add(new WinWall(new PointF(700, baseY - 2350), wallSize));

            return walls;
        }

        private List<Wall> CreateStructure2()
        {
            var walls = new List<Wall>();

            walls.Add(new NormalWall(new PointF(500, baseY - 300), wallSize));
            walls.Add(new SpikeWall(new PointF(350, baseY - 150), wallSize));
            walls.Add(new BounceWall(new PointF(400, baseY - 150), wallSize));
            walls.Add(new BounceWall(new PointF(200, baseY - 400), wallSize));
            walls.Add(new BounceWall(new PointF(200, baseY - 600), wallSize));
            walls.Add(new BounceWall(new PointF(400, baseY - 500), wallSize));

            for (int i = 0; i < 40; i++)
                walls.Add(new CircularSaw(new PointF(400 + 50 * i, baseY - 550), sawSize));

            for (int i = 0; i < 5; i++)
                walls.Add(new CircularSaw(new PointF(190 - 50 * i, baseY - 450), sawSize));

            walls.Add(new SpikeWall(new PointF(600, baseY - 800), wallSize));
            walls.Add(new BrokenWall(new PointF(850, baseY - 900), wallSize));
            walls.Add(new BrokenWall(new PointF(1000, baseY - 1000), wallSize));
            walls.Add(new BrokenWall(new PointF(1000, baseY - 1300), wallSize));
            walls.Add(new BrokenWall(new PointF(1000, baseY - 1600), wallSize));

            walls.Add(new BounceWall(new PointF(400, baseY - 1400), wallSize));
            walls.Add(new NormalWall(new PointF(100, baseY - 1650), wallSize));

            for (int i = 0; i < 5; i++)
            {
                walls.Add(new BounceWall(new PointF(300, baseY - 1850 - 100 * i), wallSize));
                walls.Add(new BounceWall(new PointF(400, baseY - 1800 - 100 * i), wallSize));
            }

            walls.Add(new BounceWall(new PointF(300, baseY - 1800 - 100 * 5), wallSize));

            for (int i = 0; i < 40; i++)
                walls.Add(new CircularSaw(new PointF(450 + 50 * i, baseY - 2200), sawSize));

            walls.Add(new BounceWall(new PointF(500, baseY - 2600), wallSize));
            walls.Add(new NormalWall(new PointF(800, baseY - 2700), wallSize));
            walls.Add(new NormalWall(new PointF(1000, baseY - 2900), wallSize));
            walls.Add(new NormalWall(new PointF(1000, baseY - 3200), wallSize));
            walls.Add(new WinWall(new PointF(700, baseY - 3300), wallSize));

            return walls;
        }

        private List<Wall> CreateStructure3()
        {
            var walls = new List<Wall>();

            walls.Add(new NormalWall(new PointF(500, baseY - 300), wallSize));

            for (int i = 0; i < 10; i++)
                walls.Add(new BounceWall(new PointF(800, baseY - 600 - 100 * i), wallSize));

            for (int i = 0; i < 16; i++)
                walls.Add(new CircularSaw(new PointF(650, baseY - 700 - 50 * i), sawSize));

            walls.Add(new SpikeWall(new PointF(300, baseY - 1500), wallSize));
            walls.Add(new SpikeWall(new PointF(300, baseY - 1800), wallSize));
            walls.Add(new BounceWall(new PointF(500, baseY - 2000), wallSize));
            walls.Add(new BounceWall(new PointF(800, baseY - 2000), wallSize));
            walls.Add(new SpikeWall(new PointF(1100, baseY - 2300), wallSize));
            walls.Add(new WinWall(new PointF(700, baseY - 2500), wallSize));

            return walls;
        }

        public void Update()
        {
            if (Player == null || CurrentWalls == null || NextWalls == null) return;

            Player.ApplyGravity();
            CheckOutOfBounds();
            UpdateSaws();
            if (Player.IsDead) return;

            if (Player.IsMoving) Player.HandleMovement();
            CheckCollisions();
            MoveBackground();
        }

        private void CheckCollisions()
        {
            Player.IsSliding = false;
            foreach (var wall in CurrentWalls.Concat(NextWalls).Where(w => w.IsActive))
            {
                if (Player.Bounds.IntersectsWith(wall.Bounds))
                {
                    wall.HandleCollision(Player);
                    if (wall is WinWall winWall && !winWall.IsTriggered)
                    {
                        currentTip = nextTip;
                        Score++;
                        winWall.IsTriggered = true;
                    }
                }
            }
            Player.IsLastSliding = Player.IsSliding;
            if (!Player.IsSliding)
            {
                foreach (var wall in CurrentWalls.OfType<BrokenWall>().Concat(NextWalls.OfType<BrokenWall>()))
                {
                    if (wall.IsBroken) wall.IsActive = false;
                }
            }
        }

        private void CheckOutOfBounds()
        {
            if (!Player.Bounds.IntersectsWith(FormRectangle)) OutOfBounds = true;
        }
        private void UpdateSaws()
        {
            foreach (var saw in CurrentWalls.OfType<CircularSaw>().Concat(NextWalls.OfType<CircularSaw>()))
                saw.Update();
        }

        private void MoveBackground()
        {
            if (Player.Position.Y < startMoveBgY && Player.VerticalVelocity < 0)
            {
                float moveAmount = Player.VerticalVelocity;
                foreach (var wall in CurrentWalls.Concat(NextWalls))
                {
                    if (wall is CircularSaw saw)
                        saw.startPosition = new PointF(saw.startPosition.X, saw.startPosition.Y - moveAmount);
                    wall.Bounds = new RectangleF(
                        wall.Bounds.X,
                        wall.Bounds.Y - moveAmount,
                        wall.Bounds.Width,
                        wall.Bounds.Height
                    );
                }
                for (int i = 0; i < 6; i++)
                {
                    var bg = bgBounds[i];
                    bgBounds[i] = new RectangleF(
                        bg.X,
                        bg.Y - moveAmount,
                        bg.Width,
                        bg.Height
                    );
                    if (bg.Y > FormRectangle.Height)
                    {
                        Random rnd = new Random();
                        bgBounds[i] = new RectangleF(
                            rnd.Next(200, (int)FormRectangle.Width - 200),
                            -FormRectangle.Height,
                            bg.Width,
                            bg.Height
                         );
                    }
                }
                baseY -= moveAmount;
                if (baseY> FormRectangle.Height)
                {
                    CurrentWalls = NextWalls;
                    GenerateNextStructure();
                }
            }
        }

        public void ToggleSpikes()
        {
            _isSpikesActive = !_isSpikesActive;

            if (_isSpikesActive) AddSpikes();
            else RemoveSpikes();
        }

        private void AddSpikes()
        {
            var spikesToAdd = new List<Wall>();

            foreach (var wall in CurrentWalls.OfType<SpikeWall>().Concat(NextWalls.OfType<SpikeWall>()))
            {
                spikesToAdd.Add(new Spikes(
                    new PointF(wall.Bounds.Left - 22, wall.Bounds.Top),
                    new Size((int)wall.Bounds.Width, (int)wall.Bounds.Height)
                ));

                spikesToAdd.Add(new Spikes(
                    new PointF(wall.Bounds.Left + 40, wall.Bounds.Top),
                    new Size((int)-wall.Bounds.Width, (int)wall.Bounds.Height)
                ));
            }

            CurrentWalls.AddRange(spikesToAdd);
        }

        private void RemoveSpikes() => CurrentWalls.RemoveAll(w => w is Spikes);
    }
}