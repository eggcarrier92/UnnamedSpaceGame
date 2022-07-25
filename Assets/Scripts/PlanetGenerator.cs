using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class PlanetGenerator : MonoBehaviour
{
    [SerializeField] private float _planetRadius = 100f;
    [SerializeField, Tooltip("This value will be rounded to the closest value that 360 is entirely divisible by")] 
    private float _angularDistanceBetweenPoints = 1f;
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

        // Change the values so that 360 degrees is entirely divisible by the angular distance between points
        int amountOfPoints = Mathf.RoundToInt(360f / _angularDistanceBetweenPoints);
        float angularDistanceBetweenPoints = 360f / amountOfPoints;
        amountOfPoints = Mathf.RoundToInt(360f / angularDistanceBetweenPoints);
        angularDistanceBetweenPoints = 360f / amountOfPoints;

        Debug.Log($"Amount of points: {amountOfPoints}, angular distance between points: {angularDistanceBetweenPoints}");
        //for (int i = 0; i < )
    }
    private void ResetTerrain()
    {
        _shape.spline.isOpenEnded = true;

        LeavePoints(2);

        _shape.spline.SetPosition(0, new Vector3(0, 0));
        _shape.spline.SetLeftTangent(0, new Vector3(-1, 0) * _tangentScale);
        _shape.spline.SetRightTangent(0, new Vector3(1, 0) * _tangentScale);

        _shape.spline.SetPosition(1, new Vector3(1, 0));
        _shape.spline.SetLeftTangent(1, new Vector3(-1, 0) * _tangentScale);
        _shape.spline.SetRightTangent(1, new Vector3(1, 0) * _tangentScale);
    }
    private void LeavePoints(int howManyPointsToLeave) 
    {
        if (_shape.spline.GetPointCount() > 0)
            for (int i = _shape.spline.GetPointCount() - 1; i > howManyPointsToLeave - 1; i--)
                _shape.spline.RemovePointAt(i);
    }
}
