using UnityEngine;

namespace Match3.Commons
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

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
	}
}