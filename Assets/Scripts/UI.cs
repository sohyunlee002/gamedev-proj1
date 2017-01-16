using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
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
    string pathToSprites = "NES - Super Mario Bros - Font(Transparent)";
    public Object assetTest;
    Sprite[] numberSprites;
    int exampleScore = 100;
    bool loaded = true;
    float loadTime = 0;

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
        SetDigits(ScoreDigits, ScorePlaces, 0);
        SetDigits(CoinsDigits, CoinsPlaces, 0);
        SetDigits(TimerDigits, TimerPlaces, 0);
        SetDigits(new List<Image>() { LoadingLives }, 1, 3);
        LoadingBackground.gameObject.SetActive(false);
        LoadingLives.gameObject.SetActive(false);
        LoadScene();
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
}
