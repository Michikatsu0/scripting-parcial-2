using UnityEngine;

public class Boid : MonoBehaviour
{

    //Con Flyweight
    ScriptableDataBoids _settings;

    // State
    [HideInInspector]
    public Vector3 _position;
    [HideInInspector]
    public Vector3 _forward;
    Vector3 _velocity;

    // To update:
    Vector3 _acceleration;
    [HideInInspector]
    public Vector3 _avgFlockHeading;
    [HideInInspector]
    public Vector3 _avgAvoidanceHeading;
    [HideInInspector]
    public Vector3 _centreOfFlockmates;
    [HideInInspector]
    public int _numPerceivedFlockmates;

    // Cached
    Material _material;
    Transform _cachedTransform;
    Transform _target;

    void Awake()
    {
        _material = transform.GetComponentInChildren<MeshRenderer>().material;
        _cachedTransform = transform;
    }

    public void Initialize(ScriptableDataBoids settings, Transform target)
    {
        this._target = target;
        this._settings = settings;

        _position = _cachedTransform.position;
        _forward = _cachedTransform.forward;

        float startSpeed = (settings._minSpeed + settings._maxSpeed) / 2;
        _velocity = transform.forward * startSpeed;
    }

    public void SetColour(Color col)
    {
        if (_material != null)
        {
            _material.color = col;
        }
    }

    public void UpdateBoid()
    {
        Vector3 acceleration = Vector3.zero;

        if (_target != null)
        {
            Vector3 offsetToTarget = (_target.position - _position);
            acceleration = SteerTowards(offsetToTarget) * _settings._targetWeight;
        }

        if (_numPerceivedFlockmates != 0)
        {
            _centreOfFlockmates /= _numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCentre = (_centreOfFlockmates - _position);

            var alignmentForce = SteerTowards(_avgFlockHeading) * _settings._alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCentre) * _settings._cohesionWeight;
            var seperationForce = SteerTowards(_avgAvoidanceHeading) * _settings._seperateWeight;

            acceleration += alignmentForce;
            acceleration += cohesionForce;
            acceleration += seperationForce;
        }

        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * _settings._avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        _velocity += acceleration * Time.deltaTime;
        float speed = _velocity.magnitude;
        Vector3 dir = _velocity / speed;
        speed = Mathf.Clamp(speed, _settings._minSpeed, _settings._maxSpeed);
        _velocity = dir * speed;

        _cachedTransform.position += _velocity * Time.deltaTime;
        _cachedTransform.forward = dir;
        _position = _cachedTransform.position;
        _forward = dir;
    }

    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(_position, _settings._boundsRadius, _forward, out hit, _settings._collisionAvoidDst, _settings._obstacleMask))
        {
            return true;
        }
        else { }
        return false;
    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = _cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(_position, dir);
            if (!Physics.SphereCast(ray, _settings._boundsRadius, _settings._collisionAvoidDst, _settings._obstacleMask))
            {
                return dir;
            }
        }

        return _forward;
    }

    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * _settings._maxSpeed - _velocity;
        return Vector3.ClampMagnitude(v, _settings._maxSteerForce);
    }

}
