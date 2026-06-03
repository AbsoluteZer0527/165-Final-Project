using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;
using UnityEngine.XR.Hands.Samples.GestureSample;

public class HandController : MonoBehaviour
{

    [Header("Hand Tracking Settings")]
    [SerializeField] XRHandTrackingEvents leftHandTrackingEvents;
    [SerializeField] XRHandTrackingEvents rightHandTrackingEvents;
    [SerializeField] XRHandShape[] handShapes;
    [SerializeField] float gestureDetectionInterval = 0f;
    [SerializeField] float minGestureThreshold = 0.9f;
    [SerializeField] HandShapeCompletenessCalculator handShapeCompletenessCalculator;

    [Header("Camera Settings")]
    [SerializeField] float cameraChangeInterval = 1f;

    [Header("References")]


    // Private variables
    private XRHandSubsystem m_HandSubsystem;
    private float timeOfLastGestureDetectionRight;
    private float timeOfLastGestureDetectionLeft;
    private float timeOfLastCameraChange = 0f;
    private int currentCameraIndex = 0;

    void Start()
    {
        var handSubsystems = new List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(handSubsystems);

        for (var i = 0; i < handSubsystems.Count; ++i)
        {
            var handSubsystem = handSubsystems[i];
            if (handSubsystem.running)
            {
                m_HandSubsystem = handSubsystem;
                break;
            }
        }

        if (m_HandSubsystem != null)
            m_HandSubsystem.updatedHands += OnUpdatedHands;
    }

    void OnUpdatedHands(XRHandSubsystem subsystem,
        XRHandSubsystem.UpdateSuccessFlags updateSuccessFlags,
        XRHandSubsystem.UpdateType updateType)
    {

        switch (updateType)
        {
            case XRHandSubsystem.UpdateType.Dynamic:
                // Update game logic that uses hand data
                Debug.Log("Dynamic");
                break;
            case XRHandSubsystem.UpdateType.BeforeRender:
                // Update visual objects that use hand data
                Debug.Log("Before Render");
                break;
        }
    }
    public void OnJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        // Only Move with Right Index Finger
        if (args.hand.handedness == Handedness.Right && args.hand.GetJoint(XRHandJointID.IndexTip).TryGetPose(out var pose))
        {
            Debug.Log($"Index rotation: {pose.rotation.eulerAngles}");

            Vector3 worldDir = pose.rotation * Vector3.forward;
            //droneController.MoveDir = droneController.vrPlayer.transform.InverseTransformDirection(worldDir);
        }

        float lastDetection = args.hand.handedness == Handedness.Right
        ? timeOfLastGestureDetectionRight
        : timeOfLastGestureDetectionLeft;

        if (Time.time - lastDetection > gestureDetectionInterval)
        {
            foreach (var handShape in handShapes)
            {
                handShapeCompletenessCalculator.TryCalculateHandShapeCompletenessScore(args.hand, handShape, out float completenessScore);
                bool isDetected = completenessScore >= minGestureThreshold;
                Debug.Log($"Detected gesture: [{args.hand.handedness}] {handShape.name} with completeness: {completenessScore}");
                // Open Hand Shape: Stops Movement
                if (isDetected && handShape == handShapes[0] && args.hand.handedness == Handedness.Right)
                {
                    // Perform actions based on the detected gesture
                    //droneController.MoveDir = Vector3.zero;
                }
                // Thumbs Up Hand Shape: Starts Game
                else if (isDetected && handShape == handShapes[1] && args.hand.handedness == Handedness.Right)
                {
                    //droneController.StartCoroutine(droneController.GameCountdown());
                }
                else if (isDetected && handShape == handShapes[1] && args.hand.handedness == Handedness.Left)
                {
                    if (Time.time - timeOfLastCameraChange > cameraChangeInterval)
                    {
                        currentCameraIndex = (currentCameraIndex + 1) % 3;
                        if (currentCameraIndex == 2)
                        {
                            //droneController.transform.GetChild(0).gameObject.SetActive(false);
                        }
                        else
                        {
                            //droneController.transform.GetChild(0).gameObject.SetActive(true);
                            //droneController.is3rdPersonView = currentCameraIndex == 1;
                        }

                        timeOfLastCameraChange = Time.time; 
                    }

                }
            }

            if (args.hand.handedness == Handedness.Right)
                timeOfLastGestureDetectionRight = Time.time;
            else
                timeOfLastGestureDetectionLeft = Time.time;
        }
        
    }
}

