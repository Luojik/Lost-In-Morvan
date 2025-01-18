using UnityEngine;

public class DoNotDestroyOnLoadScene : MonoBehaviour
{
    public GameObject[] objects; // Liste des objets à vérifier

    private static bool isInitialized = false; // Flag pour vérifier si l'initialisation a déjà eu lieu

    void Awake()
    {
        // Si l'initialisation a déjà eu lieu, on ne fait rien
        if (isInitialized)
        {
            Destroy(gameObject); // Détruire ce GameObject si une instance existe déjà
            return;
        }

        isInitialized = true; // Marquer l'initialisation comme effectuée
        DontDestroyOnLoad(gameObject); // Ne pas détruire cet objet au changement de scène

        foreach (var element in objects)
        {
            // Vérifie s'il existe déjà une instance de cet objet
            if (IsAlreadyInstantiated(element))
            {
                Destroy(element); // Détruire les doublons si nécessaire
            }
            else
            {
                DontDestroyOnLoad(element); // Ajouter si pas encore persistant
            }
        }
    }

    private bool IsAlreadyInstantiated(GameObject obj)
    {
        // Cherche tous les objets de type GameObject dans la scène
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        foreach (var other in allObjects)
        {
            // Vérifie si un objet du même nom est déjà présent dans les objets persistants
            if (other != obj && other.name == obj.name && other.hideFlags == HideFlags.DontSave)
            {
                // Si une instance du même nom existe déjà, on la considère comme une doublon
                return true;
            }
        }

        return false; // Sinon, l'objet n'est pas encore instancié
    }
}
