using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;



namespace UnityEngine.XR.Interaction.Toolkit.Samples.Hands
{
    /// <summary>
    /// Example class that reads a float value from an <see cref="XRInputValueReader"/> and converts it to a bool.
    /// Useful with hand interaction because the bool select value can be unreliable when the hand is near the tight internal threshold.
    /// </summary>
    [DefaultExecutionOrder(XRInteractionUpdateOrder.k_XRInputDeviceButtonReader)]
    public class ValueDerivedButtonReader : MonoBehaviour, IXRInputButtonReader
    {

        [SerializeField] GameObject data_ui;
        public bool renderHazards;
        public bool renderUI;

        public bool renderVoxelGrad;
        public bool renderBlueContainer;

        private List<UnityEngine.XR.InputDevice> inputDevices;


        [SerializeField]
        [Tooltip("The input reader used to reference the float value to convert to a bool.")]
        XRInputValueReader<float> m_ValueInput = new XRInputValueReader<float>("Value");

        [SerializeField]
        [Tooltip("The threshold value to use to determine when the button is pressed. Considered pressed equal to or greater than this value.")]
        [Range(0f, 1f)]
        float m_PressThreshold = 0.85f;
        
        [SerializeField]
        [Tooltip("The threshold value to use to determine when the button is released when it was previously pressed. Keeps being pressed until falls back to a value of or below this value.")]
        [Range(0f, 1f)]
        float m_ReleaseThreshold = 0.25f;

        bool m_IsPerformed;
        bool m_WasPerformedThisFrame;
        bool m_WasCompletedThisFrame;



        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnEnable()
        {
            if (renderHazards == true)
            {
                data_ui.SetActive(false);
            }
            if (renderUI == true)
            {
                renderVoxelGrad = false;
            }

            m_ValueInput?.EnableDirectActionIfModeUsed();
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void OnDisable()
        {
            if (renderHazards == true)
            {
                data_ui.SetActive(true);
            }
            if (renderUI == true)
            {
                renderVoxelGrad = true;
            }
            
            
            m_ValueInput?.DisableDirectActionIfModeUsed();
        }

        void Start()
        {
            inputDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevices(inputDevices);

            foreach (var device in inputDevices)
            {
                Debug.Log(string.Format("Device found with name '{0}' and role '{1}'", device.name, device.role.ToString()));
            }

            var leftHandDevices = new List<UnityEngine.XR.InputDevice>();
            UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.LeftHand, leftHandDevices);

            if (leftHandDevices.Count == 1)
            {
                UnityEngine.XR.InputDevice device = leftHandDevices[0];
                Debug.Log(string.Format("Device name '{0}' with role '{1}'", device.name, device.role.ToString()));
            }
            else if (leftHandDevices.Count > 1)
            {
                Debug.Log("Found more than one left hand!");
            }


        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        void Update()
        {
            var prevPerformed = m_IsPerformed;
            var pressAmount = m_ValueInput.ReadValue();

            var newValue = pressAmount >= m_PressThreshold;
            if (!newValue && prevPerformed)
                newValue = pressAmount > m_ReleaseThreshold;

            m_IsPerformed = newValue;
            m_WasPerformedThisFrame = !prevPerformed && m_IsPerformed;
            m_WasCompletedThisFrame = prevPerformed && !m_IsPerformed;
        }

        /// <inheritdoc />
        public bool ReadIsPerformed()
        {
            return m_IsPerformed;
        }

        /// <inheritdoc />
        public bool ReadWasPerformedThisFrame()
        {
            return m_WasPerformedThisFrame;
        }

        /// <inheritdoc />
        public bool ReadWasCompletedThisFrame()
        {
            return m_WasCompletedThisFrame;
        }

        /// <inheritdoc />
        public float ReadValue()
        {
            return m_ValueInput.ReadValue();
        }

        /// <inheritdoc />
        public bool TryReadValue(out float value)
        {
            return m_ValueInput.TryReadValue(out value);
        }
    }
}