using Match3.Commons;
using System;

namespace Match3.Controllers
{
    public class EventController : Singleton<EventController>
    {
        #region VARIABLES

        public Action OnStartGameAction;
        public Action OnEndGameplayAction;
        public Action<int> OnNewScoreAction;
        public Action<int> OnNewGoalScore;
        public Action<int> OnAttempMatch;
        public Action OnAttempMatchLimit;
        public Action<GemData> OnGemMatch;

        #endregion
    }
}