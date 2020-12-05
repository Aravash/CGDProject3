using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum ObjectTypes
{
    REGULAR,
    TRASH,
    ENEMY,
}

public class ObjectCreator : MonoBehaviour
{
    private GameObject[] models;
    private BoxCollider col;

    void Start()
    {
        models = Resources.LoadAll<GameObject>("objects");
        col = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        //Test code
        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("K pressed");
            BuildObject(ObjectTypes.REGULAR, false);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("L pressed");
            BuildObject(ObjectTypes.REGULAR, true);
        }
    }

    public void BuildObject(ObjectTypes type, bool wrapped)
    {
        Vector3 pos = new Vector3(Random.Range(col.bounds.min.x, col.bounds.max.x),
                                  transform.position.y,
                                  Random.Range(col.bounds.min.z, col.bounds.max.z));

        GameObject newObj = gameObject;

        switch (type)
        {
            case (ObjectTypes.REGULAR):
                {
                    newObj = Instantiate(models[Random.Range(0, models.Length)], pos, transform.rotation);
                    break;
                }
            case (ObjectTypes.TRASH):
                {
                    //Currently same as regular
                    newObj = Instantiate(models[Random.Range(0, models.Length)], pos, transform.rotation);
                    break;
                }
            case (ObjectTypes.ENEMY):
                {
                    //Currently same as regular
                    newObj = Instantiate(models[Random.Range(0, models.Length)], pos, transform.rotation);
                    break;
                }
        }

        if (wrapped)
        {
            newObj.AddComponent<WrappingHandler>();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, Vector3.one);
    }
}
