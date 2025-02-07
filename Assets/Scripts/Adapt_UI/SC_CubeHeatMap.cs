using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SC_CubeHeatMap : MonoBehaviour
{

    public int id;
    public bool isOccluded;
    public bool isOccupied;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bound")
        {
            isOccluded = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Bound")
        {
            isOccluded = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Bound")
        {
            isOccluded = false;
        }
    }


}
