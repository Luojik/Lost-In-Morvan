using UnityEngine;
using UnityEngine.UI;
using System.Reflection;

public class ButtonHandler : MonoBehaviour
{
    private PlayerStats playerStatsInstance;

    public string functionName; // Assign the function name in the inspector or dynamically

    void Start()
    {
        // Find the Player GameObject and get the PlayerStats component
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            playerStatsInstance = playerObject.GetComponent<PlayerStats>();

            if (playerStatsInstance == null)
            {
                Debug.LogError("PlayerStats component not found on the GameObject with the tag 'Player'.");
            }
        }
        else
        {
            Debug.LogError("No GameObject with the tag 'Player' found.");
        }

        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("Button component not found on the GameObject!");
        }
    }

    void OnButtonClick()
    {
        if (playerStatsInstance != null)
        {
            if (!string.IsNullOrEmpty(functionName))
            {
                // Use reflection to invoke the method by name on PlayerStats
                MethodInfo method = playerStatsInstance.GetType().GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                if (method != null)
                {
                    method.Invoke(playerStatsInstance, null); // Invoke the method with no parameters
                }
                else
                {
                    Debug.LogError($"Method '{functionName}' not found on the PlayerStats instance.");
                }
            }
            else
            {
                Debug.LogError("Function name is not assigned!");
            }
        }
        else
        {
            Debug.LogError("PlayerStats instance is not assigned or found!");
        }
    }
}

