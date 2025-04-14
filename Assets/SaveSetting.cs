using UnityEngine;

public class SaveSetting : MonoBehaviour
{
    void Start()
    {
        if(PlayerPrefs.GetString("SkipTutorial") == "true")
        {
            GameSceneManager.Instance.ChangeScene("VFXTest");
            PlayerPrefs.SetString("SkipTutorial", "true");
            PlayerPrefs.Save();
        }
        else
        {
            PlayerPrefs.SetString("SkipTutorial", "false");
            PlayerPrefs.Save();
        }
    }

    public void Save()
    {
        PlayerPrefs.SetString("SkipTutorial", "true");
        PlayerPrefs.Save();
    }
}
