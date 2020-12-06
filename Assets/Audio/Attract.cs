using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attract : MonoBehaviour
{
	
    FMOD.Studio.EventInstance fmod_hum;


    [FMODUnity.EventRef]
    public string fmodHum = null;
    [FMODUnity.EventRef]
    public string fmodFire = null;

    FMOD.Studio.PARAMETER_ID fmod_state_identifier;


    public bool activated = false;
    public bool press = false;

enum State
{
Deactivated = 0,
Activating = 1,
Running = 2,
Stopping = 3
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
	// press = Input.GetKeyDown("space");

	// If I want to change State
	if(press != activated)
	{
	  // State will change 'activated' state
	  if(activated)
	  {
	    Deactivate();

	    FMODUnity.RuntimeManager.PlayOneShot(fmodFire, transform.position);
	  }
	  else
	  {
	    Activate();	    
	  }
	}
    }

    void Activate()
    {
	activated = true;
	CurrentState = State.Activating;
	fmod_hum.setParameterByID(fmod_state_identifier, (float)CurrentState);
    }

    void Deactivate()
    {
	activated = false;
	CurrentState = State.Deactivated;
	fmod_hum.setParameterByID(fmod_state_identifier, (float)CurrentState);
    }
}
