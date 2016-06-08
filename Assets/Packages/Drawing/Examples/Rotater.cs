using UnityEngine;
using System.Collections;

namespace DrawingSystem {

    public class Rotater : MonoBehaviour {
        public const float SEED_RANGE = 1000f;

        public float freq = 0.1f;
        public float speed = 2f;

        Vector3 _seed;

        void Start() {
            _seed = SEED_RANGE * new Vector3 (Random.value, Random.value, Random.value);
        }
    	void Update () {
            var t = Time.timeSinceLevelLoad * freq;
            var dt = Time.deltaTime * speed;
            transform.localRotation *= Quaternion.Euler (
                dt * Noise (t + _seed.x, _seed.y),
                dt * Noise (t + _seed.y, _seed.z),
                dt * Noise (t + _seed.z, _seed.x));
    	}

        float Noise(float x, float y) {
            return 2f * Mathf.PerlinNoise (x, y) - 1f;
        }
    }
}