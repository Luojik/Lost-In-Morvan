using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotDestroyOnLoadScene : MonoBehaviour
{
    public GameObject[] objects;

    void Awake()
    {
        // Ne pas détruire les objets lors du chargement de nouvelles scènes
        foreach (var element in objects)
        {
            DontDestroyOnLoad(element);
        }
    }

    void Update()
    {
        // Vérifiez si la scène active est la scène 0
        /* if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            foreach (var element in objects)
            {
                // Détruire les objets si on revient à la scène 0
                Destroy(element);
            }
        } */
    }
}

