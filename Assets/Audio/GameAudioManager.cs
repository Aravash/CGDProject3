using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAudioManager : MonoBehaviour
{
    #region Singleton
    private static GameAudioManager _instance;

    public static GameAudioManager Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion


    #region FMOD Values
    [SerializeField]
    [FMODUnity.EventRef]
    public string FMODScoreUp = null;
    [SerializeField]
    [FMODUnity.EventRef]
    public string FMODScoreDown = null;
    [SerializeField]
    [FMODUnity.EventRef]
    public string FMODWin = null;
    [SerializeField]
    [FMODUnity.EventRef]
    public string FMODLose = null;
    [SerializeField]
    [FMODUnity.EventRef]
    public string FMODIncinerate = null;
    [SerializeField]
    [FMODUnity.EventRef]
    public string FMODEnemyFootstep = null;
    [SerializeField]
    [FMODUnity.EventRef]
    public string FMODWrappingDestroy = null;
    [SerializeField]
    [FMODUnity.EventRef]
    public string FMODFireworks = null;


    #endregion
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private static Vector3 getPlayerPos()
    {
        var array = GameObject.FindGameObjectsWithTag("Player");
        if (array[0]) return array[0].transform.position;
        return Vector3.zero;
    }

    public static void PlayFireworks(Vector3 pos)
    {
        FMODUnity.RuntimeManager.PlayOneShot(_instance.FMODFireworks, pos);
    }

    public static void WrappingDestroy(Vector3 pos)
    {
        FMODUnity.RuntimeManager.PlayOneShot(_instance.FMODWrappingDestroy, pos);
    }

    public static void EnemyFootstep(Vector3 pos)
    {
        FMODUnity.RuntimeManager.PlayOneShot(_instance.FMODEnemyFootstep, pos);
    }
    public static void Incinerate(Vector3 pos)
    {
        FMODUnity.RuntimeManager.PlayOneShot(_instance.FMODIncinerate, pos);
    }

    public static void GainScore()
    {
        FMODUnity.RuntimeManager.PlayOneShot(_instance.FMODScoreUp, getPlayerPos());
    }
    public static void LoseScore()
    {
        FMODUnity.RuntimeManager.PlayOneShot(_instance.FMODScoreDown, getPlayerPos());
    }

}
