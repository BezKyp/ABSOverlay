using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using System;
using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class DataPanel : MonoBehaviour
{

    public GameObject data_ui;

    // Update is called once per frame
    void Update()
    {
        if(MetaAimFlags.IndexPinching != 0) {
            if(data_ui.activeSelf) data_ui.SetActive(false);
            else data_ui.SetActive(true);
        }
    }
}
