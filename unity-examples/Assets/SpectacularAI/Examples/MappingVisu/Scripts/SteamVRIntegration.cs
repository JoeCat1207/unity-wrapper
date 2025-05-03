// SteamVRIntegration.cs
// Adds runtime integration with SteamVR (OpenVR) for MappingVisu example.
// Main Camera becomes the VR headset, with poses driven by SpectacularAI VIO.
// Spoofs SteamVR runtime to use SpectacularAI poses for both HMD and base stations (Valve Index).
// Requires SteamVR Unity plugin (com.valvesoftware.steamvr).
#if UNITY_STANDALONE && !UNITY_ANDROID
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using SpectacularAI.DepthAI;
using Valve.VR;

namespace SpectacularAI.Examples.MappingVisu
{
    /// <summary>
    /// Initializes SteamVR runtime and overrides headset poses with SpectacularAI VIO output.
    /// Creates a persistent GameObject before scene load to manage VR integration.
    /// </summary>
    internal class SteamVRIntegration : MonoBehaviour
    {
        [Tooltip("Key to reset pose origin")]
        public KeyCode ResetKey = KeyCode.R;
        [Tooltip("Optional transform to define world origin for pose resets")]
        public Transform Origin;

        private void Awake()
        {
            // Load OpenVR (SteamVR) as XR device at runtime
            StartCoroutine(LoadOpenVR());
        }

        private IEnumerator LoadOpenVR()
        {
            XRSettings.LoadDeviceByName("OpenVR");
            yield return null;
            XRSettings.enabled = true;
            // Setup pose provider on main camera after VR is enabled
            SetupPoseProvider();
        }

        private void SetupPoseProvider()
        {
            var cam = UnityEngine.Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("SteamVRIntegration: No Main Camera found to attach PoseProvider.");
                return;
            }
            // Attach SpectacularAI PoseProvider to drive camera transform
            var poseProvider = cam.gameObject.AddComponent<PoseProvider>();
            poseProvider.ResetKey = ResetKey;
            poseProvider.Origin = Origin;
        }
        
        private void OnEnable()
        {
            SteamVR_Events.NewPoses.Listen(OnNewPoses);
        }

        private void OnDisable()
        {
            SteamVR_Events.NewPoses.Remove(OnNewPoses);
        }

        private void OnNewPoses(TrackedDevicePose_t[] poses)
        {
            var output = Vio.Output;
            if (output != null && output.Status == TrackingStatus.TRACKING)
            {
                var unityPose = output.Pose.AsMatrix();
                var hmdMat = new HmdMatrix34_t
                {
                    m0 = unityPose.m00, m1 = unityPose.m10, m2 = unityPose.m20, m3 = unityPose.m30,
                    m4 = unityPose.m01, m5 = unityPose.m11, m6 = unityPose.m21, m7 = unityPose.m31,
                    m8 = unityPose.m02, m9 = unityPose.m12, m10 = unityPose.m22, m11 = unityPose.m32
                };
                int hmdIndex = (int)OpenVR.k_unTrackedDeviceIndex_Hmd;
                if (hmdIndex < poses.Length)
                {
                    poses[hmdIndex].mDeviceToAbsoluteTracking = hmdMat;
                    poses[hmdIndex].bPoseIsValid = true;
                    poses[hmdIndex].bDeviceIsConnected = true;
                }
                var system = OpenVR.System;
                if (system != null)
                {
                    for (uint i = 0; i < poses.Length; ++i)
                    {
                        if (system.GetTrackedDeviceClass(i) == ETrackedDeviceClass.TrackingReference)
                        {
                            poses[i].mDeviceToAbsoluteTracking = hmdMat;
                            poses[i].bPoseIsValid = true;
                            poses[i].bDeviceIsConnected = true;
                        }
                    }
                }
            }
        }

    }

    /// <summary>
    /// Ensures SteamVRIntegration is instantiated before any scene loads.
    /// </summary>
    internal static class VRInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            var go = new GameObject("SteamVRIntegration");
            Object.DontDestroyOnLoad(go);
            go.AddComponent<SteamVRIntegration>();
        }
    }
}
#endif