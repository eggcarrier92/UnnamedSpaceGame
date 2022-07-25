using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarGravity : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _carRigidbody;
    [SerializeField] private GameObject _attractor;
    [SerializeField] private float _gravitationalConstant = 1f;
    [SerializeField] private float _attractorMass = 1000f;

    private List<Rigidbody2D> _allRigidbodies;

    private void Awake()
    {
        _allRigidbodies = GetComponentsInChildren<Rigidbody2D>().ToList();
    }

    private void FixedUpdate()
    {
        foreach (var rigidbody in _allRigidbodies)
        {
            Vector2 directionToBody = (Vector2)_attractor.transform.position - rigidbody.position;
            rigidbody.AddForce(
            rigidbody.mass * _attractorMass * _gravitationalConstant / directionToBody.sqrMagnitude * directionToBody.normalized);
        }
    }
}
