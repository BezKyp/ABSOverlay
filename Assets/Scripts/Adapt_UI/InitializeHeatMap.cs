using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
//using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class InitializeHeatMap : MonoBehaviour
{
    public float armReach;

    public GameObject cube;
    public Vector3 bounds;
    public List<List<GameObject>> heatMapList;
    public List<GameObject> heatMapCubes;
    public Canvas UI;
    public List<GameObject> UI_Colliders;
    //public Camera camera;
    public GameObject bestVoxel;
    private List<int> tempOccludedVoxels = new List<int>();

    //public MouseMovementTracker mouseMove;

    public Global_Controller controller;

    private float time = 0.0f;    
    public float interpolationPeriod = 2f;

    private Vector3 targetPosition; // The new voxel position
    private Vector3 velocity = Vector3.zero; // Needed for SmoothDamp
    public float transitionSpeed = 5f; // Controls movement speed

    // cube set scale to 0.2 to match to 0.2x0.2 canvas
    // so just set the scale to the w/h of the canvas
    // and divide the UC_size w/h/l by the scale of the cube to find out how many can be placed in each direction


    // Start is called before the first frame update
    void Start()
    {
        float UI_w = UI.GetComponent<RectTransform>().rect.width;
        float UI_h = UI.GetComponent<RectTransform>().rect.height;

        Vector3 UC_size = this.GetComponent<MeshFilter>().sharedMesh.bounds.size; 
        Vector3 UC_scale =  this.GetComponent<Transform>().localScale;

        UC_size = new Vector3(UC_size.x * UC_scale.x, UC_size.y * UC_scale.y, UC_size.z * UC_scale.z);

        float armR = armReach * 0.1f;

        //bounds = new Vector3(Mathf.Round(UC_size[0]/UI_w), Mathf.Round((UC_size[2]) / (armR)), Mathf.Round(UC_size[1] / UI_h));
        //bounds = new Vector3(Mathf.Round(UC_size[0]/UI_w), 10, Mathf.Round(UC_size[1] / UI_h));
        bounds = new Vector3(Mathf.Round(UC_size[1] / UI_h), Mathf.Round(UC_size[0] / UI_w), 10);

        // rounding the bounds, if 
        

        //Mathf.Floor
        float numCubes = bounds[0] * bounds[1] * bounds[2];


        float x_offset = UI_w / 2;
        float y_offset = UI_h / 2;

        cube.GetComponent<Transform>().localScale = new Vector3(UI_w, UI_h, armR); // resize cube
        //heatMapList.Add(cube);


        int pos_id = 0;

        heatMapList = new List<List<GameObject>>();

        for (int i = 0; i < bounds[1]; i++)
        {
            for (int j = 0; j < (bounds[0] - 1); j++)
            {
                for (int k = 0; k < (bounds[2]); k++)
                {
                    GameObject cubeCopy = Instantiate(cube, this.transform, worldPositionStays: false);
                    cubeCopy.GetComponent<Transform>().localPosition = new Vector3(cubeCopy.GetComponent<Transform>().localPosition[0] + (UI_w * i) + (UI_w / 2), cubeCopy.GetComponent<Transform>().localPosition[1] - (UI_h * j) - (UI_h / 2), cubeCopy.GetComponent<Transform>().localPosition[2] + ((armR) * k) + (armR / 2.0f));
                    cubeCopy.gameObject.name = "Cube" + i + j + k;
                    cubeCopy.GetComponent<Map_Properties>().id = new Vector3(i, j, k);
                    cubeCopy.GetComponent<Map_Properties>().pos = cubeCopy.GetComponent<Transform>().localPosition;
                    cubeCopy.GetComponent<Map_Properties>().pos_id = pos_id;
                    cubeCopy.GetComponent<Transform>().localScale = new Vector3(0.03f, 0.03f, 0.04f);

                    pos_id += 1;

                    heatMapCubes.Add(cubeCopy);
                }
                
            }
        }

        armReachCalc();
        

        cube.GetComponent<MeshRenderer>().enabled = false; //make og cube not visible

        if (!controller.renderBlueContainer)
        {
            this.GetComponent<Renderer>().enabled = false;//

        }

        ui_Calc();

    }


    void armReachCalc()
    {

        for (int i = 0; i < heatMapCubes.Count; i++)
        {
            Vector3 loc = heatMapCubes[i].GetComponent<Map_Properties>().id;
            //Vector3 middle = new Vector3((bounds[0] / 2)-1, bounds[2] / 2, bounds[1] / 2);
            Vector3 middle = new Vector3(bounds[1] / 2, (bounds[0] / 2) - 1, bounds[2] / 2);

            float reachCalc = (((Math.Abs(loc[0] - middle[0]) / middle[0])*0.2f) + ((Math.Abs(loc[2] - middle[2]) / middle[2]) * 0.4f) + ((Math.Abs(loc[1] - middle[1]) / middle[1]) * 0.4f));

            //Vector3 x = new Vector3((loc[0]/(bounds[0]-1)), (loc[1] / (bounds[1]-1)), (loc[2] / (bounds[2]-1)));

            /*float reachCalc = (((-4 * Mathf.Pow(x[0], 2)) + (4 * x[0])) * 0.333f) 
                + (((-4 * Mathf.Pow(x[2], 2)) + (4 * x[2])) * 0.333f) 
                +(((-4 * Mathf.Pow(x[1], 2)) + (4 * x[1])) * 0.333f);*/

        
            heatMapCubes[i].GetComponent<Map_Properties>().interactionReach = reachCalc;

            // heatMapCubes[i].GetComponent<Renderer>().material.color = g.Evaluate(reachCalc);


        }
    }

    public void occlusionCover(int iter)
    {

        for (int i = Mathf.FloorToInt(heatMapCubes[iter].GetComponent<Map_Properties>().id[2]); i < (bounds[2]-1); i++)
        {
            heatMapCubes[iter].GetComponent<Map_Properties>().isOccluded = true;
            iter += 1;
        }
    }

    public void occlusionUnCover(int iter)
    {
        
        for (int i = Mathf.FloorToInt(heatMapCubes[iter].GetComponent<Map_Properties>().id[2]); i < (bounds[2] - 1); i++)
        {
            heatMapCubes[iter].GetComponent<Map_Properties>().isOccluded = false;
            iter += 1;
        }
    }

    public void tempOcclusionCover(int iter)
    {

        for (int i = Mathf.FloorToInt(heatMapCubes[iter].GetComponent<Map_Properties>().id[2]); i < (bounds[2] - 1); i++)
        {
            if (heatMapCubes[iter].GetComponent<Map_Properties>().isOccluded == false)
            {
                heatMapCubes[iter].GetComponent<Map_Properties>().isOccluded = true;
                heatMapCubes[iter].GetComponent<Map_Properties>().VoxelCalculation();
                tempOccludedVoxels.Add(iter);
                //Debug.Log("Temporary Occluded Voxels: " + tempOccludedVoxels);
                iter += 1;

            }
            else
            {
                break;
            }
            
        }



    }

    public void tempOcclusionUnCover()
    {
        if (tempOccludedVoxels != null)
        {
            for (int i = 0; i < tempOccludedVoxels.Count; i++)
            {
                heatMapCubes[tempOccludedVoxels[i]].GetComponent<Map_Properties>().isOccluded = false;
                heatMapCubes[tempOccludedVoxels[i]].GetComponent<Map_Properties>().VoxelCalculation();
            }
            tempOccludedVoxels.Clear();
        }
        
    }

    private int bestVoxelSearch(int int_voxel, float temp_int)
    {
        for (int i = 0; i < heatMapCubes.Count; i++)
        {
            if (heatMapCubes[i].GetComponent<Map_Properties>().finalVal < temp_int)
            {
                //bestVoxel = heatMapCubes[i];
                temp_int = heatMapCubes[i].GetComponent<Map_Properties>().finalVal;
                int_voxel = i;
            }

        }

        return int_voxel;
    }

    /*private bool IsUIVisible(GameObject UI_Collider)
    {
        if (heatMapCubes == null || UI_Collider == null) return false;

           
            int heatMapLayer = bestVoxel.layer;
            int layerMask = ~(1 << heatMapLayer); // Exclude heatMapCube's layer

            Vector3 origin = camera.GetComponent<Transform>().position;
            Vector3 targetCenter = UI_Collider.GetComponent<Transform>().position;
            Vector3 direction = (targetCenter - origin).normalized;

            // Perform the raycast with the layer mask
            if (Physics.Raycast(origin, direction, out RaycastHit raycastHit, Mathf.Infinity, layerMask))
            {
                // Visualize the raycast
                //Debug.DrawLine(origin, raycastHit.point, Color.green, 1f); // Green if it hits something
                                                                           //Debug.Log($"Ray hit: {raycastHit.transform.name} at distance {raycastHit.distance}");

                if (raycastHit.collider.tag == UI_Collider.tag)
                {
                    //Debug.Log("Raycast hit the objective: " + UI_Colliders[0].name);
                    return true;
                }
                else
                {
                    //Debug.Log("Raycast hit something else: " + raycastHit.transform.name);
                    return false;
                }
            }
            else
            {
                // Visualize a missed raycast
                //Debug.DrawLine(origin, origin + direction * 100f, Color.red, 1f); // Red if no hit
                                                                                  //Debug.Log("Raycast missed everything.");
                return false;
            }

            /*

                // Cast center ray
                if (Physics.Raycast(origin, direction, out RaycastHit hit, Mathf.Infinity, layerMask))
                {
                    hits[0] = hit.collider.gameObject == UI_Colliders[0].gameObject;
                    Debug.DrawRay(origin, (targetCenter - origin), hits[0] ? Color.green : Color.red, 1f);
                }

                // Cast rays to corners
                for (int i = 1; i < (UI_Colliders.Count); i++)
                {
                    if (Physics.Raycast(origin, (UI_Colliders[i].GetComponent<Transform>().position - origin).normalized, out RaycastHit cornerHit, Mathf.Infinity, layerMask))
                    {
                        hits[i] = cornerHit.collider.gameObject == UI_Colliders[i].gameObject;
                        Debug.DrawRay(origin, (UI_Colliders[i].GetComponent<Transform>().position - origin), hits[i] ? Color.green : Color.red, 1f);
                    }
                }

                // If any of the rays failed to reach UI_Collider, return false
                if (hits.Contains(false))
                {
                    Debug.Log("UI is NOT fully visible!");
                    return false;
                }

                Debug.Log("UI is fully visible!");
                return true;
                */


       // } 

    public void ui_Calc()
    {
        //bestVoxel = heatMapCubes[0]; // initialize as first cube
        int int_voxel = 0;
        float temp_int = 2; // made this 2 since all voxel calc values will be less than two (voxel range 0-1)
        int_voxel = bestVoxelSearch(int_voxel, temp_int);

        float prevVoxel_Val = (bestVoxel != null) ?
        heatMapCubes[bestVoxel.GetComponent<Map_Properties>().pos_id].GetComponent<Map_Properties>().finalVal :
        2f;

        float newVoxelVal = heatMapCubes[int_voxel].GetComponent<Map_Properties>().finalVal;

        if ((prevVoxel_Val >= 0.5) || (((prevVoxel_Val - newVoxelVal) / prevVoxel_Val) >= 0.45)) // Should stabilize voxel movement a lil
        {
            bestVoxel = heatMapCubes[int_voxel];
            //bestVoxel.GetComponent<Transform>().localPosition = heatMapCubes[int_voxel].GetComponent<Transform>().localPosition;
            targetPosition = bestVoxel.GetComponent<Transform>().localPosition; // Set new target position

            //UI.GetComponent<Transform>().rotation = Quaternion.Slerp(UI.GetComponent<Transform>().rotation, camera.GetComponent<Transform>().rotation, 5f * Time.deltaTime);

            Vector3 Col_Transform = UI_Colliders[0].GetComponent<Transform>().transform.localPosition - targetPosition;

            UI_Colliders[0].GetComponent<Transform>().transform.localPosition = bestVoxel.GetComponent<Transform>().localPosition;

            for (int i = 1; i < 5; i++)
            {
                UI_Colliders[i].GetComponent<Transform>().transform.localPosition -= Col_Transform;
            }


            Physics.SyncTransforms(); // Force Unity to update the physics system
        }


        

        // UI_Collider.GetComponent<Transform>().rotation = Quaternion.Slerp(UI.GetComponent<Transform>().rotation, camera.GetComponent<Transform>().rotation, 5f * Time.deltaTime);


        // raycast check if Voxel is actually fully visible



    }

    /*
    private void FixedUpdate()
    {
        time += Time.deltaTime;

        //mouseMove.GetMouseSpeed();

        if (time >= interpolationPeriod)
        {

            // if the current best voxel has majorly changed in quality value then recalculate
            //ui_calc while loop
            // check raytrace
            // else: just continue

            tempOcclusionUnCover();

            int iter = 0;

            bool flag = false;
            while (!flag) 
            {
                ui_Calc();

                if (iter > 5) // creating a quit from the loop so it doesnt go forever, only for demo
                {
                    flag = true;
                }

                bool[] hits = new bool[5];

                for (int i = 0; i < 5; i++)
                {
                    hits[i] = IsUIVisible(UI_Colliders[i]);
                }
                
                if (hits.Contains(false))
                {
                    tempOcclusionCover(bestVoxel.GetComponent<Map_Properties>().pos_id);
                    iter += 1;
                }
                else { flag = true; }

            }

            
            //Debug.Log("UI_Collider NOW Located in: " + UI_Collider.GetComponent<Transform>().transform.localPosition);

            time = time - interpolationPeriod;
        }

        if (UI != null)
        {
            UI.transform.localPosition = Vector3.Lerp(UI.transform.localPosition, targetPosition, transitionSpeed * Time.deltaTime);
            UI_Colliders[0].transform.localPosition = UI.transform.localPosition; // Keep collider in sync
        }

    }*/


}
