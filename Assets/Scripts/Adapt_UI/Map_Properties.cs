// using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Map_Properties : MonoBehaviour
{
    public Vector3 id; //x,y,z according to the cube
    public Vector3 pos; // world position
    public int pos_id; // iter pos in list
    public bool isOccluded;
    public bool isFOV;
    public float interactionReach;
    public float occlusionWeight;
    public float finalVal;

    public Gradient g;
    public Material material_bad;
    public Material material_good;

    public LayerMask bound_layer;
    public InitializeHeatMap initializeHeatMap;
    public Global_Controller controller;




    private void Start()
    {
        GradientColorKey[] gck = new GradientColorKey[3];
        GradientAlphaKey[] gak = new GradientAlphaKey[3];
        gck[0].color = Color.green;
        gck[0].time = 0.0F;
        gck[1].color = Color.yellow;
        gck[1].time = 0.5F;
        gck[2].color = Color.red;
        gck[2].time = 1.0F;
        gak[0].alpha = 0.4F;
        gak[0].time = 0.0F;
        gak[1].alpha = 0.4F;
        gak[1].time = 0.5F;
        gak[2].alpha = 0.4F;
        gak[2].time = 1.0F;
        g.SetKeys(gck, gak);
        if (controller.renderVoxelGrad == true)
        {
            GetComponent<Renderer>().material = material_good;
        }
    }

    // Update is called once per frame
    void FixedUpdate() 
    {

        VoxelCalculation();
        
    }

    public void VoxelCalculation() // For rendering the material and placing the UI
    {
        if (isOccluded == false)
        {
            finalVal = interactionReach;
            if (controller.renderVoxelGrad == true)
            {
                GetComponent<Renderer>().material.color = g.Evaluate(finalVal);
            }
            
        }
        else
        {
            finalVal = 1;
            if (controller.renderVoxelGrad == true)
                GetComponent<Renderer>().material = material_bad;

        }
    }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Bound" || other.gameObject.layer == 6)
            {
                //print("collided");
                //GetComponent<MeshRenderer>().materials[0] = material_bad;
                //GetComponent<Renderer>().material = material_bad;
                isOccluded = true;
                initializeHeatMap.occlusionCover(pos_id);
                //occlusionCover(pos_id);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Bound" || other.gameObject.layer == 6)
            {
                //print("stay");
                //GetComponent<MeshRenderer>().materials[0] = material_bad;
                //GetComponent<Renderer>().material = material_bad;
                isOccluded = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Bound" || other.gameObject.layer == 6)
            {
                //print("exit");
                //GetComponent<MeshRenderer>().materials[0] = material_good;
                //GetComponent<Renderer>().material = material_good;
                isOccluded = false;
                initializeHeatMap.occlusionUnCover(pos_id);
        }
        }


        /* void OnCollisionEnter(Collision col)
         {
             if(col.gameObject.tag == "Bound" || col.gameObject.layer == 6)
             {
                 print("collided");
                 GetComponent<MeshRenderer>().materials[0] = material_bad;
                 isOccluded = true;
             }

         }

         void OnCollisionStay(Collision col)
         {
             if (col.gameObject.tag == "Bound" || col.gameObject.layer == 6)
             {
                 print("collided");
                 GetComponent<MeshRenderer>().materials[0] = material_bad;
                 isOccluded = true;
             }

         }*/



}
