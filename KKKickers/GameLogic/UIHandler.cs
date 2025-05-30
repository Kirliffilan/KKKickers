using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKKickers.GameLogic
{
    public class UIHandler
    {
        public static void Initialize()
        {
            if (_instance != null)
                throw new InvalidOperationException("SpikesActivator already initialized.");
            _instance = new();
        }

        private static UIHandler _instance;
        public static UIHandler Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException("SpikesActivator not initialized. Call Initialize() first.");
                return _instance;
            }
        }
        private readonly static string[] Tips = [
           "Если нажать кнопку 'ESC', игра закроется, и ты, наконец, обретёшь покой!",
            "Установнен новый рекорд по количеству потраченных минут жизни!",
            "А может эта шипованая стена стоит там не просто так?",
            "Каждый неудачный отскок приближает тебя к просветлению!\nИли к нервному срыву. Как посмотреть."
        ];
        private readonly string _highscoreTip = "Новый рекорд!\nНовое поражение..";
        private string _currentTip;
        public string CurrentTip => _currentTip;
        private string _nextTip;

        private UIHandler() => Reset();

        public void Reset() => _currentTip = Tips[0];
        public void ShowHighScoreTip() => _currentTip = _highscoreTip;
        public void ChooseNextTip(int i) => _nextTip = Tips[i];
        public void SwapTips() => _currentTip = _nextTip;
    }
}
