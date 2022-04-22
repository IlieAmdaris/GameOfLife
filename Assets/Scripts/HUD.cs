using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    public SaveDialog saveDialog;
    public LoadDialog loadDialog;
    public bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {
        saveDialog.gameObject.SetActive(false);
        loadDialog.gameObject.SetActive(false);
        
    }
    public void showLoadDialog()
    {
        loadDialog.gameObject.SetActive(true);
        isActive = true;
    }
    public void ShowSaveDialog()
    {
        saveDialog.gameObject.SetActive(true);
        isActive = true;
    }
}
