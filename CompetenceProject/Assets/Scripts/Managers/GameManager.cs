using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public int collectiblesGathered;

    public static GameManager instance;
    public UIManager uiManager;
    public GameObject playerExplosionPrefab;

    public static AudioBank audioBank;
    public AudioSource audioSource;

    public GameObject player;

    public float Score {
        get {
            return Mathf.Round(player.transform.position.z * 100f) / 100f;
        }
    }

    public bool charactersCanMove {
        get;
        private set;

    }

    private void Awake() {
        if (instance == null) instance = this;
        else Destroy(this);

        player = GameObject.FindGameObjectWithTag("Player");

        uiManager = FindObjectOfType<UIManager>();
        audioBank = FindObjectOfType<AudioBank>();
    }

    private void Start() {
        collectiblesGathered = 0;

        charactersCanMove = false;

        if (PlayerPrefs.HasKey("Tutorial")) {
            if (PlayerPrefs.GetInt("Tutorial") == 0) {
                StartGame();
            }
        }
    }

    public void StartGame() {
        StartCoroutine("StartCountdown");
    }

    IEnumerator StartCountdown() {
        uiManager.EnableCountdownText();
        charactersCanMove = true;
        int timer = 3;
        while (timer > 0) {
        uiManager.SetCountdownText(timer.ToString());
        timer--;
        yield return new WaitForSeconds(1f);
        }
        uiManager.SetCountdownText("Go");
        yield return new WaitForSeconds(1f);
        uiManager.DisableCountdownText();
        charactersCanMove = true;
    }

    public void GatherCollectible(GameObject collectible, Animator animator) {
        instance.collectiblesGathered++;
        uiManager.SetScoreText(collectiblesGathered);
        animator.Play("CollectiblesDeath");
        AudioSource audioSource = collectible.GetComponent<AudioSource>();
        audioSource.clip = audioBank.collectibleDeath;
        audioSource.Play();
    }

    public void KillPlayer(GameObject _player) {
        charactersCanMove = false;
        StartCoroutine("PlayerDeath", _player);
    }

    IEnumerator PlayerDeath(GameObject _player) {
        AudioSource audioSource = _player.GetComponent<AudioSource>();
        audioSource.clip = audioBank.playerDeath;
        audioSource.Play();
        yield return new WaitForSeconds(0.34f);
        player.SetActive(false);
        GameObject explosion = Instantiate(playerExplosionPrefab, new Vector3(_player.transform.position.x, _player.transform.position.y+1.5f, _player.transform.position.z-0.275f), Quaternion.identity) as GameObject;
        yield return new WaitForSeconds(1f);
        uiManager.OpenDeathPanel();
        Destroy(explosion);
    }

    public void UpdateHighscore() {
       float _score = Score;
        if (!PlayerPrefs.HasKey("Highscore")) {
            PlayerPrefs.SetFloat("Highscore", _score);
        }

        if (_score > PlayerPrefs.GetFloat("Highscore")) {
            PlayerPrefs.SetFloat("Highscore", _score);
        }

    }

    public void PlayerVictory() {
        uiManager.OpenVictoryPanel();
        charactersCanMove = false;
    }

    #region Scenes

    public void ChangeScene(string newScene) {
        SceneManager.LoadScene(newScene);
    }

    public void NextLevel() {
        string currentLevel = SceneManager.GetActiveScene().name;
        if (currentLevel.Contains("Level")) {
            int levelIndex = int.Parse(currentLevel.Substring(currentLevel.Length-1));
            if (levelIndex < 5) {
                string nextLevel = "Level" + (levelIndex + 1).ToString();
                print(nextLevel);
                SceneManager.LoadScene(nextLevel);
            }
            else if (levelIndex == 5)
                SceneManager.LoadScene("MainMenu");
        }
    }

    public void ReloadScene() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame() {
        Application.Quit();
    }

    #endregion

    public void PauseGame() {
        charactersCanMove = false;
        Animator[] animatorsInTheScene = FindObjectsOfType(typeof(Animator)) as Animator[];
        foreach (Animator animatorItem in animatorsInTheScene) {
            animatorItem.enabled = false;
        }
    }

    public void ResumeGame() {
        charactersCanMove = true;
        Animator[] animatorsInTheScene = FindObjectsOfType(typeof(Animator)) as Animator[];
        foreach (Animator animatorItem in animatorsInTheScene) {
            animatorItem.enabled = true;
        }
    }

    public static AudioBank GetAudioBank {
        get { return audioBank; }
    }
}
