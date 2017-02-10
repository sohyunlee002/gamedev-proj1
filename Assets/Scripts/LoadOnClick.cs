using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadOnClick : MonoBehaviour {

    GameObject UI_Canvas;
    GameObject Title_Screen;

    public void Start() {
        SceneManager.UnloadSceneAsync("Main Scene");
        UI_Canvas = GameObject.Find("UI_Canvas");
        Title_Screen = GameObject.Find("Title Screen");
        DontDestroyOnLoad(UI_Canvas);
        //UI_Canvas.SetActive(false);
    }

    public void LoadScene()
    {
        Debug.Log(UIManager.uiManager);
        UIManager.uiManager.LoadScene();
        Title_Screen.SetActive(false);
        //Wait a few seconds.
        //SceneManager.LoadScene("Main Scene");
    }

}
