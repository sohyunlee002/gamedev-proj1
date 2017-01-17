using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class UI : MonoBehaviour {

    Image LoadingBackground;
    Image LoadingLives;
    GameObject Score;
    List<Image> ScoreDigits = new List<Image>();
    int ScorePlaces = 6;
    GameObject Coins;
    List<Image> CoinsDigits = new List<Image>();
    int CoinsPlaces = 2;
    GameObject Timer;
    List<Image> TimerDigits = new List<Image>();
    int TimerPlaces = 3;
    Animator coinAnim;
    string pathToSprites = "NES - Super Mario Bros - Font(Transparent)";
    public Object assetTest;
    Sprite[] numberSprites;
    bool loaded = true;
    float loadTime = 0;
    int playerLives = 1;
    int playerScore = 0;
    int playerTime = 330;
    int playerCoins = 0;
    float timeIncrement = 1;

    // Use this for initialization
    void Start()
    {
        LoadingBackground = GameObject.Find("Loading_Background").GetComponent<Image>();
        LoadingLives = GameObject.Find("Loading_Lives").GetComponent<Image>();
        numberSprites = Resources.LoadAll(pathToSprites).OfType<Sprite>().Take(10).ToArray();
        Score = GameObject.Find("Score");
        foreach (Transform child in Score.transform)
        {
            ScoreDigits.Add(child.GetComponent<Image>());
        }
        Coins = GameObject.Find("Coins");
        foreach (Transform child in Coins.transform)
        {
            CoinsDigits.Add(child.GetComponent<Image>());
        }
        Timer = GameObject.Find("Timer");
        foreach (Transform child in Timer.transform)
        {
            TimerDigits.Add(child.GetComponent<Image>());
        }
        SetDigits(ScoreDigits, ScorePlaces, playerScore);
        SetDigits(CoinsDigits, CoinsPlaces, playerCoins);
        SetDigits(TimerDigits, TimerPlaces, playerTime);
        SetDigits(new List<Image>() { LoadingLives }, 1, playerLives);
        LoadingBackground.gameObject.SetActive(false);
        LoadingLives.gameObject.SetActive(false);
        LoadScene();
        coinAnim = GameObject.Find("Coin Sprite").GetComponent<Animator>();
        coinAnim.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (!loaded && loadTime > 0)
        {
            loadTime -= Time.deltaTime;
        }
        else if (!loaded && loadTime <= 0)
        {
            SceneManager.LoadScene("Main Scene");
            loaded = true;
            LoadingBackground.gameObject.SetActive(false);
            LoadingLives.gameObject.SetActive(false);
            coinAnim.enabled = true;
        }
        else
        {
            KeepTime();
        }
    }

    void SetDigits(List<Image> digits, int numberPlaces, int result)
    {
        int placeCounter = numberPlaces - 1;
        IEnumerator<Image> digitsEnum = digits.GetEnumerator();
        digitsEnum.MoveNext();
        while (placeCounter >= 0) {
            int digit = result / (int) (Mathf.Pow(10, placeCounter));
            result = result % (int)(Mathf.Pow(10, placeCounter));
            if (digit >= 10) {
                Debug.Log("going in here at least");
                int maxNum = 0;
                while (placeCounter >= 0) {
                    maxNum += (9 * (int) Mathf.Pow(10, placeCounter));
                    placeCounter -= 1;
                    Debug.Log(maxNum);
                }
                SetDigits(digits, numberPlaces, maxNum);
                return;
            }
            Debug.Log(digit);
            Debug.Log(digitsEnum.Current.name);
            digitsEnum.Current.sprite = numberSprites[digit];
            digitsEnum.MoveNext();
            placeCounter -= 1;
        }
    }

    public void LoadScene()
    {
        loaded = false;
        LoadingBackground.gameObject.SetActive(true);
        LoadingLives.gameObject.SetActive(true);
        loadTime = 1.5f;
    }

    public void ReloadScene()
    {
        playerTime = 330;
        SetDigits(TimerDigits, TimerPlaces, playerTime);
        LoadScene();
    }

    public void UpdateScore(int scoreToAdd) {
        playerScore += scoreToAdd;
        SetDigits(ScoreDigits, ScorePlaces, playerScore);
    }

    void KeepTime() {
        timeIncrement -= Time.deltaTime;
        if (timeIncrement <= 0) {
            playerTime -= 1;
            SetDigits(TimerDigits, TimerPlaces, playerTime);
            timeIncrement = 1;
        }
    }

    public void TakeLife() {
        playerLives -= 1;
        SetDigits(new List<Image>() { LoadingLives }, 1, playerLives);
        if (playerLives == 0) {
            //Do game over scene and back to Menu Scene?
            SceneManager.LoadScene("Menu Scene");
            Destroy(this.gameObject);
        }
        ReloadScene();
    }

    public void AddCoin() {
        playerCoins += 1;
        SetDigits(CoinsDigits, CoinsPlaces, playerCoins);
    }
}
