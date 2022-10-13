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
    [SerializeField] 
    private bool _isOpenEnded = false;

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

        for (int i = 0; i < amountOfChunks; i++)
            _chunks.Add(Instantiate(_chunkPrefab, transform).GetComponent<SpriteShapeController>());

        // Set points
        for (int i = 0; i < amountOfPoints; i++)
        {
            float distanceToPlanetCenter =
                _planetRadius + _maxHillHeight * Mathf.PerlinNoise(i * _perlinNoiseScale + _perlinNoiseOffset, 0);
            Vector3 pointCoordinates = new(
                distanceToPlanetCenter * Mathf.Sin(angularDistanceBetweenPoints * i * Mathf.Deg2Rad),
                distanceToPlanetCenter * Mathf.Cos(angularDistanceBetweenPoints * i * Mathf.Deg2Rad));
            int chunkIndex = Mathf.FloorToInt(angularDistanceBetweenPoints / angularSizeOfChunk * i);
            int pointIndexInChunk = Mathf.FloorToInt(i - chunkIndex * angularSizeOfChunk / angularDistanceBetweenPoints);
            //Debug.Log(i - chunkIndex * (angularSizeOfChunk / angularDistanceBetweenPoints - (angularSizeOfChunk / angularDistanceBetweenPoints - Mathf.Floor(angularSizeOfChunk / angularDistanceBetweenPoints))));
            //Debug.Log($"i:{i}, chunk index: {chunkIndex}, as:{angularSizeOfChunk}, ad:{angularDistanceBetweenPoints}, as/ad:{angularSizeOfChunk/angularDistanceBetweenPoints}, as%ad:{angularSizeOfChunk % angularDistanceBetweenPoints}");
            Debug.Log(pointIndexInChunk + " " + chunkIndex);
            if (pointIndexInChunk > _chunks[chunkIndex].spline.GetPointCount() - 1)
                _chunks[chunkIndex].spline.InsertPointAt(pointIndexInChunk, pointCoordinates);
            else
                _chunks[chunkIndex].spline.SetPosition(pointIndexInChunk, pointCoordinates);
        }
        // Fill in the gaps
        for (int i = 0; i < _chunks.Count; i++)
        {
            int nextChunk = i + 1 < _chunks.Count ? i + 1 : 0;
            _chunks[i].spline.InsertPointAt(_chunks[i].spline.GetPointCount(), _chunks[nextChunk].spline.GetPosition(0));
        }
        //Set tangents
        //for (int i = 0; i < _chunks[0].spline.GetPointCount(); i++)
        //{
        //    int previousPointIndex = i - 1;
        //    if (i - 1 < 0)
        //        previousPointIndex = _chunks[0].spline.GetPointCount() - 1;
        //    int nextPointIndex = i + 1;
        //    if (i + 1 > _chunks[0].spline.GetPointCount() - 1)
        //        nextPointIndex = 0;

        //    Vector3 directionToPreviousPoint = (_chunks[0].spline.GetPosition(previousPointIndex) - _chunks[0].spline.GetPosition(i)).normalized;
        //    Vector3 directionToNextPoint = (_chunks[0].spline.GetPosition(nextPointIndex) - _chunks[0].spline.GetPosition(i)).normalized;
        //    Vector3 average = (directionToPreviousPoint - directionToNextPoint) / 2;
        //    average = average.normalized * _tangentScale;

        //    _chunks[0].spline.SetTangentMode(i, ShapeTangentMode.Continuous);
        //    _chunks[0].spline.SetLeftTangent(i, average);
        //    _chunks[0].spline.SetRightTangent(i, -average);
        //}
        // Add a point in the center
        if (_chunkHasAPointInTheCenter)
            foreach (var chunk in _chunks)
                chunk.spline.InsertPointAt(chunk.spline.GetPointCount(), Vector3.zero);
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
    private void ResetTerrain()
    {

        foreach(var chunk in _chunks)
        {
            chunk.spline.isOpenEnded = _isOpenEnded;
            // Leave 2 points in a chunk
            int howManyPointsToLeave = 2;
            if (chunk.spline.GetPointCount() > 0)
                for (int i = chunk.spline.GetPointCount() - 1; i > howManyPointsToLeave - 1; i--)
                    chunk.spline.RemovePointAt(i);

            chunk.spline.SetPosition(0, new Vector3(0, 0));
            chunk.spline.SetLeftTangent(0, new Vector3(-1, 0) * _tangentScale);
            chunk.spline.SetRightTangent(0, new Vector3(1, 0) * _tangentScale);

            chunk.spline.SetPosition(1, new Vector3(1, 0));
            chunk.spline.SetLeftTangent(1, new Vector3(-1, 0) * _tangentScale);
            chunk.spline.SetRightTangent(1, new Vector3(1, 0) * _tangentScale);
        }
    }
}
