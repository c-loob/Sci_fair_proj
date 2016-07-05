using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace QuarkVr.Scripts
{

    public class QuarkVrDisplayManager
    {
        [DllImport("QuarkVrUnityPlugin")]
        private static extern OrientationQuat GetEyePose();

        private const float GearVrFovX = 90.0f;
        private const float GearVrFovY = 90.0f;
        private const int GearVrW = 2560;
        private const int GearVrH = 1440;

        private const float leftEyeOffset = - 0.0320000015f;//-0.0424099192f;
        private const float rightEyeOffset = 0.0320000015f;//0.0211623386f;

        public float virtualTextureScale = 1.0f;

        private EyeRenderDesc[] eyeDescs;
        private RenderTexture[] eyeTextures;
        private IntPtr[] eyeTextureIds;
        private EyePos[] eyePoses;
        private bool trackingInit = false;

        public QuarkVrDisplayManager()
        {
            eyeDescs = new EyeRenderDesc[2]
            {
                CreateEyeRenderDesc(Eye.Left),
                CreateEyeRenderDesc(Eye.Right)
            };

            ConfigureEyeTextures();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct OrientationQuat
        {
            public float x, y, z, w;
            public int changed;
        }

        public struct EyePos
        {
            /// <summary>
            /// The position.
            /// </summary>
            public Vector3 position;

            /// <summary>
            /// The orientation.
            /// </summary>
            public Quaternion orientation;
        }

        public struct EyeRenderDesc
        {
            public Vector2 resolution;
            public Vector2 fov;
        }

        public enum Eye
        {
            Center = -1,
            Left   = 0,
            Right  = 1,
        }

        private EyeRenderDesc CreateEyeRenderDesc(Eye eye)
        {
            return new EyeRenderDesc()
            {
                resolution = new Vector2(GearVrW / 2, GearVrH),
                fov = new Vector2(GearVrFovX, GearVrFovY)
            };
        }

        public EyeRenderDesc GetEyeRenderDesc(Eye eye)
        {
            return eyeDescs[(int)eye];
        }

        private void ConfigureEyePoses(Transform mainCam)
        {
            eyePoses = new EyePos[2];
            var pos = mainCam.position;

            var leftEye = new EyePos
            {
                position = new Vector3(pos.x + leftEyeOffset, pos.y, pos.z),
                orientation = Quaternion.identity
            };

            var rightEye = new EyePos
            {
                position = new Vector3(pos.x + rightEyeOffset, pos.y, pos.z),
                orientation = Quaternion.identity
            };

            eyePoses[0] = leftEye;
            eyePoses[1] = rightEye;
        }

        public EyePos[] GetEyePos(Transform mainCam)
        {
            if(!trackingInit)
            {
                ConfigureEyePoses(mainCam);
                trackingInit = true;

                return eyePoses;
            }
            var eyePos = GetEyePose();

            if(eyePos.changed == 1)
            {
                var pos = mainCam.position;
                eyePoses[0].orientation = new Quaternion(-eyePos.x, -eyePos.y, eyePos.z, eyePos.w);
                eyePoses[0].position = new Vector3(pos.x + leftEyeOffset, pos.y, pos.z);
                eyePoses[1].orientation = new Quaternion(-eyePos.x, -eyePos.y, eyePos.z, eyePos.w);
                eyePoses[1].position = new Vector3(pos.x + rightEyeOffset, pos.y, pos.z);
            }

            return eyePoses;
        }

        private void ConfigureEyeTextures()
        {
            eyeTextures = new RenderTexture[2];
            eyeTextureIds = new IntPtr[2];

            foreach (var eye in new Eye[] { Eye.Left, Eye.Right})
            {
                int eyeIndex = (int)eye;
                EyeRenderDesc eyeDesc = eyeDescs[(int)eye];

                eyeTextures[eyeIndex] = new RenderTexture(
                    (int)eyeDesc.resolution.x,
                    (int)eyeDesc.resolution.y,
                    24,
                    RenderTextureFormat.ARGB32);

                eyeTextures[eyeIndex].antiAliasing = 2;

                eyeTextures[eyeIndex].Create();
                eyeTextureIds[eyeIndex] = eyeTextures[eyeIndex].GetNativeTexturePtr();
            }

        }

        public IntPtr GetNativeTex(Eye eye)
        {
            return eyeTextureIds[(int)eye];
        }

        public RenderTexture GetEyeTexture(Eye eye)
        {
            return eyeTextures[(int)eye];
        }

        public void ConfigureCamera(Camera cam, Eye eye)
        {
            var desc = GetEyeRenderDesc(eye);
            cam.fieldOfView = desc.fov.y;
            cam.aspect = desc.resolution.x / desc.resolution.y;
            cam.targetTexture = GetEyeTexture(eye);

            float vs = virtualTextureScale;
            cam.rect = new Rect(0f, 1f - vs, vs, vs);
        }

    }
}
