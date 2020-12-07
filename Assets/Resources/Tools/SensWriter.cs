using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SensWriter : MonoBehaviour
{
    public void writeSens(System.Single vol)
    {
        StreamWriter writer = new StreamWriter("Assets/Resources/Tools/sensitivity.txt", true);
        writer.Write(vol);
        Debug.Log("wrote "+ vol);
        writer.Close();
    }
}
