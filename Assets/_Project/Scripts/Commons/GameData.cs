using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

public enum GemType {
    Milk,
    Apple,
    Orange,
    Bread,
    Lettuce,
    Coconut,
    Flower,
    Special
}

[System.Serializable]
public class GemData {
    public GemType type;
    public Sprite sprite;
    public int minMatch = 3;
}

[System.Serializable]
public class SpecialGemData {
    public string name;
    public GameObject prefab;
}

[System.Serializable]
public class AudioClipInfo {
    public string name;
    public AudioClip clip;
}

[CreateAssetMenu(fileName = "GameData", menuName = "Match3/GameData", order = 1)]
public class GameData : ScriptableObject 
{
    
    [SerializeField]
    List<GemData> gems = new List<GemData>();
    [SerializeField]
    List<SpecialGemData> specialGems = new List<SpecialGemData>();
    [SerializeField]
    List<AudioClipInfo> audioClipInfos = new List<AudioClipInfo>();
    [SerializeField]
    string[] comboMessages;
    public int maxCombo {
        get { return comboMessages.Length; }
    }

    public GemData GemOfType(GemType type) {
        return gems.Find(gem => gem.type == type);
    }

    public GemData RandomGem() {
        return Miscellaneous.Choose(gems);
    }

    public GameObject GetSpecialGem(string name) {
        SpecialGemData sgd = specialGems.Find(gem => gem.name == name);
        if(sgd != null)
            return sgd.prefab;

        return null;
    }

    public AudioClip GetAudioClip(string name) {
        AudioClipInfo audioClipInfo = audioClipInfos.Find(
            aci => aci.name == name
        );

        if(audioClipInfo != null)
            return audioClipInfo.clip;

        return null;
    }

    public string GetComboMessage(int combo) {
        return comboMessages[combo];
    }
}
