using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardHutSpawner : MonoBehaviour
{
    [SerializeField]
    Transform building;

    [SerializeField]
    Transform doorA;

    [SerializeField]
    Transform doorB;

    [SerializeField]
    Transform doorC;

    [SerializeField]
    Transform doorD;

    [SerializeField]
    LayerMask triggerLayers;

    Vector3 doorAStart;
    Vector3 doorBStart;
    Vector3 doorCStart;
    Vector3 doorDStart;

    Vector3 doorAOpen;
    Vector3 doorBOpen;
    Vector3 doorCOpen;
    Vector3 doorDOpen;

    Vector3 doorATarget;
    Vector3 doorBTarget;
    Vector3 doorCTarget;
    Vector3 doorDTarget;

    int objectsInTrigger = 0;

    Vector3 checkPosition = new Vector3(0, 1f, -0.085f);
    Vector3 checkHalfs = new Vector3(1.635f, 0.75f, 1.62f);

    void Awake()
    {
        doorAStart = doorA.position;
        doorBStart = doorB.position;
        doorCStart = doorC.position;
        doorDStart = doorD.position;

        doorATarget = doorAStart;
        doorBTarget = doorBStart;
        doorCTarget = doorCStart;
        doorDTarget = doorDStart;

        doorAOpen = doorA.position - (doorA.right * 0.55f);
        doorBOpen = doorB.position - (doorA.right * 1.106669f);
        doorCOpen = doorC.position + (doorA.right * 1.106669f);
        doorDOpen = doorD.position + (doorD.right * 0.55f);
    }

    private void Update()
    {
        doorA.position = Vector3.Lerp(doorA.position, doorATarget, Time.deltaTime * 5);
        doorB.position = Vector3.Lerp(doorB.position, doorBTarget, Time.deltaTime * 5);
        doorC.position = Vector3.Lerp(doorC.position, doorCTarget, Time.deltaTime * 5);
        doorD.position = Vector3.Lerp(doorD.position, doorDTarget, Time.deltaTime * 5);

        if(Physics.CheckBox(transform.TransformPoint(checkPosition), checkHalfs, transform.rotation, triggerLayers))
        {
            doorATarget = doorAOpen;
            doorBTarget = doorBOpen;
            doorCTarget = doorCOpen;
            doorDTarget = doorDOpen;
        }
        else
        {
            doorATarget = doorAStart;
            doorBTarget = doorBStart;
            doorCTarget = doorCStart;
            doorDTarget = doorDStart;
        }
    }
}
