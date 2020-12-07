using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAudio : MonoBehaviour
{
    FMOD.Studio.EventInstance fmod_hum;

    [FMODUnity.EventRef]
    public string FMODScoreUp = null;
    [FMODUnity.EventRef]
    public string FMODScoreDown = null;
    [FMODUnity.EventRef]
    public string FMODWin = null;
    [FMODUnity.EventRef]
    public string FMODLose = null;

    private Vector3 pos = new Vector3(0, 1, 0);

    //FMOD.Studio.PARAMETER_ID fmod_state_identifier;

    // Start is called before the first frame update
    void Start()
    {
        //fmod_hum = FMODUnity.RuntimeManager.CreateInstance(fmodHum);
        //FMODUnity.RuntimeManager.AttachInstanceToGameObject(fmod_hum, GetComponent<Transform>(), GetComponent<Rigidbody>());

        //// Attach the State between FMOD and this script
        //FMOD.Studio.EventDescription state_description;
        //fmod_hum.getDescription(out state_description);
        //FMOD.Studio.PARAMETER_DESCRIPTION state_parameter_description;
        //state_description.getParameterDescriptionByName("State", out state_parameter_description);
        //fmod_state_identifier = state_parameter_description.id;


        //fmod_hum.start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void playScoreUp()
    {
        playScoreUpAudio();
    }

    public void playScoreDown()
    {
        playScoreDownAudio();
    }

    public void playWin()
    {
        playWinAudio();
    }

    public void playLost()
    {
        playLoseAudio();
    }

    private void playWinAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODWin, pos);
    }

    private void playLoseAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODLose, pos);
    }

    private void playScoreUpAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODScoreUp, pos);
    }

    private void playScoreDownAudio()
    {
        FMODUnity.RuntimeManager.PlayOneShot(FMODScoreDown, pos);
    }

}
