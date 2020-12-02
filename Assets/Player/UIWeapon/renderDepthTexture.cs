using UnityEngine;

/// <summary>
/// Assures, that the camera renders the depth texture
/// </summary>
[ExecuteInEditMode]
public class renderDepthTexture : MonoBehaviour
{
    void OnEnable()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }
}