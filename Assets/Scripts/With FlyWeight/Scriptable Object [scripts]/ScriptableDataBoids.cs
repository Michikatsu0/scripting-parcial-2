using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ScriptableDataBoids", order = 1)]
public class ScriptableDataBoids : ScriptableObject
{
    public float _minSpeed = 2;
    public float _maxSpeed = 5;
    public float _perceptionRadius = 2.5f;
    public float _avoidanceRadius = 1;
    public float _maxSteerForce = 3;

    public float _alignWeight = 1;
    public float _cohesionWeight = 1;
    public float _seperateWeight = 1;

    public float _targetWeight = 1;

    public LayerMask _obstacleMask;
    public float _boundsRadius = .27f;
    public float _avoidCollisionWeight = 10;
    public float _collisionAvoidDst = 5;
}
