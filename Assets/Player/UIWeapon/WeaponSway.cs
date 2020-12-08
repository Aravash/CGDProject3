using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{

    [SerializeField]public float strength = 0.5f;
    [SerializeField]public float smoothStrength = 1f;
    [SerializeField]private float maxAmount = 1f;

    public Vector3 initialPosition;

    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    public void gunUpdate(Vector3 finalPosition)
    {
        finalPosition.x = Mathf.Clamp(finalPosition.x, -maxAmount, maxAmount);
        finalPosition.y = Mathf.Clamp(finalPosition.y, -maxAmount, maxAmount);
        
        transform.localPosition = Vector3.Lerp(
            transform.localPosition, (finalPosition + initialPosition) * strength, smoothStrength * Time.deltaTime);
    }
}
