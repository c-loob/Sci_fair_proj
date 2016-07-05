using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

namespace QuarkVr.Scripts
{
    public class QuarkVRMovingCamera : MonoBehaviour
    {
        //#if UNITY_IPHONE && !UNITY_EDITOR
        //	[DllImport ("__Internal")]
        //#else
        [DllImport("QuarkVrUnityPlugin")]
        //#endif
        private static extern bool QuarkVrInit(IntPtr leftEyeTex, IntPtr rightEyeTex);

        // We'll pass native pointer to a texture in Unity.
        // The plugin will fill texture data from native code.
        //#if UNITY_IPHONE && !UNITY_EDITOR
        //	[DllImport ("__Internal")]
        //#else
        [DllImport("QuarkVrUnityPlugin")]
        //#endif
        private static extern void SetTextureFromUnity(System.IntPtr texture);

        //#if UNITY_IPHONE && !UNITY_EDITOR
        //	[DllImport ("__Internal")]
        //#else
        [DllImport("QuarkVrUnityPlugin")]
        //#endif
        private static extern IntPtr GetRenderEventFunc();

        private bool hasData = false;
        private Quaternion quaternion;

        public Camera leftEyeCamera { get; private set; }
        public Camera rightEyeCamera { get; private set; }
        public Transform lefteEyeAnchor { get; private set; }
        public Transform rightEyeAnchor { get; private set; }
        private GameObject qvrCameraRig;
        private Camera mainCam;
        private bool isInit = false;

        private float m_DistanceFromCamera = 75f;

        private QuarkVrDisplayManager displayManager;

        private void OrientationChanged(float x, float y, float z, float w)
        {
            quaternion = new Quaternion(x, y, z, w);

            hasData = true;
        }

        void Start()
        {
            qvrCameraRig = GameObject.Find("QuarkVRMovingCameraRig");

            mainCam = Camera.main;

            displayManager = new QuarkVrDisplayManager();

            if (qvrCameraRig != null)
            {
                var eyePos = displayManager.GetEyePos(mainCam.transform);
                lefteEyeAnchor = new GameObject("leftEyeAnchor").transform;
                lefteEyeAnchor.localScale = Vector3.one;
                lefteEyeAnchor.localPosition = eyePos[(int)QuarkVrDisplayManager.Eye.Left].position;//Vector3.zero;
                lefteEyeAnchor.localRotation = eyePos[(int)QuarkVrDisplayManager.Eye.Left].orientation;

                leftEyeCamera = lefteEyeAnchor.gameObject.AddComponent<Camera>();
                //Destroy(qvrCameraRig.GetComponent<AudioListener>());
                displayManager.ConfigureCamera(leftEyeCamera, QuarkVrDisplayManager.Eye.Left);

                rightEyeAnchor = new GameObject("rightEyeAnchor").transform;
                rightEyeAnchor.localScale = Vector3.one;
                rightEyeAnchor.localPosition = eyePos[(int)QuarkVrDisplayManager.Eye.Right].position;//Vector3.zero;
                rightEyeAnchor.localRotation = eyePos[(int)QuarkVrDisplayManager.Eye.Right].orientation;
                rightEyeCamera = rightEyeAnchor.gameObject.AddComponent<Camera>();
                displayManager.ConfigureCamera(rightEyeCamera, QuarkVrDisplayManager.Eye.Right);

                CreateTextureAndPassToPlugin();
                isInit = true;
            }
        }

        private void CreateTextureAndPassToPlugin()
        {
            var leftEyeTex = displayManager.GetNativeTex(QuarkVrDisplayManager.Eye.Left);
            var rightEyeTex = displayManager.GetNativeTex(QuarkVrDisplayManager.Eye.Right);
            QuarkVrInit(leftEyeTex, rightEyeTex);
        }

        public void Update()
        {
            var eyePos = displayManager.GetEyePos(mainCam.transform);

            lefteEyeAnchor.localPosition = eyePos[(int)QuarkVrDisplayManager.Eye.Left].position;//Vector3.zero;
            lefteEyeAnchor.localRotation = eyePos[(int)QuarkVrDisplayManager.Eye.Left].orientation;

            rightEyeAnchor.localPosition = eyePos[(int)QuarkVrDisplayManager.Eye.Right].position;//Vector3.zero;
            rightEyeAnchor.localRotation = eyePos[(int)QuarkVrDisplayManager.Eye.Right].orientation;

            mainCam.transform.localRotation = eyePos[(int)QuarkVrDisplayManager.Eye.Left].orientation;
            mainCam.transform.localPosition = (eyePos[(int)QuarkVrDisplayManager.Eye.Left].orientation * Vector3.forward) * m_DistanceFromCamera;
        }

        public void LateUpdate()
        {
            if (!isInit)
            {
                return;
            }
            GL.IssuePluginEvent(GetRenderEventFunc(), 1);
        }

        //public void LateUpdate()
        //{
        //    if (!isInit)
        //    {
        //        return;
        //    }
        //    GL.IssuePluginEvent(GetRenderEventFunc(), 1);

        //    var eyePos = displayManager.GetEyePos(mainCam.transform);

        //    lefteEyeAnchor.localPosition = eyePos[(int)QuarkVrDisplayManager.Eye.Left].position;//Vector3.zero;
        //    lefteEyeAnchor.localRotation = eyePos[(int)QuarkVrDisplayManager.Eye.Left].orientation;

        //    rightEyeAnchor.localPosition = eyePos[(int)QuarkVrDisplayManager.Eye.Right].position;//Vector3.zero;
        //    rightEyeAnchor.localRotation = eyePos[(int)QuarkVrDisplayManager.Eye.Right].orientation;

        //    mainCam.transform.localRotation = eyePos[(int)QuarkVrDisplayManager.Eye.Left].orientation;
        //    mainCam.transform.localPosition = (eyePos[(int)QuarkVrDisplayManager.Eye.Left].orientation * Vector3.forward) * m_DistanceFromCamera;
        //}
    }
}
