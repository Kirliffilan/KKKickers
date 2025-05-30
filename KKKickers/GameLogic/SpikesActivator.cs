namespace KKKickers.GameLogic
{
    public class SpikesActivator
    {
        public static void Initialize()
        {
            if (_instance != null)
                throw new InvalidOperationException("SpikesActivator already initialized.");
            _instance = new();
        }

        private static SpikesActivator _instance;
        public static SpikesActivator Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("SpikesActivator not initialized. Call Initialize() first.");
                return _instance;
            }
        }

        private bool _isSpikesActive;

        private SpikesActivator() => _isSpikesActive = true;

        public void Reset()
        {
            _isSpikesActive = true;
            RemoveSpikes();
        }

        public void ToggleSpikes()
        {
            _isSpikesActive = !_isSpikesActive;
            if (_isSpikesActive) RemoveSpikes();
            else AddSpikes();
        }

        private void AddSpikes() => StructureGenerator.Instance.ActivateSpikes();
        private void RemoveSpikes() => StructureGenerator.Instance.DeactivateSpikes();
    }
}
