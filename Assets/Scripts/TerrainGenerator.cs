using UnityEngine;
using UnityEngine.U2D;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] private int _amountOfPoints = 150;
    [SerializeField] private float _distanceBetweenPoints = 3f;
    [SerializeField] private float _maxHeight = 5f;
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
        _shape.spline.SetPosition(2, _shape.spline.GetPosition(2) + _distanceBetweenPoints * (_amountOfPoints + 1) * Vector3.right);
        _shape.spline.SetPosition(3, _shape.spline.GetPosition(3) + _distanceBetweenPoints * (_amountOfPoints + 1) * Vector3.right);

        // Generate points
        for (int i = 0; i < _amountOfPoints; i++)
        {
            float xPos = _shape.spline.GetPosition(i + 1).x + _distanceBetweenPoints;
            _shape.spline.InsertPointAt(i + 2, new Vector3(xPos, _maxHeight * Mathf.PerlinNoise(i * _perlinNoiseScale + _perlinNoiseOffset, 0)));
        }

        // Set tangents
        for (int i = 0; i < _amountOfPoints; i++)
        {
            Vector3 directionToPreviousPoint = (_shape.spline.GetPosition(i + 1) - _shape.spline.GetPosition(i + 2)).normalized;
            Vector3 directionToNextPoint = (_shape.spline.GetPosition(i + 3) - _shape.spline.GetPosition(i + 2)).normalized;
            Vector3 average = (directionToPreviousPoint - directionToNextPoint) / 2;
            average = average.normalized * _tangentScale;

            _shape.spline.SetTangentMode(i + 2, ShapeTangentMode.Continuous);
            _shape.spline.SetLeftTangent(i + 2, average);
            _shape.spline.SetRightTangent(i + 2, -average);
        }
    }
    private void ResetTerrain()
    {
        if (_shape.spline.GetPointCount() > 0)
            for (int i = _shape.spline.GetPointCount() - 1; i > 3; i--)
                _shape.spline.RemovePointAt(i);

        _shape.spline.SetPosition(0, new Vector3(-1, -1));
        _shape.spline.SetLeftTangent (0, new Vector3(1, 0) * _tangentScale);
        _shape.spline.SetRightTangent(0, new Vector3(-1, 0) * _tangentScale);

        _shape.spline.SetPosition(1, new Vector3(-1,  1));
        _shape.spline.SetLeftTangent (1, new Vector3(-1, 0) * _tangentScale);
        _shape.spline.SetRightTangent(1, new Vector3(1, 0) * _tangentScale);

        _shape.spline.SetPosition(2, new Vector3(1,  1));
        _shape.spline.SetLeftTangent (2, new Vector3(-1, 0) * _tangentScale);
        _shape.spline.SetRightTangent(2, new Vector3(1, 0) * _tangentScale);

        _shape.spline.SetPosition(3, new Vector3(1, -1));
        _shape.spline.SetLeftTangent (3, new Vector3(1, 0) * _tangentScale);
        _shape.spline.SetRightTangent(3, new Vector3(-1, 0) * _tangentScale);

        _shape.spline.isOpenEnded = false;
    }
}
