using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingSceneController : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(CargarGameplay());
    }

    IEnumerator CargarGameplay()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("Gameplay");

        while (!load.isDone)
        {
            yield return null;
        }
    }
}
