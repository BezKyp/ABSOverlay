using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Jobs;
using UnityEngine.Serialization;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Unity.XR.CoreUtils;


public class Alignment : MonoBehaviour
{
    [SerializeField]
    ARTrackedImageManager m_TrackedImageManager;
    [SerializeField]
    GameObject marker_env;

    [SerializeField] TMPro.TextMeshProUGUI tex;

    void Start() {
        // tex.text = "This is running";
    }
    void OnEnable() { 
        tex.text = "in onenable";
        m_TrackedImageManager.trackedImagesChanged += OnChanged;
    }

    void OnDisable() => m_TrackedImageManager.trackedImagesChanged -= OnChanged;

    void OnChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        tex.text = "Something changed";
        foreach (var newImage in eventArgs.added)
        {
            // Handle added event
            if(newImage.referenceImage.name == "Marker1") {
               tex.text = "Found image at " + newImage.transform.position + "\n";
                marker_env.transform.position = newImage.transform.position;
                marker_env.transform.rotation = newImage.transform.rotation; 
            }
            
        }

        foreach (var updatedImage in eventArgs.updated)
        {
            // Handle updated event
        }

        foreach (var removedImage in eventArgs.removed)
        {
            // Handle removed event
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_TrackedImageManager == null)
        {
            tex.text = "Could not find tracked image manager";
        }
    }
}
