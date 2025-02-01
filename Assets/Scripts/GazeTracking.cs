using UnityEngine;
using System.IO;
using System;

public class GazeTracking : MonoBehaviour
{
    GameObject child;
    private StreamWriter fileWriter;
    private Vector3 collision_pos;

    void Start() {
        // Open the file for writing (append mode)
        string filePath = Application.persistentDataPath + "/collision_data.txt";
        Debug.Log("File Path: " + filePath); // Debug log
        fileWriter = new StreamWriter(filePath, true);

        // collision_pos = (Vector3)(0.0f,0.0f,0.0f);
    }

    void OnCollisionEnter(Collision collision) {
        switch(collision.gameObject.tag) {
            case "Hazard":

                // break;
            case "Fall Hazard":

                // break;
            case "Trip Hazard":

                // break;
            case "Slip Hazard":

                // break;
            case "Head Knocker":

                // break;
            case "Knee Knocker":

                // break;
            case "Moving Hazard":
                child = collision.gameObject.transform.GetChild(0).gameObject;
                child.SetActive(true);
                break;
        }


    }

    void OnCollisionStay(Collision collision) {
        if(collision_pos != collision.contacts[0].point) {
            
            switch(collision.gameObject.tag) {
                case "Hazard":

                    // break;
                case "Fall Hazard":

                    // break;
                case "Trip Hazard":

                    // break;
                case "Slip Hazard":

                    // break;
                case "Head Knocker":

                    // break;
                case "Knee Knocker":

                    // break;
                case "Moving Hazard":
                    collision_pos = collision.contacts[0].point;
                    WriteToLogFile();
                    break;
            }
        }
        

    }

    void OnCollisionExit(Collision collision) {
        child.SetActive(false);
    }

    private void WriteToLogFile()
    {
        // Write data to the file in the desired format
        string data = $"{collision_pos}\n";
        Debug.Log("Writing to file: " + data); // Debug log
        fileWriter.WriteLine(data);
        fileWriter.Flush(); // Flush the data to the file immediately
    }

    private void OnApplicationQuit()
    {
        // Close the file when the application is quit
        fileWriter.Close();
    }

}
