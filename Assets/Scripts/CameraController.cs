using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform _planet;

    private Transform _transform;

    private void Start()
    {
        _transform = transform;
    }

    private void Update()
    {
        _transform.rotation = Quaternion.Euler(0f, 0f, Vector2.Angle(Vector2.up, _transform.position - _planet.position));
    }
}
