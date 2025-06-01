using KKKickers.GameObjects;
using KKKickers.GameObjects.Walls;

namespace KKKickers.GameLogic
{
    public class StructureGenerator
    {
        public static void Initialize(float formHeight)
        {
            if (_instance != null)
                throw new InvalidOperationException("BackgroundUpdator already initialized.");
            _instance = new(formHeight);
        }

        private static StructureGenerator _instance;
        public static StructureGenerator Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("BackgroundUpdator not initialized. Call Initialize() first.");
                return _instance;
            }
        }

        private readonly Size _wallSize = new(20, 128);
        private readonly Size _sawSize = new(32, 32);
        private float _baseY;
        private int _previousStructureType;
        private float _formHeight;

        private List<Wall> _currentStructure;
        private List<Wall> _nextStructure;
        private List<Wall> _spikes;

        public List<Wall> CurrentStructure => _currentStructure;
        public List<Wall> NextStructure => _nextStructure;
        
        public List<Wall> Spikes => _spikes;

        private StructureGenerator(float formHeight) => _formHeight = formHeight;

        public void SetFormHeight(float formHeight) => _formHeight = formHeight;

        public void Reset()
        {
            InitializeFirstStructure();
            GenerateNextStructure();
        }

        public void InitializeFirstStructure() => _currentStructure = CreateStructure0();

        public void GenerateNextStructure()
        {
            Random rnd = new();

            int structureType = _previousStructureType;
            while (structureType == _previousStructureType) 
                structureType = rnd.Next(1, 4);

            UIHandler.Instance.ChooseNextTip(structureType);

            _baseY = _currentStructure.Min(w => w.Bounds.Top);
            switch (structureType)
            {
                case 1:
                    _nextStructure = CreateStructure1();
                    break;
                case 2:
                    _nextStructure = CreateStructure2();
                    break;
                case 3:
                    _nextStructure = CreateStructure3();
                    break;
            }

            _spikes = [];
            AddSpikes();

            _previousStructureType = structureType;
        }

        private List<Wall> CreateStructure0()
        {
            _previousStructureType = 0;
            var structure = new List<Wall>
            {
                new NormalWall(new PointF(403, 531), _wallSize),
                new NormalWall(new PointF(500, 400), _wallSize),
                new SpikeWall(new PointF(700, 300), _wallSize),
                new CircularSaw(new PointF(430, 300), _sawSize, -120, -50),
                new BrokenWall(new PointF(800, 180), _wallSize),
                new BrokenWall(new PointF(900, 180), _wallSize),
                new BrokenWall(new PointF(1000, 180), _wallSize),
                new CircularSaw(new PointF(800, 100), _sawSize, -100),
                new CircularSaw(new PointF(1000, 100), _sawSize, 100),
                new SpikeWall(new PointF(900, -200), _wallSize),
                new WinWall(new PointF(800, -500), _wallSize),
                new CircularSaw(new PointF(750, -500), _sawSize, 0, 100)
            };

            return structure;
        }

        private List<Wall> CreateStructure1()
        {
            var structure = new List<Wall>
            {
                new NormalWall(new PointF(500, _baseY - 300), _wallSize),
                new NormalWall(new PointF(200, _baseY - 500), _wallSize),
                new NormalWall(new PointF(200, _baseY - 800), _wallSize)
            };
            
            for (int i = 0; i < 8; i++)
                structure.Add(new BrokenWall(new PointF(400 + 50 * i, _baseY - 950), _wallSize));
            for (int i = 0; i < 15; i++)
                structure.Add(new CircularSaw(new PointF(i * 50, _baseY - 1000), _sawSize));

            structure.Add(new BounceWall(new PointF(1000, _baseY - 950), _wallSize));
            structure.Add(new BounceWall(new PointF(1000, _baseY - 1150), _wallSize));
            structure.Add(new BounceWall(new PointF(1000, _baseY - 1350), _wallSize));
            structure.Add(new NormalWall(new PointF(800, _baseY - 1550), _wallSize));
            structure.Add(new CircularSaw(new PointF(850, _baseY - 1550), _sawSize, 0, 100));
            structure.Add(new NormalWall(new PointF(600, _baseY - 1750), _wallSize));
            structure.Add(new CircularSaw(new PointF(550, _baseY - 1750), _sawSize, 0, 100));
            structure.Add(new NormalWall(new PointF(850, _baseY - 1950), _wallSize));
            structure.Add(new CircularSaw(new PointF(900, _baseY - 1950), _sawSize, 0, 100));
            structure.Add(new NormalWall(new PointF(1100, _baseY - 2150), _wallSize));
            structure.Add(new WinWall(new PointF(700, _baseY - 2350), _wallSize));
            
            return structure;
        }

        private List<Wall> CreateStructure2()
        {
            var structure = new List<Wall>
            {
                new NormalWall(new PointF(500, _baseY - 300), _wallSize),
                new SpikeWall(new PointF(350, _baseY - 150), _wallSize),
                new BounceWall(new PointF(400, _baseY - 150), _wallSize),
                new BounceWall(new PointF(200, _baseY - 400), _wallSize),
                new BounceWall(new PointF(200, _baseY - 600), _wallSize),
                new BounceWall(new PointF(400, _baseY - 500), _wallSize)
            };

            for (int i = 0; i < 40; i++)
                structure.Add(new CircularSaw(new PointF(400 + 50 * i, _baseY - 550), _sawSize));
            for (int i = 0; i < 5; i++)
                structure.Add(new CircularSaw(new PointF(190 - 50 * i, _baseY - 450), _sawSize));

            structure.Add(new SpikeWall(new PointF(600, _baseY - 800), _wallSize));
            structure.Add(new BrokenWall(new PointF(850, _baseY - 900), _wallSize));
            structure.Add(new BrokenWall(new PointF(1000, _baseY - 1000), _wallSize));
            structure.Add(new BrokenWall(new PointF(1000, _baseY - 1300), _wallSize));
            structure.Add(new BrokenWall(new PointF(1000, _baseY - 1600), _wallSize));
            structure.Add(new BounceWall(new PointF(400, _baseY - 1400), _wallSize));
            structure.Add(new NormalWall(new PointF(100, _baseY - 1650), _wallSize));

            for (int i = 0; i < 5; i++)
            {
                structure.Add(new BounceWall(new PointF(300, _baseY - 1850 - 100 * i), _wallSize));
                structure.Add(new BounceWall(new PointF(400, _baseY - 1800 - 100 * i), _wallSize));
            }

            structure.Add(new BounceWall(new PointF(300, _baseY - 1800 - 100 * 5), _wallSize));

            for (int i = 0; i < 40; i++)
                structure.Add(new CircularSaw(new PointF(450 + 50 * i, _baseY - 2200), _sawSize));

            structure.Add(new BounceWall(new PointF(500, _baseY - 2600), _wallSize));
            structure.Add(new NormalWall(new PointF(800, _baseY - 2700), _wallSize));
            structure.Add(new NormalWall(new PointF(1000, _baseY - 2900), _wallSize));
            structure.Add(new NormalWall(new PointF(1000, _baseY - 3200), _wallSize));
            structure.Add(new WinWall(new PointF(700, _baseY - 3300), _wallSize));

            return structure;
        }

        private List<Wall> CreateStructure3()
        {
            var structure = new List<Wall>
            {
                new NormalWall(new PointF(500, _baseY - 300), _wallSize)
            };

            for (int i = 0; i < 10; i++)
                structure.Add(new BounceWall(new PointF(800, _baseY - 600 - 100 * i), _wallSize));
            for (int i = 0; i < 16; i++)
                structure.Add(new CircularSaw(new PointF(650, _baseY - 700 - 50 * i), _sawSize));

            structure.Add(new SpikeWall(new PointF(300, _baseY - 1500), _wallSize));
            structure.Add(new SpikeWall(new PointF(300, _baseY - 1800), _wallSize));
            structure.Add(new BounceWall(new PointF(500, _baseY - 2000), _wallSize));
            structure.Add(new BounceWall(new PointF(800, _baseY - 2000), _wallSize));
            structure.Add(new SpikeWall(new PointF(1100, _baseY - 2300), _wallSize));
            structure.Add(new WinWall(new PointF(700, _baseY - 2500), _wallSize));

            return structure;
        }

        public void UpdateSaws()
        {
            foreach (var saw in CurrentStructure.OfType<CircularSaw>().Concat(NextStructure.OfType<CircularSaw>()))
                saw.Update();
        }

        private void AddSpikes()
        {
            foreach (var wall in CurrentStructure.Concat(NextStructure).OfType<SpikeWall>())
            {
                _spikes.Add(new Spikes(
                    new PointF(wall.Bounds.Left - 22, wall.Bounds.Top),
                    new Size((int)wall.Bounds.Width, (int)wall.Bounds.Height)
                    ));
                _spikes.Add(new Spikes(
                    new PointF(wall.Bounds.Left + 40, wall.Bounds.Top),
                    new Size((int)-wall.Bounds.Width, (int)wall.Bounds.Height)
                ));
            }
        }

        public void DeactivateSpikes()
        {
            foreach (var spike in _spikes) spike.IsActive = false;
        }
        public void ActivateSpikes()
        {
            foreach (var spike in _spikes) spike.IsActive = true;
        }

        public void Move()
        {
            Player player = Player.Instance;
            if (player.Position.Y < player.StartMoveBgY && player.VerticalVelocity < 0)
            {
                float moveAmount = player.VerticalVelocity;
                foreach (var wall in _currentStructure.Concat(_nextStructure).Concat(Spikes))
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
                _baseY -= moveAmount;
                if (_baseY > _formHeight)
                {
                    _currentStructure = _nextStructure;
                    GenerateNextStructure();
                }
            }
        }
    }
}
