using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

[System.Serializable]
public class HintInfo
{
    public BaseGem gem;
    public BaseGem currentSwap;
    public List<BaseGem> swaps = new List<BaseGem>();

    public HintInfo(BaseGem gem) {
        this.gem = gem;
    }
}

public class HintController : SingletonMonoBehaviour<HintController> {

    List<HintInfo> hints = new List<HintInfo>();
    HintInfo currentHint;
    Coroutine hinting;

    public float hintDelay = 30f;

    public static bool hasHints {
        get { return Instance.hints.Count > 0; }
    }

    public static bool isShowing {
        get { return Instance.currentHint != null; }
    }

    public static bool paused;
    
    HintInfo GetHint(BaseGem gem, BaseGem otherGem) {
        if(!(gem && otherGem))
            return null;

        HintInfo hintInfo = null;

        HintInfo hintA = hints.Find(h => h.gem == gem);
        HintInfo hintB = hints.Find(h => h.gem == otherGem);

        BoardController.SwapGems(gem, otherGem);

        MatchInfo matchA = gem.GetMatch();
        MatchInfo matchB = otherGem.GetMatch();

        if(matchA.isValid) {
            hintInfo = hintA != null ? hintA : new HintInfo(gem);
            hintInfo.swaps.Add(otherGem);
        } else if(matchB.isValid) {
            hintInfo = hintB != null ? hintB : new HintInfo(otherGem);
            hintInfo.swaps.Add(gem);
        }

        BoardController.SwapGems(gem, otherGem);

        return hintInfo;
    }

    public static void FindHints() {
        Instance.hints.Clear();

        for(int j = 0; j < BoardController.height; ++j) {
            for(int i = 0; i < BoardController.width; ++i) {
                BaseGem gem = BoardController.GetGem(i, j);

                // Swap Right
                BaseGem otherGem = BoardController.GetGem(i + 1, j);
                if(otherGem && otherGem.type != gem.type) {
                    HintInfo hintInfo = Instance.GetHint(gem, otherGem);

                    if(hintInfo != null && !Instance.hints.Contains(hintInfo))
                        Instance.hints.Add(hintInfo);
                }

                // Swap Up
                otherGem = BoardController.GetGem(i, j + 1);
                if(otherGem && otherGem.type != gem.type) {
                    HintInfo hintInfo = Instance.GetHint(gem, otherGem);

                    if(hintInfo != null && !Instance.hints.Contains(hintInfo))
                        Instance.hints.Add(hintInfo);
                }
            }
        }
    }

    public static void ShowHint() {
        if(hasHints && !isShowing) {
            HintInfo hintInfo = Instance.hints[
                Random.Range(0, Instance.hints.Count)
            ];
            hintInfo.gem.Hint();

            hintInfo.currentSwap = hintInfo.swaps[
                Random.Range(0, hintInfo.swaps.Count)
            ];
            hintInfo.currentSwap.Hint();
            Instance.currentHint = hintInfo;
        }
    }

    public static void StopCurrentHint() {
        if(isShowing) {
            Instance.currentHint.gem.Hint(false);
            Instance.currentHint.currentSwap.Hint(false);
            Instance.currentHint = null;
        }
    }

    public static void StartHinting() {
        if(Instance.hinting == null && !isShowing)
            Instance.hinting = Instance.StartCoroutine(
                Instance.IEStartHinting()
            );
    }

    public static void StopHinting() {
        if(Instance.hinting != null)
            Instance.StopCoroutine(Instance.hinting);
        
        Instance.hinting = null;
    }

    IEnumerator IEStartHinting() {

        paused = false;
        yield return new WaitForSecondsAndNotPaused(hintDelay, () => paused);

        ShowHint();
        Instance.hinting = null;
    }
}
