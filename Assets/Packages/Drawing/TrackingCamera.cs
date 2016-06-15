using UnityEngine;
using System.Collections;

namespace DrawingSystem {

	[RequireComponent(typeof(Camera))]
	public class TrackingCamera : MonoBehaviour {
        public const string KEY_COLORSPACE_LINEAR = "LINEAR_SPACE";
        public const string KEY_COLORSPACE_GAMMA = "GAMMA_SPACE";

        public TextureEvent OnCreateTexture;
        public TextureEvent OnUpdateTexture;

		public Material trackingMat;
        public Color clearColor = Color.black;
        public LayerMask layermask;
        public Camera[] renderCams;

		RenderTexture _captureRtex;
		RenderTexture _canvasRtex;

        Camera _attachedCam;

		void Start() {
			_attachedCam = GetComponent<Camera>();
		}
        void OnDestroy() {
            Release();
        }
        void Update() {
            var width = Screen.width;
            var height = Screen.height;
            var targetTex = _attachedCam.targetTexture;
            if (targetTex != null) {
                width = targetTex.width;
                height = targetTex.height;
            }
			if (_captureRtex == null || _captureRtex.width != width || _captureRtex.height != height) {
                Debug.LogFormat ("Capture tex size {0}x{1}", width, height);

				Release();
				_captureRtex = new RenderTexture(width, height, 24, 
                    RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
				_canvasRtex = new RenderTexture(width, height, 24,
                    RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
				_captureRtex.Create();
				_canvasRtex.Create();

                RenderTexture.active = _canvasRtex;
				GL.Clear(true, true, Color.black);
				RenderTexture.active = null;

                OnCreateTexture.Invoke (_canvasRtex);
			}
            UpdateCameraSettings();

            Capture ();

            OnUpdateTexture.Invoke (_canvasRtex);
		}
        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            Graphics.Blit(_captureRtex, _canvasRtex, trackingMat);
            Graphics.Blit (_canvasRtex, dst);
            Debug.LogFormat ("Src size {0}x{1}", src.width, src.height);
		}

		void Release() {
			Destroy(_captureRtex);
			Destroy(_canvasRtex);
		}

        void Capture () {
            for (var i = 0; i < renderCams.Length; i++) {
                var targetCam = renderCams [i];
                if (!targetCam.isActiveAndEnabled)
                    continue;
                
                var camset = new CameraSettings (targetCam);
                targetCam.clearFlags = CameraClearFlags.SolidColor;
                targetCam.backgroundColor = clearColor;
                targetCam.cullingMask = layermask.value;
                targetCam.targetTexture = _captureRtex;
                targetCam.Render ();
                camset.Load ();
            }
        }
        void UpdateCameraSettings() {
            _attachedCam.clearFlags = CameraClearFlags.Nothing;
            _attachedCam.cullingMask = 0;
        }

        public struct CameraSettings {
            public Camera targetCam;
            public CameraClearFlags clearFlags;
            public Color backgroundColor;
            public int cullingMask;
            public RenderTexture targetTexture;

            public CameraSettings(Camera targetCam) {
                this.targetCam = targetCam;
                this.clearFlags = targetCam.clearFlags;
                this.backgroundColor = targetCam.backgroundColor;
                this.cullingMask = targetCam.cullingMask;
                this.targetTexture = targetCam.targetTexture;
            }
            public void Load() {
                targetCam.clearFlags = clearFlags;
                targetCam.backgroundColor = backgroundColor;
                targetCam.cullingMask = cullingMask;
                targetCam.targetTexture = targetTexture;     
            }
        }
	}
}
