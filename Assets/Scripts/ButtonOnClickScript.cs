using UnityEngine;
using UnityEngine.UI;

public class ButtonOnClickScript : MonoBehaviour
{
    private Button button;

    void Start()
    {
        // Get the Button component attached to this GameObject
        button = GetComponent<Button>();

        // Check if the button is found and set up the click listener
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found on the GameObject.");
        }
    }

    void OnButtonClick()
    {
        // Find the GameObject with the "GameManager" tag
        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");

        if (gameManagerObject != null)
        {
            // Get the SceneLoader component attached to the GameManager object
            SceneLoader sceneLoader = gameManagerObject.GetComponent<SceneLoader>();

            if (sceneLoader != null)
            {
                // Call LoadLevel to load the "Dialogue 0" scene
                sceneLoader.LoadLevel("Dialogue 0");
            }
            else
            {
                Debug.LogError("SceneLoader component not found on GameManager.");
            }
        }
        else
        {
            Debug.LogError("GameManager object with the specified tag was not found.");
        }
    }
}
