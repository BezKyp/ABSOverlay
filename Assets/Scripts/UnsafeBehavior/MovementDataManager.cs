using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MovementDataManager : MonoBehaviour
{
    public OVRCameraRig cameraRig;
    public GameObject alertIconActive;
    public TextMeshProUGUI alertText;
    public TextMeshProUGUI infoText;
    public GameObject infoArea;
    public GameObject DebugMode;

    public float safeSpeedThreshold = 1.0f;
    public float abruptMovementThreshold = 0.05f;
    public float abruptRotationThreshold = 150.0f;
    public float backwardStepThreshold = 1.0f;
    public float spinTurnThreshold = 200.0f;
    public float notificationDuration = 3.0f;
    private float notificationTimer = 0f;

    // New Variables for Velocity Duration Check
    public float abruptMovementDurationThreshold = 0.5f; // The time a high velocity needs to last to be considered a spin turn
    private float highVelocityTime = 0f;

    private Vector3 lastPosition;
    private Vector3 lastVelocity = Vector3.zero;
    private Vector3 lastRotation;
    private float lastTime;

    private float cumulativeBackwardDistance = 0.0f;

    // Audio Variables
    public AudioSource audioSource;
    public AudioClip walkingBackwardClip;
    public AudioClip unsafeSpeedClip;
    public AudioClip abruptMovementClip;
    public AudioClip spinTurnClip;

    public GraphRenderer graphRenderer;  // Reference to GraphRenderer

    void Start()
    {
        if (cameraRig == null)
        {
            Debug.LogError("CameraRig reference is missing. Please assign it in the inspector.");
            enabled = false;
            return;
        }

        lastPosition = cameraRig.centerEyeAnchor.transform.position;
        lastRotation = cameraRig.centerEyeAnchor.transform.eulerAngles;
        lastTime = Time.time;

        SetUIState(alertIconActive, false);
        SetUIState(alertText.gameObject, false);
        SetUIState(infoText.gameObject, false);
        SetUIState(infoArea, false);
        SetUIState(DebugMode, true);
    }

    void Update()
    {
        (float cumulativeBackwardDistance, float gaitSpeed, float velocityChange, float angularVelocityYaw, float angularVelocityPitch, float angularVelocityRoll) = UpdateMovementCalculations();

        CheckAbruptMovements(gaitSpeed, velocityChange, angularVelocityYaw, angularVelocityPitch, angularVelocityRoll);
        CheckUnsafeGaitSpeed(gaitSpeed);
        CheckCumulativeBackwardDistance(cumulativeBackwardDistance);
        //CheckSpinTurning(angularVelocityYaw);

        UpdateNotificationTimer();
        UpdateUI(velocityChange, cumulativeBackwardDistance, gaitSpeed, angularVelocityYaw, angularVelocityPitch, angularVelocityRoll);

        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            //bool isDebugModeActive = !DebugMode.activeSelf;
            //DebugMode.SetActive(isDebugModeActive);
            // Toggle infoText and infoArea based on debug mode state
            bool isActive = !infoText.gameObject.activeSelf; // Toggle based on current state
            infoText.gameObject.SetActive(isActive);
            infoArea.SetActive(isActive);
        }

        if (graphRenderer != null)
        {
            graphRenderer.AddGaitSpeed(gaitSpeed);
        }
    }

    (float, float, float, float, float, float) UpdateMovementCalculations()
    {
        Vector3 currentPosition = cameraRig.centerEyeAnchor.transform.position;
        Vector3 currentRotation = cameraRig.centerEyeAnchor.transform.eulerAngles;
        float currentTime = Time.time;

        float deltaTime = Mathf.Max(currentTime - lastTime, Time.deltaTime);

        Vector3 currentVelocity = (currentPosition - lastPosition) / deltaTime;
        float velocityChange = Mathf.Abs(currentVelocity.magnitude - lastVelocity.magnitude);

        Vector3 movementDirection = currentPosition - lastPosition;
        Vector3 forwardDirection = cameraRig.centerEyeAnchor.transform.forward;

        float deltaYaw = Mathf.Abs(Mathf.DeltaAngle(lastRotation.y, currentRotation.y));
        float deltaPitch = Mathf.Abs(Mathf.DeltaAngle(lastRotation.x, currentRotation.x));
        float deltaRoll = Mathf.Abs(Mathf.DeltaAngle(lastRotation.z, currentRotation.z));

        float angularVelocityYaw = deltaYaw / deltaTime;
        float angularVelocityPitch = deltaPitch / deltaTime;
        float angularVelocityRoll = deltaRoll / deltaTime;

        lastPosition = currentPosition;
        lastRotation = currentRotation;
        lastVelocity = currentVelocity;
        lastTime = currentTime;

        Vector3 headsetVelocity = GetHeadsetVelocity();
        float gaitSpeed = headsetVelocity.magnitude;

        float dotProduct = Vector3.Dot(movementDirection, forwardDirection);
        if (dotProduct < 0)
        {
            cumulativeBackwardDistance += movementDirection.magnitude;
        }
        else
        {
            ResetCumulativeDistance();
        }

        return (cumulativeBackwardDistance, gaitSpeed, velocityChange, angularVelocityYaw, angularVelocityPitch, angularVelocityRoll);
    }

    private bool notificationActive = false;  // To track if a notification is active

    private void CheckAbruptMovements(float gaitSpeed, float velocityChange, float angularVelocityYaw, float angularVelocityPitch, float angularVelocityRoll)
    {
        if (notificationActive)
            return;

        bool isSpinTurning = angularVelocityYaw > spinTurnThreshold;
        bool isAbruptRotation = Mathf.Max(angularVelocityYaw, angularVelocityPitch, angularVelocityRoll) > abruptRotationThreshold;

        if (isSpinTurning)
        {
            highVelocityTime += Time.deltaTime;
            if (highVelocityTime > abruptMovementDurationThreshold)
            {
                HandleUnsafeNotification("Spin Turn Detected!", spinTurnClip);
                return;
            }
        }
        else
        {
            highVelocityTime = 0f;
        }

        if (isAbruptRotation)
        {
            HandleUnsafeNotification("Abrupt Movement Detected!", abruptMovementClip);
        }
    }


    //private void CheckSpinTurning(float angularVelocityYaw)
    //{
    //    // Ensure it is a rapid and sustained spin
    //    bool isSpinTurning = angularVelocityYaw > spinTurnThreshold;

    //    if (isSpinTurning)
    //    {
    //        HandleUnsafeNotification("Spin turn detected!");
    //    }
    //}

    void CheckUnsafeGaitSpeed(float gaitSpeed)
    {
        if (gaitSpeed > safeSpeedThreshold)
        {
            HandleUnsafeNotification("Unsafe Speed Detected!", unsafeSpeedClip);
        }
    }

    void CheckCumulativeBackwardDistance(float cumulativeBackwardDistance)
    {
        if (cumulativeBackwardDistance >= backwardStepThreshold)
        {
            HandleUnsafeNotification("Walking Backwards detected!", walkingBackwardClip);
        }
    }


    Vector3 GetHeadsetVelocity()
    {
        List<UnityEngine.XR.XRNodeState> nodeStates = new List<UnityEngine.XR.XRNodeState>();
        UnityEngine.XR.InputTracking.GetNodeStates(nodeStates);

        foreach (var nodeState in nodeStates)
        {
            if (nodeState.nodeType == UnityEngine.XR.XRNode.Head && nodeState.TryGetVelocity(out Vector3 velocity))
            {
                return velocity;
            }
        }
        return Vector3.zero;
    }

    private void ResetCumulativeDistance()
    {
        cumulativeBackwardDistance = 0;
        if (infoText != null)
        {
            infoText.text = "Cumulative Backward Distance: 0.0m";
        }
    }

    private void HandleUnsafeNotification(string message, AudioClip clip)
    {
        if (alertText != null)
        {
            alertText.text = message;
            alertText.gameObject.SetActive(true);
        }

        if (alertIconActive != null)
        {
            SetUIState(alertIconActive, true);
        }

        // Stop any currently playing sound and play the new one
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }

        notificationTimer = notificationDuration;
        notificationActive = true;
    }

    private void UpdateNotificationTimer()
    {
        if (notificationTimer > 0f)
        {
            notificationTimer -= Time.deltaTime;
        }
        else
        {
            // Reset notification state once the timer is done
            SetUIState(alertText.gameObject, false);
            SetUIState(alertIconActive, false);
            notificationActive = false;  // Allow new notifications after the cooldown
        }
    }

    void UpdateUI(float velocityChange, float cumulativeBackwardDistance, float gaitSpeed, float angularVelocityYaw, float angularVelocityPitch, float angularVelocityRoll)
    {
        if (infoText != null)
        {
            infoText.text = $"Gait Speed: {gaitSpeed:F2} m/s\n" +
                            $"Cumulative Backward Distance: {cumulativeBackwardDistance:F2} m\n" +
                            $"Yaw Velocity: {angularVelocityYaw:F2}°/s\n" +
                            $"Pitch Velocity: {angularVelocityPitch:F2}°/s\n" +
                            $"Roll Velocity: {angularVelocityRoll:F2}°/s";
        }
    }

    // Set the active state of UI elements
    void SetUIState(GameObject uiElement, bool isActive)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(isActive);
        }
    }
}


