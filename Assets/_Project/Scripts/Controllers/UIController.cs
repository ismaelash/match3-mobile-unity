using System.Collections;
using System.Collections.Generic;
using Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using IsmaelNascimento.Controllers;

public class UIController : SingletonMonoBehaviour<UIController> {

    [Header("Screens")]
    [SerializeField]
    CanvasGroup mainScreen;
    [SerializeField]
    CanvasGroup gameScreen;

    [Header("Game Screen")]

    [SerializeField]
    TextMeshProUGUI scoreText;

    [SerializeField]
    TextMeshProUGUI comboScoreText;

    [SerializeField]
    TextMeshProUGUI comboMultiplierText;

    [SerializeField]
    TextMeshProUGUI goalScoreText;
    [SerializeField]
    TextMeshProUGUI timeLeftText;
    [SerializeField]
    TextMeshProUGUI highscoreText;
    [SerializeField]
    TextMeshProUGUI msgText;

    CanvasGroup currentScreen;

    float timePulse;

    public static void ShowMainScreen() {
        //UpdateHighScore(GameController.highscore);
        Instance.StartCoroutine(
            Instance.IEChangeScreen(Instance.mainScreen, executeAfter: () => {
                GameController.Instance.ShowGemMenu();
            })
        );
    }

    public static void ShowGameScreen() {
        
        UpdateScore(GameController.Instance.Score);
        UpdateGoalScore(GameController.Instance.CurrentGoalScore);
        UpdateTimeLeft(GameController.Instance.TimeLeft);
        Instance.StartCoroutine(
            Instance.IEChangeScreen(Instance.gameScreen, () => {
                GameController.Instance.ShowGemMenu(false);
            })
        );
    }

    public static void UpdateScore(int score) {
        Instance.scoreText.text = $"{ score }";
        //instance.scoreText.transform.parent
        //    .GetComponent<Animator>().SetTrigger("pulse");
    }

    public static void UpdateComboScore(int comboScore, int multiplier) {
        Instance.comboScoreText.text = $"+{ comboScore / Mathf.Max(multiplier, 1) }";
        Instance.comboMultiplierText.text = multiplier > 1 ? $" x{ multiplier }" : "";

        Instance.comboScoreText.GetComponent<Animator>().SetTrigger("pulse");
    }

    
    //public static void UpdateHighScore(int score) {
    //    Instance.highscoreText.text = $"High Score: { score }";
    //}

    public static void UpdateGoalScore(int goalScore) {
        Instance.goalScoreText.text = $"/{ goalScore }";
        Instance.goalScoreText.GetComponent<Animator>().SetTrigger("pulse");
    }

    public static void UpdateTimeLeft(float timeLeft) {
        //if(timeLeft <= 30) {
        //    if(Time.time - Instance.timePulse > 1f) {
        //        Instance.timeLeftText.GetComponent<Animator>().SetTrigger("pulse");
        //        Instance.timePulse = Time.time;
        //        SoundController.PlaySfxInstance(GameData.GetAudioClip("click"));
        //    }
        //} else {
        //    Instance.timePulse = 0;
        //}
        
        System.TimeSpan timeSpan = System.TimeSpan.FromSeconds(timeLeft);
        string mm = timeSpan.Minutes.ToString("D2");
        string ss = timeSpan.Seconds.ToString("D2");
        Instance.timeLeftText.text = $"{ mm }:{ ss }";
    }

    public static void ShowMessage(string msg) {
        Instance.msgText.text = $"{ msg }";
        Instance.msgText.transform.GetComponent<Animator>().SetTrigger("pulse");
    }

    IEnumerator IEChangeScreen(
        CanvasGroup screen,
        System.Action executeBefore = null, System.Action executeAfter = null
    ) {
        executeBefore?.Invoke();

        screen.alpha = 0;
        screen.gameObject.SetActive(false);

        if(currentScreen) {
            while(currentScreen.alpha > 0) {
                currentScreen.alpha -= Time.deltaTime * 2;
                yield return null;
            }
            currentScreen.gameObject.SetActive(false);
        }

        currentScreen = screen;
        currentScreen.gameObject.SetActive(true);

        while(currentScreen.alpha < 1) {
            currentScreen.alpha += Time.deltaTime * 2;
            yield return null;
        }

        executeAfter?.Invoke();
    }
}