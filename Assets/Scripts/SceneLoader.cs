using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] public GameObject loadingScreen;
    [SerializeField] public Slider loadingSlider;

    public void LoadLevel(string levelToLoad)
    {   
        loadingScreen.SetActive(true);

        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        Debug.Log("Starting to load scene: " + levelToLoad);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone)
        {
            float progressValue = Mathf.Clamp01(loadOperation.progress / 0.9f);
            loadingSlider.value = progressValue;

            Debug.Log("Loading progress: " + (progressValue * 100) + "%");

            yield return null;
        }

        loadingScreen.SetActive(false);

        Debug.Log("Loading complete!");
    }
}
