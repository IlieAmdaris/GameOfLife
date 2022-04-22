using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadDialog : MonoBehaviour
{
    public Dropdown patternName;
    public HUD hud;
    void Start()
    {
        ReloadOptions();
    }
    private void OnEnable()
    {
        ReloadOptions();
    }
    public void QuitDialog()
    {
        hud.isActive = false;
        gameObject.SetActive(false);
    }
    public void LoadPattern()
    {
        EventManager.TriggerEvent("LoadPattern");
        hud.isActive = false;
        gameObject.SetActive(false);
    }
    void ReloadOptions()
    {
        List<string> options = new List<string>();
        string[] filePaths = Directory.GetFiles(@"patterns/");
        for(int i = 0; i < filePaths.Length; i++)
        {
            string fileName = filePaths[i].Substring(filePaths[i].LastIndexOf('/') + 1);
            string extension = System.IO.Path.GetExtension(fileName);
            fileName = fileName.Substring(0,fileName.Length - extension.Length);
            options.Add(fileName);
        }
        patternName.ClearOptions();
        patternName.AddOptions(options);
    }

}
