using UnityEngine;

public class BoidManager : MonoBehaviour
{
    const int _threadGroupSize = 1024;

    public ScriptableDataBoids _settings;
    public ComputeShader _compute;
    Boid[] _boids;

    void Start()
    {
        _boids = FindObjectsOfType<Boid>();
        foreach (Boid b in _boids)
        {
            b.Initialize(_settings, null);
        }

    }

    void Update()
    {
        if (_boids != null)
        {

            int numBoids = _boids.Length;
            var boidData = new BoidData[numBoids];

            for (int i = 0; i < _boids.Length; i++)
            {
                boidData[i].position = _boids[i]._position;
                boidData[i].direction = _boids[i]._forward;
            }

            var boidBuffer = new ComputeBuffer(numBoids, BoidData.Size);
            boidBuffer.SetData(boidData);

            _compute.SetBuffer(0, "boids", boidBuffer);
            _compute.SetInt("numBoids", _boids.Length);
            _compute.SetFloat("viewRadius", 2.5f);
            _compute.SetFloat("avoidRadius", 1);

            int threadGroups = Mathf.CeilToInt(numBoids / (float)_threadGroupSize);
            _compute.Dispatch(0, threadGroups, 1, 1);

            boidBuffer.GetData(boidData);

            for (int i = 0; i < _boids.Length; i++)
            {
                _boids[i]._avgFlockHeading = boidData[i].flockHeading;
                _boids[i]._centreOfFlockmates = boidData[i].flockCentre;
                _boids[i]._avgAvoidanceHeading = boidData[i].avoidanceHeading;
                _boids[i]._numPerceivedFlockmates = boidData[i].numFlockmates;

                _boids[i].UpdateBoid();
            }

            boidBuffer.Release();
        }
    }

    public struct BoidData
    {
        public Vector3 position;
        public Vector3 direction;

        public Vector3 flockHeading;
        public Vector3 flockCentre;
        public Vector3 avoidanceHeading;
        public int numFlockmates;

        public static int Size
        {
            get
            {
                return sizeof(float) * 3 * 5 + sizeof(int);
            }
        }
    }
}
