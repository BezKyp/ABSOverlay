using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceOpacity : MonoBehaviour
{

    public GameObject room;
    public GameObject steeringgear;
    public GameObject cabinet1;
    public GameObject platform;
    public GameObject pumpsystem;
    public GameObject cabinet2;
    
    // Start is called before the first frame update
    void Start()
    {
        float alpha = 0.5f;

        Material room_mat = room.GetComponent<Renderer>().material;
        Color room_col = new Color(room_mat.color.r, room_mat.color.g, room_mat.color.b, alpha);
        room_mat.SetColor("_Color", room_col);

        Material sg_mat = steeringgear.GetComponent<Renderer>().material;
        Color sg_col = new Color(sg_mat.color.r, sg_mat.color.g, sg_mat.color.b, alpha);
        sg_mat.SetColor("_Color", sg_col);

        Material c_mat = cabinet1.GetComponent<Renderer>().material;
        Color c_col = new Color(c_mat.color.r, c_mat.color.g, c_mat.color.b, alpha);
        c_mat.SetColor("_Color", c_col);

        Material c2_mat = cabinet2.GetComponent<Renderer>().material;
        Color c2_col = new Color(c2_mat.color.r, c2_mat.color.g, c2_mat.color.b, alpha);
        c2_mat.SetColor("_Color", c2_col);

        Material pl_mat = platform.GetComponent<Renderer>().material;
        Color pl_col = new Color(pl_mat.color.r, pl_mat.color.g, pl_mat.color.b, alpha);
        pl_mat.SetColor("_Color", pl_col);

        Material pump_mat = pumpsystem.GetComponent<Renderer>().material;
        Color pump_col = new Color(pump_mat.color.r, pump_mat.color.g, pump_mat.color.b, alpha);
        pump_mat.SetColor("_Color", pump_col);
    }

}
