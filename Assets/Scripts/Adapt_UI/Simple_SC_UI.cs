using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simple_SC_UI : MonoBehaviour
{
    public List<GameObject> heatMapCubes;
    public Canvas UI;
    public Camera camera;
    public GameObject bestVoxel;

    public Global_Controller controller;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    void ui_Calc()
    {

        bool t_prevVoxel_Val = (bestVoxel != null) ?
        heatMapCubes[bestVoxel.GetComponent<SC_CubeHeatMap>().id].GetComponent<SC_CubeHeatMap>().isOccluded :
        false;  // checks if the current voxel is occluded

        if (t_prevVoxel_Val == true) {
            for (int i = 0; i < heatMapCubes.Count; i++)
            {
                if (heatMapCubes[i].GetComponent<SC_CubeHeatMap>().isOccluded == false)
                {
                    bestVoxel = heatMapCubes[i];
                    controller.UI_POS = heatMapCubes[i].GetComponent<SC_CubeHeatMap>().id;
                }
            }
        }


    }

    // Update is called once per frame
    void Update()
    {
        ui_Calc();



    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Alarm_Area")
        {
            controller.ALARM = true;
        }

        if (other.gameObject.tag == "Pump_Area")
        {
            controller.PUMP = true;
        }

        if (other.gameObject.tag == "STG_Area")
        {
            controller.STG = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Alarm_Area")
        {
            controller.ALARM = true;
        }

        if (other.gameObject.tag == "Pump_Area")
        {
            controller.PUMP = true;
        }

        if (other.gameObject.tag == "STG_Area")
        {
            controller.STG = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.tag == "Alarm_Area") || (other.gameObject.tag == "Pump_Area") || (other.gameObject.tag == "STG_Area"))
        {
            controller.ALARM = false;
            controller.PUMP = false;
            controller.STG = false;
        }

    }

}
