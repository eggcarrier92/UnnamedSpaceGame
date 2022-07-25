using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _carRigidbody;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private KeyCode _accelerateKey = KeyCode.D;
    [SerializeField] private KeyCode _brakeKey = KeyCode.A;
    [SerializeField] private float _accelerationForce;
    [SerializeField] private float _brakeForce;
    [SerializeField] private Vector2 _forcePosition = new(0f, -0.5f);

    private Transform _carTransform;
    private Collider2D[] _allColliders;
    private Vector2 _localCenterOfMass;

    private bool IsGrounded
    {
        get 
        {
            List<Collider2D> allContacts = new();
            foreach (var collider in _allColliders)
            {
                List<Collider2D> contacts = new();
                collider.GetContacts(contacts);
                allContacts.AddRange(contacts);
            }
            foreach(var contact in allContacts)
            {
                if (_whatIsGround == (_whatIsGround | (1 << contact.gameObject.layer)))
                    return true;
            }
            return false;
        }
    }

    private void Start()
    {
        _carTransform = _carRigidbody.transform;
        _allColliders = GetComponentsInChildren<Collider2D>();
    }
    private void Awake()
    {
        CalculateCenterOfMass();
    }

    private void CalculateCenterOfMass()
    {
        _localCenterOfMass = Vector2.zero;
        Vector2 centerOfMass = Vector2.zero;
        float totalMass = 0f;
        foreach (var rigidbody in GetComponentsInChildren<Rigidbody2D>().ToList())
        {
            centerOfMass += ((Vector2)rigidbody.transform.localPosition + rigidbody.centerOfMass) * rigidbody.mass;
            totalMass += rigidbody.mass;
        }
        centerOfMass /= totalMass;
        _localCenterOfMass = centerOfMass;
    }

    private void FixedUpdate()
    {
        ControlCar();
    }

    private void ControlCar()
    {
        Vector2 forcePosition = _carTransform.TransformPoint(_localCenterOfMass);
        if (!IsGrounded)
            return;
        if (Input.GetKey(_accelerateKey))
            _carRigidbody.AddForceAtPosition(_carTransform.right * _accelerationForce, forcePosition);
        if (Input.GetKey(_brakeKey))
            _carRigidbody.AddForceAtPosition(-_carTransform.right * _brakeForce, forcePosition);
    }
}
