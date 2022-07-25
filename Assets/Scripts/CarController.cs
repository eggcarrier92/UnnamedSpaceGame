using System.Collections;
using System.Collections.Generic;
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

    private void FixedUpdate()
    {
        if (!IsGrounded)
            return;
        if (Input.GetKey(_accelerateKey))
            _carRigidbody.AddForceAtPosition(_carTransform.right * _accelerationForce, _forcePosition + (Vector2)_carTransform.position);
        if (Input.GetKey(_brakeKey))
            _carRigidbody.AddForceAtPosition(-_carTransform.right * _brakeForce, _forcePosition + (Vector2)_carTransform.position);
    }
}
