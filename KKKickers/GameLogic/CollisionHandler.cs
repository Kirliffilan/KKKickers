using KKKickers.GameObjects;
using KKKickers.GameObjects.Walls;

namespace KKKickers.GameLogic
{
    public class CollisionHandler
    {
        public static void Initialize(RectangleF formRectangle)
        {
            if (_instance != null)
                throw new InvalidOperationException("CollisionHandler already initialized.");
            _instance = new(formRectangle);
        }

        private static CollisionHandler _instance;
        public static CollisionHandler Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("CollisionHandler not initialized. Call Initialize() first.");
                return _instance;
            }
        }
        private bool _outOfBounds;
        public bool OutOfBounds => _outOfBounds;
        private RectangleF _formRectangle;

        private CollisionHandler(RectangleF formRectangle)
        {
            _outOfBounds = false;
            _formRectangle = formRectangle;
        }

        public void SetFormRectangle(RectangleF formRectangle) => _formRectangle = formRectangle;

        public void Reset() => _outOfBounds = false;
        public void CheckOutOfBounds()
        {
            if (!Player.Instance.Bounds.IntersectsWith(_formRectangle))
                _outOfBounds = true;
        }

        public void CheckCollisions()
        {
            Player player = Player.Instance;
            StructureGenerator structureGenerator = StructureGenerator.Instance;

            player.IsSliding = false;
            foreach (var wall in structureGenerator.CurrentStructure.Concat(structureGenerator.NextStructure).Concat(structureGenerator.Spikes).Where(w => w.IsActive))
            {
                if (player.Bounds.IntersectsWith(wall.Bounds))
                {
                    wall.HandleCollision(player);
                    if (wall is WinWall winWall && !winWall.IsTriggered)
                    {
                        ScoreHandler.Instance.AddScore();
                        UIHandler.Instance.UpdateTip();
                        winWall.IsTriggered = true;
                    }
                }
            }
            player.IsLastSliding = player.IsSliding;
            if (!player.IsSliding)
            {
                foreach (var wall in structureGenerator.CurrentStructure.OfType<BrokenWall>().Concat(structureGenerator.NextStructure.OfType<BrokenWall>()))
                {
                    if (wall.IsBroken) wall.IsActive = false;
                }
            }
        }
    }
}