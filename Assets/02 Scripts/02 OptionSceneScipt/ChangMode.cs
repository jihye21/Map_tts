using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangMode : MonoBehaviour
{
    FullScreenMode screenMode;
    public Toggle fullscreenBtn;

    void initUI()
    {
        fullscreenBtn.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }
    public void ChangeChangMode(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }
}
