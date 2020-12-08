using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.VFX;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;

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

    [SerializeField] GameObject vfx;


    // Start is called before the first frame update
    void Start()
    {
        my_color = getColour(desired_type);

        Scene currentScene = SceneManager.GetActiveScene();
        if (currentScene.name == "GameScene" || currentScene.name == "GameScene_ai")
        {
            vfx = GameObject.FindGameObjectWithTag(desired_type.ToString());
            vfx.GetComponent<VisualEffect>().Stop();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject, 2);

        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "GameScene" || currentScene.name == "GameScene_ai")

        {

            GameManager._i.counterDec();



            // No points gained or lost from incinerating an object

            if (incinerator)

            {

                Debug.Log("Incinerated!");

                if (other.tag == "GOOD")

                {

                    GameManager._i.mistakeGood();

                }

                else if (other.GetComponent<Enemy>())

                {

                    GameManager._i.enemyBurnt();

                }

                return;

            }



            // classify object

            if (other.tag == "GOOD")

            {

                if (other.GetComponent<Renderer>().material.color == my_color)

                {

                    GameManager.ChangeScore(GoodGain);
                    vfx.GetComponent<VisualEffect>().Play();
                    GameAudioManager.PlayFireworks(this.transform.position);
                    return;

                }

                GameManager.ChangeScore(-WrongColourLoss);

                GameManager._i.mistakeColour();

            }

            else if (other.tag == "BAD")

            {

                GameManager.ChangeScore(-BadLoss);



                if (other.GetComponent<Enemy>())

                {

                    GameManager._i.enemyMissed();

                }

                else

                {

                    GameManager._i.mistakeTrash();

                }

            }
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
