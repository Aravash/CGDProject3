using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chute : MonoBehaviour
{
    public enum col_ids
    {
        RED,
        BLUE,
        CYAN,
        YELLOW,
        LIME,
        PURPLE,
    }
    [SerializeField] col_ids desired_type = col_ids.RED;
    Color my_color;
    [SerializeField] private float GoodGain = 2f;
    [SerializeField] private float WrongColourLoss = 3f;
    [SerializeField] private float BadLoss = 5;
    
    [SerializeField] bool incinerator = false;


    // Start is called before the first frame update
    void Start()
    {
        my_color = getColour(desired_type);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject, 2);

        // No points gained or lost from incinerating an object
        if (incinerator)
        {
            Debug.Log("Incinerated!");
            return;
        }

        // classify object
        if (other.tag == "GOOD")
        {
            if (other.GetComponent<Renderer>().material.color == my_color &&
                !other.GetComponent<WrappingHandler>())
            {
                Debug.Log("CORRECT! GAIN POINTS!!"); // gain 2
                GameManager.ChangeScore(GoodGain);
                return;
            }
            Debug.Log("WRONG!!! LOSE POINTS!!!"); // lose 3
            GameManager.ChangeScore(-WrongColourLoss);
        }
        else if (other.tag == "BAD")
        {
            Debug.Log("WRONG!!! LOSE MANY POINTS!!!!!"); // lose 5
            GameManager.ChangeScore(-BadLoss);
        }
    }

    public static Color getColour(col_ids col)
    {
        switch (col)
        {
            default:
                return Color.white;
            case col_ids.RED:
                return Color.red;
            case col_ids.BLUE:
                return Color.blue;
            case col_ids.CYAN:
                return Color.cyan;
            case col_ids.YELLOW:
                return new Color(1, 1, 0);
            case col_ids.LIME:
                return Color.green;
            case col_ids.PURPLE:
                return new Color(0.5f, 0, 1);
        }
    }
}
