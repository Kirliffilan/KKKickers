namespace KKKickers.GameLogic
{
    public class ScoreHandler
    {
        public static void Initialize()
        {
            if (_instance != null)
                throw new InvalidOperationException("ScoreHandler already initialized.");
            _instance = new();
        }
        private static ScoreHandler _instance;
        public static ScoreHandler Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("ScoreHandler not initialized. Call Initialize() first.");
                return _instance;
            }
        }

        public int Score { get; private set; }
        public int HighScore { get; private set; }

        private ScoreHandler()
        {
            Score = 0;
            HighScore = Properties.Settings.Default.HighScore;
        }

        public void AddScore() => Score++;

        public void Reset()
        {
            if (Score > HighScore)
            {
                HighScore = Score;
                Properties.Settings.Default.HighScore = HighScore;
                Properties.Settings.Default.Save();
            }
            Score = 0;
        }
    }
}