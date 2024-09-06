using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private string fadeInTriggerName = "FadeIn";

    private Animator fadeAnimator;

    [SerializeField] public float fadeInDuration = 1f;

    private void Start()
    {
        GameObject fadeObject = GameObject.FindGameObjectWithTag("FadeImage");

        if (fadeObject != null)
        {
            fadeImage = fadeObject.GetComponent<Image>();
            if (fadeImage == null)
            {
                Debug.LogError("No Image component found on the GameObject with the specified tag.");
            }
        }
        else
        {
            Debug.LogError("No GameObject found with the specified tag.");
        }

        if (fadeImage != null)
        {
            fadeAnimator = fadeImage.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("No Image component assigned.");
        }
    }

    public void LoadLevel(string levelToLoad)
    {   
        StartCoroutine(LoadLevelAsync(levelToLoad));
    }

    IEnumerator LoadLevelAsync(string levelToLoad)
    {
        fadeAnimator.SetTrigger(fadeInTriggerName);
        Debug.Log("Starting to load scene: " + levelToLoad);

        yield return new WaitForSeconds(fadeInDuration);

        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        Debug.Log("Loading complete!");
    }


}
