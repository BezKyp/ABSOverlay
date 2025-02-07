using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
// using XRHandSubsystem.updatedHands;

public class RelativePosToPlayer : MonoBehaviour
{
    
    // public Transform end;
    public Transform cam;
    public GameObject warning;
    public GameObject background;
    public GameObject box;

    public GameObject pipe1;
    public GameObject pipe2;
    public GameObject pipe3;
    public GameObject hk1;
    public GameObject hk2;
    public GameObject kk1;
    public GameObject kk2;
    public GameObject th;



    private Vector3 forward;
    private Vector3 playerToHaz;
    private float height;
    public TMPro.TextMeshProUGUI tex;
    public TMPro.TextMeshProUGUI extra_data;
    string type;
    static int ind;

    static bool taken = false;
    // private bool mine = false;
    static float min_dist;

    // List<GameObject> hazs;

    void Start() {
        // hazs = new List<GameObject>();
        // hazs = GameObject.FindGameObjectsWithTag("Hazard");
        min_dist = 0.2f;
    }
    
    // Update is called once per frame
    void Update()
    {
        
        // min_dist = 0.2f;
        
        height = cam.transform.position.y - this.transform.position.y;
        //dictionary

            switch(this.gameObject.name) {
                case "Pipe":
                    if (height > 0.5f) type = "Trip Hazard";
                    else if (height > 0) type = "Knee Knocker";
                    else type = "Head Knocker";
                    break;
                case "Hole":
                    type = "Fall Hazard";
                    break;
                case "clipboard":
                case "Head Knocker":
                    /*if (height > 0) */type = "Head Knocker";
                    break;
                case "Stairs":
                    type = "Trip Hazard";
                    break;
                case "Moving Object":
                    type = "Moving Hazard";
                    break;
                case "Spill":
                    type = "Slip Hazard";
                    break;
            }

        //relative pos
        GameObject[] hazs = {pipe1, pipe2, pipe3, hk1, hk2, kk1, kk2, th};
        
        // ind = 2;
        
        float h_dir, v_dir;
        Vector3 closest;
        forward = Vector3.Normalize(cam.TransformDirection(Vector3.forward));

        closest = (this.gameObject.GetComponent<Collider>().ClosestPoint(cam.position));
        playerToHaz = closest - cam.transform.position;
        playerToHaz[1] = 0;
        if(playerToHaz.magnitude < 1.0f) box.SetActive(true);
        else box.SetActive(false);


        for(int i = 0; i < 8; i++) {
            closest = (hazs[i].GetComponent<Collider>().ClosestPoint(cam.position));

            playerToHaz = closest - cam.transform.position;
                // Debug.Log("i: " + i + ", dist: " + playerToHaz.magnitude + "\n");
            if(playerToHaz.magnitude <= min_dist) {
                min_dist = playerToHaz.magnitude;
                ind = i;
            }
        }



                // Debug.Log("ind: " + ind + ", min_dist: " + min_dist + "\n");
        if(hazs[ind] == this.gameObject /*|| !taken*/) {
            // Debug.Log("ind: " + ind + "\n");
            closest = (this.gameObject.GetComponent<Collider>().ClosestPoint(cam.position));

            forward = Vector3.Normalize(cam.TransformDirection(Vector3.forward));
            playerToHaz = closest - cam.transform.position;
            h_dir = Vector3.Dot(playerToHaz, cam.transform.right);
            v_dir = cam.TransformDirection(Vector3.forward)[1] - playerToHaz[1];
            forward[1] = 0;
            playerToHaz[1] = 0;

            taken = true;
            // mine = true;
            min_dist = playerToHaz.magnitude;
            tex.text = type;
            extra_data.text = type + "\n\n";

            // box.SetActive(true);
            // background.SetActive(true);
            playerToHaz = Vector3.Normalize(playerToHaz);
            float d = Vector3.SignedAngle(forward, playerToHaz, Vector3.forward);
            extra_data.text += "Horizontal angle: " + d.ToString("0.0");
            if(h_dir < 0) {
                tex.text += " to the left\n";
                extra_data.text += " left\n\n";
            }
            else {
                tex.text += " to the right\n";
                extra_data.text += " right\n\n";
            }

            forward = Vector3.Normalize(cam.TransformDirection(Vector3.forward));
            forward[0] = 0;
            playerToHaz = closest - cam.transform.position;
            playerToHaz[0] = 0;
            playerToHaz = Vector3.Normalize(playerToHaz);
            float v = Vector3.SignedAngle(forward, playerToHaz, Vector3.up);
            extra_data.text += "Vertical angle: " + v.ToString("0.0");
            if(v_dir > 0) {
                tex.text += " and down ";
                extra_data.text += " down\n\n";
            }
            else {
                tex.text += " and up ";
                extra_data.text += " down\n\n";
            }

            if (v > 60 || d > 60) {
                tex.text += "out of view\n";
                extra_data.text += "Out of view\n\n";
            }
            else if (v > 16 || d > 16) {
                tex.text += "in peripheral\n";
                extra_data.text += "Peripheral\n\n";
            }
            else extra_data.text += "Direct view\n\n";

            taken = false;
        }
        else {
            // background.SetActive(false);
            // tex.text = "";
            taken = false;
            // mine = false;
            // box.SetActive(false);
        }
        
    }
}
