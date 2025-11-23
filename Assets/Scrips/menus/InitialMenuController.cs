using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LoadingScene");
    }

    public void OpenAchievements()
    {
        SceneManager.LoadScene("AchievementsScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
