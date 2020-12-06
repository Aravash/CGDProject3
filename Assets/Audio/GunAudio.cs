using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAudio : MonoBehaviour
{
    FMOD.Studio.EventInstance fmod_hum;

    [FMODUnity.EventRef]
    public string fmodHum = null;
    [FMODUnity.EventRef]
    public string fmodFire = null;

    FMOD.Studio.PARAMETER_ID fmod_state_identifier;

    enum State
    {
        Deactivated = 0,
        Activating = 1,
        Running = 2
    }
    [SerializeField] private State CurrentState = State.Deactivated;

    // Start is called before the first frame update
    void Start()
    {
        fmod_hum = FMODUnity.RuntimeManager.CreateInstance(fmodHum);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(fmod_hum, GetComponent<Transform>(), GetComponent<Rigidbody>());

        // Attach the State between FMOD and this script
        FMOD.Studio.EventDescription state_description;
        fmod_hum.getDescription(out state_description);
        FMOD.Studio.PARAMETER_DESCRIPTION state_parameter_description;
        state_description.getParameterDescriptionByName("State", out state_parameter_description);
        fmod_state_identifier = state_parameter_description.id;


        fmod_hum.start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void playGrab()
    {
        if (CurrentState == State.Deactivated) { Activate(); };
    }

    public void playDrop()
    {
        if (CurrentState != State.Deactivated) { Deactivate(); };
    }

    public void playFire()
    {
        if (CurrentState != State.Deactivated) { Fire(); };
    }

    private void Activate()
    {
        CurrentState = State.Activating;
        fmod_hum.setParameterByID(fmod_state_identifier, (float)CurrentState);
    }

    private void Deactivate()
    {
        CurrentState = State.Deactivated;
        fmod_hum.setParameterByID(fmod_state_identifier, (float)CurrentState);
    }

    private void Fire()
    {
        FMODUnity.RuntimeManager.PlayOneShot(fmodFire, transform.position);
    }
}
