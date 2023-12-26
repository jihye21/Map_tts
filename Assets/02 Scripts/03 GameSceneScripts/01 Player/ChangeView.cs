using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeView : MonoBehaviour
{
    public Camera firstPersonCamera;
    public Camera overheadCamera;

    void Start()
    {
        ShowOverheadView();
    }

    public void ShowOverheadView()
    {
        firstPersonCamera.enabled = false;
        overheadCamera.enabled = true;
    }

    public void ShowFirstPersonView()
    {
        firstPersonCamera.enabled = true;
        overheadCamera.enabled = false;
    }

}


    

