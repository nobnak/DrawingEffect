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
        public Camera targetCam;
        public Color clearColor = Color.black;
        public LayerMask layermask;

		RenderTexture _captureRtex;
		RenderTexture _canvasRtex;

        Camera _captureCam;
        Camera _attachedCam;

		void Start() {
			_attachedCam = GetComponent<Camera>();
            _captureCam = new GameObject ("Capture Camera").AddComponent<Camera> ();

            _captureCam.transform.SetParent (transform, false);
            _captureCam.transform.localPosition = Vector3.zero;
            _captureCam.transform.localRotation = Quaternion.identity;
		}
        void OnDestroy() {
            Release();
        }
        void Update() {
            var width = targetCam.pixelWidth;
            var height = targetCam.pixelHeight;

			var tiledWidth = width;
			var tiledHeight = height;
			if (_captureRtex == null || _captureRtex.width != tiledWidth || _captureRtex.height != tiledHeight) {
				Release();
				_captureRtex = new RenderTexture(tiledWidth, tiledHeight, 24, 
                    RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
				_canvasRtex = new RenderTexture(tiledWidth, tiledHeight, 24,
                    RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
				_captureRtex.Create();
				_canvasRtex.Create();

                RenderTexture.active = _canvasRtex;
				GL.Clear(true, true, Color.black);
				RenderTexture.active = null;

                OnCreateTexture.Invoke (_canvasRtex);
			}
            UpdateCameraSettings();
            OnUpdateTexture.Invoke (_canvasRtex);
		}
        void OnRenderImage(RenderTexture src, RenderTexture dst) {
            Graphics.Blit(_captureRtex, _canvasRtex, trackingMat);
            Graphics.Blit (_canvasRtex, dst);
		}

		void Release() {
			Destroy(_captureRtex);
			Destroy(_canvasRtex);
		}

        void UpdateCameraSettings() {
            _attachedCam.transform.localPosition = targetCam.transform.localPosition;
            _attachedCam.transform.localRotation = targetCam.transform.localRotation;

            _attachedCam.clearFlags = CameraClearFlags.Nothing;
            _attachedCam.cullingMask = 0;
            _attachedCam.depth = targetCam.depth - 1;
            _attachedCam.orthographic = targetCam.orthographic;
            _attachedCam.orthographicSize = targetCam.orthographicSize;
            _attachedCam.fieldOfView = targetCam.fieldOfView;

            _captureCam.CopyFrom (_attachedCam);

            _captureCam.clearFlags = CameraClearFlags.SolidColor;
            _captureCam.backgroundColor = clearColor;
            _captureCam.cullingMask = layermask;
            _captureCam.depth = targetCam.depth - 2;
            _captureCam.orthographic = targetCam.orthographic;
            _captureCam.orthographicSize = targetCam.orthographicSize;
            _captureCam.fieldOfView = targetCam.fieldOfView;

            _captureCam.targetTexture = _captureRtex;
        }
	}
}
