using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagScript : MonoBehaviour
{
    public UIUpdater uIUpdater;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {
            uIUpdater.Finish();
        }
    }
}
