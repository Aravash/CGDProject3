using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    public bool flop;
    private bool flip;


    private enum TERRAIN_TYPE { UNKNOWN = -1, GRASS, GRAVEL, WOOD_FLOOR, WATER };

    [SerializeField]
    private TERRAIN_TYPE currentTerrain;

    [FMODUnity.EventRef]
    public string FMODFootstepEvent = null;

    private FMOD.Studio.EventInstance footsteps;

    private void Update()
    {
        DetermineTerrain();

        if(flip != flop) { SelectAndPlayFootstep(); flip = flop; }
    }

    private void DetermineTerrain()
    {
        RaycastHit[] hit;

        hit = Physics.RaycastAll(transform.position, Vector3.down, 10.0f);

        foreach (RaycastHit rayhit in hit)
        {
            if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Gravel"))
            {
                currentTerrain = TERRAIN_TYPE.GRAVEL;
                break;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Wood"))
            {
                currentTerrain = TERRAIN_TYPE.WOOD_FLOOR;
                break;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Grass"))
            {
                currentTerrain = TERRAIN_TYPE.GRASS;
            }
            else if (rayhit.transform.gameObject.layer == LayerMask.NameToLayer("Water"))
            {
                currentTerrain = TERRAIN_TYPE.WATER;
            }
        }
    }

    public void SelectAndPlayFootstep()
    {
        PlayFootstep((int)currentTerrain);
    }

    private void PlayFootstep(int terrain)
    {
        footsteps = FMODUnity.RuntimeManager.CreateInstance(FMODFootstepEvent);
        footsteps.setParameterByName("Terrain", terrain);
        footsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        footsteps.start();
        footsteps.release();
    }
}