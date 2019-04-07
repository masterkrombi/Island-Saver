using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {

    public Slider loadingBar;

    AsyncOperation loadScene;
    bool loadTheScene = true;

    void Start()
    {
        loadingBar.value = loadingBar.minValue;
    }

    void Update()
    {
        if (loadTheScene)
        {
            loadScene = SceneManager.LoadSceneAsync("EndlessRunnerMode");
            loadScene.allowSceneActivation = false;
            loadTheScene = false;
        }

        float progress = (loadScene.progress * 100) / 0.9f;
        loadingBar.value = progress;

        if (progress >= 100f)
        {
            loadScene.allowSceneActivation = true;
        }
    }

}
