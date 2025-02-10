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
    public Animator alertanim;
    public AudioSource alertsfx;
    public GameObject pipe1;
    public GameObject pipe2;
    public GameObject pipe3;
    public GameObject hk1;
    public GameObject hk2;
    public GameObject kk1;
    public GameObject kk2;
    public GameObject th;


    private int num_hk;
    private int num_kk;
    private int num_th;
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
        min_dist = 1.0f;
    }
    
    // Update is called once per frame
    void Update()
    {
        extra_data.text = "Closest hazard:\n\n";
        // min_dist = 0.2f;
        
        height = cam.transform.position.y - this.transform.position.y;
        //dictionary

            switch(this.gameObject.name) {
                case "Pipe":
                    if (height > 0f) type = "Trip Hazard";
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
        if(playerToHaz.magnitude < 1.0f) {
            box.SetActive(true);
            if(tex.text == "") {
                alertanim.SetTrigger("Appear");
                alertsfx.Play();
            }
            if(!tex.text.Contains(type)) tex.text += type + " ";
        }
        else {
            box.SetActive(false);
            tex.text.Replace(type, "");
        }

        // num_hk = 0;
        // num_kk = 0;
        // num_th = 0;
        for(int i = 0; i < 8; i++) {
            closest = (hazs[i].GetComponent<Collider>().ClosestPoint(cam.position));

            playerToHaz = closest - cam.transform.position;
            playerToHaz[1] = 0;
                // Debug.Log("i: " + i + ", dist: " + playerToHaz.magnitude + "\n");
            if(playerToHaz.magnitude <= min_dist) {
                min_dist = playerToHaz.magnitude;
                ind = i;
            }
        }

        if(min_dist >= 0.2f) {
            // tex.text = "";
            // alertanim.SetTrigger("Disappear");

        }

        if(hazs[ind] == this.gameObject) {
            closest = (this.gameObject.GetComponent<Collider>().ClosestPoint(cam.position));
            
            forward = Vector3.Normalize(cam.TransformDirection(Vector3.forward));
            playerToHaz = closest - cam.transform.position;
            h_dir = Vector3.Dot(playerToHaz, cam.transform.right);
            v_dir = cam.TransformDirection(Vector3.forward)[1] - playerToHaz[1];
            forward[1] = 0;
            playerToHaz[1] = 0;

            taken = true;
            min_dist = playerToHaz.magnitude;
            extra_data.text += type + "\n\n";

            playerToHaz = Vector3.Normalize(playerToHaz);
            float d = Vector3.SignedAngle(forward, playerToHaz, Vector3.forward);
            extra_data.text += "Horizontal angle: " + d.ToString("0.0");
            if(h_dir < 0) {
                extra_data.text += " left\n\n";
            }
            else {
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
                extra_data.text += " down\n\n";
            }
            else {
                extra_data.text += " down\n\n";
            }

            if (v > 60 || d > 60) {
                extra_data.text += "Out of view\n\n";
            }
            else if (v > 16 || d > 16) {
                extra_data.text += "Peripheral\n\n";
            }
            else extra_data.text += "Direct view\n\n";

            taken = false;
        }
        
    }
}
