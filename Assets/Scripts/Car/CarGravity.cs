using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarGravity : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _carRigidbody;
    [SerializeField] private GameObject _attractor;
    [SerializeField] private float _gravityAcceleration = 9.81f;

    private List<Rigidbody2D> _allRigidbodies;
    private float _globalMass;

    private Vector2 _gravityForce;

    private Vector2 _previousVelocity;
    private float _gForce;
    private float _maxGForce;

    private void Awake()
    {
        _allRigidbodies = GetComponentsInChildren<Rigidbody2D>().ToList();
    }
    private void Start()
    {
        InvokeRepeating(nameof(ShowGForce), 0f, .5f);
    }

    private void FixedUpdate()
    {
        foreach (var rigidbody in _allRigidbodies)
        {
            Vector2 directionToBody = (Vector2)_attractor.transform.position - rigidbody.position;
            Vector2 force = rigidbody.mass * _gravityAcceleration * directionToBody.normalized;
            rigidbody.AddForce(force);
            _gravityForce += force;
            _globalMass += rigidbody.mass;
        }
        CalculateGForce();
        _gravityForce = Vector2.zero;
        _globalMass = 0;
    }

    private void CalculateGForce()
    {
        //Debug.Log((_gravityForce / _globalMass).magnitude);
        _gForce = (_carRigidbody.velocity - _gravityForce / _globalMass * Time.deltaTime - _previousVelocity).magnitude / 9.81f / Time.deltaTime;
        _previousVelocity = _carRigidbody.velocity;
        _maxGForce = _gForce > _maxGForce ? _gForce : _maxGForce;
    }

    private void ShowGForce()
    {
        Debug.Log($"G-Force: {Math.Round(_maxGForce, 2)}G");
        _maxGForce = 0;
    }
}
