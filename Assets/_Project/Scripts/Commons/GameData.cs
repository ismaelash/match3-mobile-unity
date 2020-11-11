using System.Collections.Generic;
using UnityEngine;

namespace Match3.Commons
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Match3/GameData")]
    public class GameData : ScriptableObject
    {
        #region ENUMS

        public enum GemType
        {
            Milk,
            Apple,
            Orange,
            Bread,
            Lettuce,
            Coconut,
            Flower
        }

        #endregion

        #region VARIABLES

        [SerializeField] private List<GemData> gems = new List<GemData>();
        [SerializeField] private List<AudioClipInfo> audioClipInfos = new List<AudioClipInfo>();

        #endregion

        #region PUBLIC_METHODS

        public GemData RandomGem()
        {
            return Miscellaneous.Choose(gems);
        }

        public AudioClip GetAudioClip(string name)
        {
            AudioClipInfo audioClipInfo = audioClipInfos.Find(audioClipInfoData => audioClipInfoData.name == name);

            if (audioClipInfo != null)
            {
                return audioClipInfo.clip;
            }

            return null;
        }

        #endregion
    }
}