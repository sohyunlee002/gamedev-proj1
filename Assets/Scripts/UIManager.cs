using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class UIManager : MonoBehaviour {

    //Singleton UIManager.
    public static UIManager uiManager;

    GameObject LoadingBackground;
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
    string pathToSprites = "NES - Super Mario Bros - Font(Transparent)";
    public Object assetTest;
    Sprite[] numberSprites;
    float loadTime = 1.5f;
    int playerLives = 3;
    int playerScore = 0;
    int playerTime = 330;
    int playerCoins = 0;

    // Use this for initialization
    void Start()
    {
        if (uiManager == null)
        {
            uiManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        LoadingBackground = GameObject.Find("Loading_Background");
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
        LoadingBackground.SetActive(false);
    }

    /* Use coroutine for spending time on Loading screen. */
    IEnumerator ShowLoadingScreen() {
        LoadingBackground.SetActive(true);
        yield return new WaitForSeconds(loadTime);
        LoadingBackground.SetActive(false);
        SceneManager.LoadScene("Main Scene");
        StartCoroutine("KeepTime");
        yield break;
    }

    // Update is called once per frame
    void Update () {}

    void SetDigits(List<Image> digits, int numberPlaces, int result)
    {
        int placeCounter = numberPlaces - 1;
        IEnumerator<Image> digitsEnum = digits.GetEnumerator();
        digitsEnum.MoveNext();
        while (placeCounter >= 0) {
            int digit = result / (int) (Mathf.Pow(10, placeCounter));
            result = result % (int)(Mathf.Pow(10, placeCounter));
            if (digit >= 10) {
                int maxNum = 0;
                while (placeCounter >= 0) {
                    maxNum += (9 * (int) Mathf.Pow(10, placeCounter));
                    placeCounter -= 1;
                }
                SetDigits(digits, numberPlaces, maxNum);
                return;
            }
            digitsEnum.Current.sprite = numberSprites[digit];
            digitsEnum.MoveNext();
            placeCounter -= 1;
        }
    }

    public void LoadScene()
    {
        playerTime = 330;
        SetDigits(TimerDigits, TimerPlaces, playerTime);
        StopCoroutine("KeepTime");
        StartCoroutine("ShowLoadingScreen");
    }

    public void UpdateScore(int scoreToAdd) {
        playerScore += scoreToAdd;
        SetDigits(ScoreDigits, ScorePlaces, playerScore);
    }

    IEnumerator KeepTime() {
        while (playerTime > 0)
        {
            yield return new WaitForSeconds(1);
            playerTime -= 1;
            SetDigits(TimerDigits, TimerPlaces, playerTime);
        }
        TakeLife();
        LoadScene();
        yield break;
    }

    public void TakeLife() {
        playerLives -= 1;
        SetDigits(new List<Image>() { LoadingLives }, 1, playerLives);
        if (playerLives == 0)
        {
            //Do game over scene and back to Menu Scene?
            SceneManager.LoadScene("Menu Scene");
            playerLives = 3;
        }
        else
        {
            LoadScene();
        }
    }

    public void AddCoin() {
        playerCoins += 1;
        SetDigits(CoinsDigits, CoinsPlaces, playerCoins);
    }
}
