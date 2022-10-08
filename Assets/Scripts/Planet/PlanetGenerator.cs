using UnityEngine;
using UnityEngine.U2D;

public class PlanetGenerator : MonoBehaviour
{
    [SerializeField] private float _planetRadius = 100f;

    [SerializeField, Tooltip("The angular distance between the points of the Sprite Shape. This value will be rounded to the closest value that 360 is entirely divisible by.")] 
    private float _angularDistanceBetweenPoints = 1f;

    [SerializeField, Tooltip("Scale of each tangent of each point of the Sprite Shape.")] 
    private float _tangentScale = 1f;

    [SerializeField, Tooltip("Size of one chunk in meters. This value will be rounded to the closest value that 360 is entirely divisible by.")]
    private float _chunkSize = 100f;

    [SerializeField] private float _maxHillHeight = 10f;
    [SerializeField, Range(0f, 1f)] private float _perlinNoiseScale = .5f;
    [SerializeField] private float _perlinNoiseOffset = 0f;
    [SerializeField] private bool _isOpenEnded = false;

    private SpriteShapeController _shape;


    public void Generate()
    {
        InitializeComponents();
        ResetTerrain();

        // Change the values so that 360 degrees is entirely divisible by the angular distance between points
        int amountOfPoints = Mathf.RoundToInt(360f / _angularDistanceBetweenPoints);
        float angularDistanceBetweenPoints = 360f / amountOfPoints;
        amountOfPoints = Mathf.RoundToInt(360f / angularDistanceBetweenPoints);
        angularDistanceBetweenPoints = 360f / amountOfPoints;

        //Debug.Log($"Amount of points: {amountOfPoints}, angular distance between points: {angularDistanceBetweenPoints}");

        // Set points
        for (int i = 0; i < amountOfPoints; i++)
        {
            float distanceToPlanetCenter = 
                _planetRadius + _maxHillHeight * Mathf.PerlinNoise(i * _perlinNoiseScale + _perlinNoiseOffset, 0);
            Vector3 pointCoordinates = new(
                distanceToPlanetCenter * Mathf.Sin(angularDistanceBetweenPoints * i * Mathf.Deg2Rad),
                distanceToPlanetCenter * Mathf.Cos(angularDistanceBetweenPoints * i * Mathf.Deg2Rad));
            if (i > _shape.spline.GetPointCount() - 1)
                _shape.spline.InsertPointAt(i, pointCoordinates);
            else
                _shape.spline.SetPosition(i, pointCoordinates);
        }
        // Set tangents
        for (int i = 0; i < _shape.spline.GetPointCount(); i++)
        {
            int previousPointIndex = i - 1;
            if (i - 1 < 0)
                previousPointIndex = _shape.spline.GetPointCount() - 1;
            int nextPointIndex = i + 1;
            if (i + 1 > _shape.spline.GetPointCount() - 1)
                nextPointIndex = 0;

            Vector3 directionToPreviousPoint = (_shape.spline.GetPosition(previousPointIndex) - _shape.spline.GetPosition(i)).normalized;
            Vector3 directionToNextPoint = (_shape.spline.GetPosition(nextPointIndex) - _shape.spline.GetPosition(i)).normalized;
            Vector3 average = (directionToPreviousPoint - directionToNextPoint) / 2;
            average = average.normalized * _tangentScale;

            _shape.spline.SetTangentMode(i, ShapeTangentMode.Continuous);
            _shape.spline.SetLeftTangent(i, average);
            _shape.spline.SetRightTangent(i, -average);
        }
    }
    private void InitializeComponents()
    {
        _shape = GetComponent<SpriteShapeController>();
    }
    private void ResetTerrain()
    {
        _shape.spline.isOpenEnded = _isOpenEnded;

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
