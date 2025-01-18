using UnityEngine;

public class DoNotDestroyOnLoadScene : MonoBehaviour
{
    public GameObject[] objects; // Liste des objets � v�rifier

    private static bool isInitialized = false; // Flag pour v�rifier si l'initialisation a d�j� eu lieu

    void Awake()
    {
        // Si l'initialisation a d�j� eu lieu, on ne fait rien
        if (isInitialized)
        {
            Destroy(gameObject); // D�truire ce GameObject si une instance existe d�j�
            return;
        }

        isInitialized = true; // Marquer l'initialisation comme effectu�e
        DontDestroyOnLoad(gameObject); // Ne pas d�truire cet objet au changement de sc�ne

        foreach (var element in objects)
        {
            // V�rifie s'il existe d�j� une instance de cet objet
            if (IsAlreadyInstantiated(element))
            {
                Destroy(element); // D�truire les doublons si n�cessaire
            }
            else
            {
                DontDestroyOnLoad(element); // Ajouter si pas encore persistant
            }
        }
    }

    private bool IsAlreadyInstantiated(GameObject obj)
    {
        // Cherche tous les objets de type GameObject dans la sc�ne
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (var other in allObjects)
        {
            // V�rifie si un objet du m�me nom est d�j� pr�sent dans les objets persistants
            if (other != obj && other.name == obj.name && other.hideFlags == HideFlags.DontSave)
            {
                // Si une instance du m�me nom existe d�j�, on la consid�re comme une doublon
                return true;
            }
        }

        return false; // Sinon, l'objet n'est pas encore instanci�
    }
}
