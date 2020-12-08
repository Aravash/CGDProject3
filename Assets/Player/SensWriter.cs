using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class SensWriter : MonoBehaviour
{
   [SerializeField] private float sens = 30;
   [SerializeField] private Text txt;
   
   public static SensWriter instance;

    void Awake () {
        DontDestroyOnLoad (transform.gameObject);
        instance = this;
        txt.text = sens.ToString();
    }
    
    public static float GetSens()
    {
        return instance.sens;
    }
    
    public static void SetSens(System.Single vol)
    {
        instance.sens = vol;
        instance.txt.text = vol.ToString();
    }
}
