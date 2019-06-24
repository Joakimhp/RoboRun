using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public Text scoreText;
    public Text finalScore;
    public Text highscoreText;
    public Text HighscoreTextValue;    
    public GameObject tutorialPanelAndroid;
    public GameObject tutorialPanelDesktop;
    public GameObject[] tutorialWindowsAndroid;
    public GameObject[] tutorialWindowsDesktop;
    public int currentTutorialWindowIndex;
    public GameObject pauseMenu;
    public GameObject deathPanel;
    public GameObject victoryPanel;
    public Text countdownText;
    public Text countdownTextShadow;
    public Button pauseButton;

    private void Awake() {

        if (!PlayerPrefs.HasKey("Tutorial")) {
            PlayerPrefs.SetInt("Tutorial", 1);
        }
        

        if (PlayerPrefs.GetInt("Tutorial") == 1) {
            currentTutorialWindowIndex = 0;
            ShowTutorial();
        }

    }

    private void Start() {
        scoreText.text = "Score: 0";
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    private void ShowTutorial() {
        tutorialPanelDesktop.SetActive(true);
        tutorialWindowsDesktop[0].SetActive(true);
    }

    public void ResetTutorial() {
        PlayerPrefs.SetInt("Tutorial", 1);
    }

    public void NextTutorialWindow() {
        if (currentTutorialWindowIndex < 4) {
            tutorialWindowsDesktop[currentTutorialWindowIndex].SetActive(false);
            currentTutorialWindowIndex++;
            tutorialWindowsDesktop[currentTutorialWindowIndex].SetActive(true);
        }
        else {
            tutorialWindowsDesktop[currentTutorialWindowIndex].SetActive(false);
            PlayerPrefs.SetInt("Tutorial", 0);
            GameManager.instance.StartGame();
        }
    }
#elif UNITY_ANDROID || UNITY_IPHONE
    private void ShowTutorial() {
        tutorialPanelAndroid.SetActive(true);
        tutorialWindows[0].SetActive(true);
    }

    public void ResetTutorial() {
        PlayerPrefs.SetInt("Tutorial", 1);
    }

    public void NextTutorialWindow() {
        if (currentTutorialWindowIndex < 4) {
            tutorialWindows[currentTutorialWindowIndex].SetActive(false);
            currentTutorialWindowIndex++;
            tutorialWindows[currentTutorialWindowIndex].SetActive(true);
        }
        else {
            tutorialWindows[currentTutorialWindowIndex].SetActive(false);
            PlayerPrefs.SetInt("Tutorial", 0);
            GameManager.instance.StartGame();
        }
    }

#endif



    private void Update() {
        float score = GameManager.instance.Score;
        SetScoreText(score);
    }

    public void SetCountdownText(string time) {
        countdownText.text = time;
        countdownTextShadow.text = time;
    }

    public void EnableCountdownText() {
        countdownText.enabled = true;
        countdownTextShadow.enabled = true;
        pauseButton.GetComponent<Image>().color = new Color(1, 1, 1, .3f);
        pauseButton.GetComponent<Button>().enabled = false;
    }

    public void DisableCountdownText() {
        countdownText.enabled = false;
        countdownTextShadow.enabled = false;
        pauseButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        pauseButton.GetComponent<Button>().enabled = true;
    }

    public void SetScoreText(float score) {
        scoreText.text = string.Format("Score: {0}",  score);
    }

    public void PauseGame() {
        pauseMenu.SetActive(true);
        GameManager.instance.PauseGame();
    }

    public void ResumeGame() {
        pauseMenu.SetActive(false);
        GameManager.instance.ResumeGame();
    }

    public void OpenDeathPanel() {
        deathPanel.SetActive(true);
        finalScore.text = string.Format(GameManager.instance.Score.ToString());
        scoreText.enabled = false;
        GameManager.instance.UpdateHighscore();
        HighscoreTextValue.text = PlayerPrefs.GetFloat("Highscore").ToString();
    }

    public void OpenVictoryPanel() {
        victoryPanel.SetActive(true);
    }
}


