using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class PlanetGenerator : MonoBehaviour
{
    [Header("Generation")]

    [SerializeField] 
    private float _planetRadius = 100f;
    [SerializeField, Tooltip("The distance between the points of the Sprite Shape. This value will be rounded to the closest value that the planet's circumference is entirely divisible by.")] 
    private float _distanceBetweenPoints = 20f;
    [SerializeField, Tooltip("Scale of each tangent of each point of the Sprite Shape.")] 
    private float _tangentScale = 1f;
    [SerializeField] 
    private float _maxHillHeight = 10f;
    [SerializeField, Range(0f, 1f)] 
    private float _perlinNoiseScale = .5f;
    [SerializeField] 
    private float _perlinNoiseOffset = 0f;

    /*-------------------------------------------------------------*/
    [Header("Chunk options")]

    [SerializeField]
    private GameObject _chunkPrefab;
    [SerializeField, Tooltip("Size of one chunk in meters. This value will be rounded to the closest value that the planet's circumference is entirely divisible by.")]
    private float _chunkSize = 1000f;
    [SerializeField] 
    private bool _chunkHasAPointInTheCenter = true;

    /*-------------------------------------------------------------*/

    private List<SpriteShapeController> _chunks;

    public void Generate()
    {
        if (_chunkSize < 2 * _distanceBetweenPoints)
        {
            Debug.LogError("Chunk size must be at least 2 times higher than distance between points");
            return;
        }
        InitializeComponents();
        //ResetTerrain();

        float planetCircumference = 2 * Mathf.PI * _planetRadius;
        float angularDistanceBetweenPoints = 360f * _distanceBetweenPoints / planetCircumference;
        float angularSizeOfChunk = 360f * _chunkSize / planetCircumference;

        int amountOfPoints = RoundToClosestEntireDivisor(360f, ref angularDistanceBetweenPoints);
        int amountOfChunks = RoundToClosestEntireDivisor(360f, ref angularSizeOfChunk);

        //Debug.Log($"Amount of chunks: {amountOfChunks}, angular size of a chunk: {angularSizeOfChunk}");
        //Debug.Log($"Amount of points: {amountOfPoints}, angular distance between points: {angularDistanceBetweenPoints}");

        foreach (var chunk in _chunks)
            DestroyImmediate(chunk.gameObject);
        _chunks = new();

        for (int chunkIndex = 0; chunkIndex < amountOfChunks; chunkIndex++)
            _chunks.Add(Instantiate(_chunkPrefab, transform).GetComponent<SpriteShapeController>());

        // Set points
        for (int pointIndex = 0; pointIndex < amountOfPoints; pointIndex++)
        {
            float distanceToPlanetCenter =
                _planetRadius + _maxHillHeight * Mathf.PerlinNoise(pointIndex * _perlinNoiseScale + _perlinNoiseOffset, 0);
            Vector3 pointCoordinates = new(
                distanceToPlanetCenter * Mathf.Sin(angularDistanceBetweenPoints * pointIndex * Mathf.Deg2Rad),
                distanceToPlanetCenter * Mathf.Cos(angularDistanceBetweenPoints * pointIndex * Mathf.Deg2Rad));
            int chunkIndex = GetChunk(pointIndex, angularSizeOfChunk, angularDistanceBetweenPoints);
            int pointIndexInChunk = GetPointIndexInChunk(pointIndex, angularSizeOfChunk, angularDistanceBetweenPoints);
            if (pointIndexInChunk > _chunks[chunkIndex].spline.GetPointCount() - 1)
                _chunks[chunkIndex].spline.InsertPointAt(pointIndexInChunk, pointCoordinates);
            else
                _chunks[chunkIndex].spline.SetPosition(pointIndexInChunk, pointCoordinates);
        }
        //Set tangents
        for (int pointIndex = 0; pointIndex < amountOfPoints; pointIndex++)
        {
            Debug.Log($"Processing {pointIndex}");

            // Current point
            int chunkIndex = GetChunk(pointIndex, angularSizeOfChunk, angularDistanceBetweenPoints);
            int pointIndexInChunk = GetPointIndexInChunk(
                pointIndex, angularSizeOfChunk, angularDistanceBetweenPoints);

            bool firstPointInChunk = pointIndexInChunk == 0;

            int previousPointIndex = pointIndex == 0 ? amountOfPoints - 2 : pointIndex - 1;
            int previousPointChunk = GetChunk(previousPointIndex, angularSizeOfChunk, angularDistanceBetweenPoints);
            int previousPointIndexInChunk = GetPointIndexInChunk(previousPointIndex, angularSizeOfChunk, angularDistanceBetweenPoints);

            int nextPointIndex = pointIndex == amountOfPoints - 1 ? 1 : pointIndex + 1;
            int nextPointChunk = GetChunk(nextPointIndex, angularSizeOfChunk, angularDistanceBetweenPoints);
            int nextPointIndexInChunk = GetPointIndexInChunk(nextPointIndex, angularSizeOfChunk, angularDistanceBetweenPoints);

            Vector3 pointPosition = _chunks[chunkIndex].spline.GetPosition(pointIndexInChunk);
            Vector3 previousPointPosition = _chunks[previousPointChunk].spline.GetPosition(previousPointIndexInChunk);
            Vector3 nextPointPosition = _chunks[nextPointChunk].spline.GetPosition(nextPointIndexInChunk);

            Vector3 directionToPreviousPoint = (previousPointPosition - pointPosition).normalized;
            Vector3 directionToNextPoint = (nextPointPosition - pointPosition).normalized;
            Vector3 tangentValue = (directionToPreviousPoint - directionToNextPoint) / 2;
            tangentValue = tangentValue.normalized * _tangentScale;
            Vector3 leftTangent = firstPointInChunk ? Vector3.zero : tangentValue;
            Vector3 rightTangent = -tangentValue;

            _chunks[chunkIndex].spline.SetTangentMode(pointIndexInChunk, ShapeTangentMode.Broken);
            _chunks[chunkIndex].spline.SetLeftTangent(pointIndexInChunk, leftTangent);
            _chunks[chunkIndex].spline.SetRightTangent(pointIndexInChunk, rightTangent);
        }
        // Fill in the gaps
        for (int chunkIndex = 0; chunkIndex < _chunks.Count; chunkIndex++)
        {
            int nextChunk = chunkIndex + 1 < _chunks.Count ? chunkIndex + 1 : 0;
            int pointIndex = _chunks[chunkIndex].spline.GetPointCount();
            _chunks[chunkIndex].spline.InsertPointAt(pointIndex, _chunks[nextChunk].spline.GetPosition(0));
            _chunks[chunkIndex].spline.SetTangentMode(pointIndex, ShapeTangentMode.Broken);
            _chunks[chunkIndex].spline.SetLeftTangent(pointIndex, -_chunks[nextChunk].spline.GetRightTangent(0));
        }
        // Add a point in the center
        if (_chunkHasAPointInTheCenter)
            foreach (var chunk in _chunks)
                chunk.spline.InsertPointAt(chunk.spline.GetPointCount(), Vector3.zero);
        Debug.Log("finished");
    }

    private int RoundToClosestEntireDivisor(float dividend, ref float divisor)
    {
        int quotient = Mathf.RoundToInt(dividend / divisor);
        divisor = dividend / quotient;
        quotient = Mathf.RoundToInt(dividend / divisor);
        divisor = dividend / quotient;
        return quotient;
    }

    private void InitializeComponents()
    {
        //_shape = GetComponent<SpriteShapeController>();
        _chunks = GetComponentsInChildren<SpriteShapeController>().ToList();
    }
    private int GetChunk(int pointIndex, float chunkSize, float distanceBetweenPoints)
    {
        return Mathf.FloorToInt(distanceBetweenPoints / chunkSize * pointIndex);
    }
    private int GetPointIndexInChunk(int pointIndex, float chunkSize, float distanceBetweenPoints)
    {
        int chunkIndex = GetChunk(pointIndex, chunkSize, distanceBetweenPoints);
        return Mathf.FloorToInt(pointIndex - chunkIndex * chunkSize / distanceBetweenPoints);
    }
    private static int GetPointIndexInChunk(int pointIndex, float chunkSize, float distanceBetweenPoints, int chunkIndex)
    {
        return Mathf.FloorToInt(pointIndex - chunkIndex * chunkSize / distanceBetweenPoints);
    }
}
