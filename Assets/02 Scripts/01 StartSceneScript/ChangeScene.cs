using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    // Start is called before the first frame update
    public void GameScene()
    {
        SceneManager.LoadScene("small_map");
    }

    public void OptionScene()
    {
        SceneManager.LoadScene("OptionScene");
    }

    public void StartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void QuitScene()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
