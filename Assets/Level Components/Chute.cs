using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chute : MonoBehaviour
{
    enum col_ids
    {
        WHITE,
        GREEN,
        RED,
        BLUE,
        CYAN,
        YELLOW,
        LIME,
        ORANGE,
        PURPLE,
        BLACK
    }
    [SerializeField] col_ids desired_type = col_ids.WHITE;
    Color my_color;
    [SerializeField] private float GoodGain = 2f;
    [SerializeField] private float WrongColourLoss = 3f;
    [SerializeField] private float BadLoss = 5;
    
    [SerializeField] bool incinerator = false;


    // Start is called before the first frame update
    void Start()
    {
        switch (desired_type)
        {
            default:
                my_color = Color.white;
                break;
            case col_ids.GREEN:
                my_color = Color.green * 0.5f;
                break;
            case col_ids.RED:
                my_color = Color.red;
                break;
            case col_ids.BLUE:
                my_color = Color.blue;
                break;
            case col_ids.CYAN:
                my_color = Color.cyan;
                break;
            case col_ids.YELLOW:
                my_color = new Color(1, 1, 0, 1);
                break;
            case col_ids.LIME:
                my_color = Color.green;
                break;
            case col_ids.ORANGE:
                my_color = new Color(1, 0.5f, 0);
                break;
            case col_ids.PURPLE:
                my_color = new Color(0.5f, 0, 1);
                break;
            case col_ids.BLACK:
                my_color = Color.black;
                break;
        }
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
            if (other.GetComponent<Renderer>().material.color == my_color)
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
}
