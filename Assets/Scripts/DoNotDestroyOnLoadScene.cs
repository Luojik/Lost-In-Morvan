using UnityEngine;
using UnityEngine.SceneManagement;

public class DoNotDestroyOnLoadScene : MonoBehaviour
{
    public GameObject[] objects;

    void Awake()
    {
        // Ne pas d�truire les objets lors du chargement de nouvelles sc�nes
        foreach (var element in objects)
        {
            DontDestroyOnLoad(element);
        }
    }

    void Update()
    {
        // V�rifiez si la sc�ne active est la sc�ne 0
        /* if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            foreach (var element in objects)
            {
                // D�truire les objets si on revient � la sc�ne 0
                Destroy(element);
            }
        } */
    }
}

