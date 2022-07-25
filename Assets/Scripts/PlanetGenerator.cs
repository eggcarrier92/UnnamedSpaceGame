using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlanetGenerator : MonoBehaviour
{
    [SerializeField] private float _planetRadius = 100f;
    [SerializeField] private float _angularDistanceBetweenPoints = 1f;
    [SerializeField] private float _maxHillHeight = 10f;
    [SerializeField] private float _tangentScale = 1f;
    [SerializeField, Range(0f, 1f)] private float _perlinNoiseScale = .5f;
    [SerializeField] private float _perlinNoiseOffset = 0f;

    private SpriteShapeController _shape;

    private void Start()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _shape = GetComponent<SpriteShapeController>();
    }

    public void Generate()
    {
        InitializeComponents();
        ResetTerrain();
    }
    private void ResetTerrain()
    {
        _shape.spline.isOpenEnded = true;

        DeletePoints(2);

        _shape.spline.SetPosition(0, new Vector3(0, 0));
        _shape.spline.SetLeftTangent(0, new Vector3(-1, 0) * _tangentScale);
        _shape.spline.SetRightTangent(0, new Vector3(1, 0) * _tangentScale);

        _shape.spline.SetPosition(1, new Vector3(1, 0));
        _shape.spline.SetLeftTangent(1, new Vector3(-1, 0) * _tangentScale);
        _shape.spline.SetRightTangent(1, new Vector3(1, 0) * _tangentScale);
    }
    private void DeletePoints(int howManyPointsToLeave) 
    {
        if (_shape.spline.GetPointCount() > 0)
            for (int i = _shape.spline.GetPointCount() - 1; i > howManyPointsToLeave - 1; i--)
                _shape.spline.RemovePointAt(i);
    }
}
