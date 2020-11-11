using UnityEngine;

namespace Match3.Commons
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        #region VARIABLES
        
        public static T Instance { get; private set; }

        #endregion

        #region MONOBEHAVIOUR

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
            }
            else
            {
                Destroy(this);
            }
        }

        #endregion
    }
}