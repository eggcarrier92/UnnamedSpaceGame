using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

public class ChunkLoaderUnloader : MonoBehaviour
{
    [SerializeField]
    private GameObject _planet;

    [SerializeField]
    private float _activeChunkDistance = 500f;

    [SerializeField]
    private Transform _transform;

    private List<SpriteShapeController> _chunks;

    private void Start()
    {
        _chunks = _planet.GetComponent<PlanetControls>().Chunks;
        StartCoroutine(CheckChunks());
    }
    private IEnumerator CheckChunks()
    {
        foreach (var chunk in _chunks)
        {
            Vector3 chunkPosition = chunk.spline.GetPosition((chunk.spline.GetPointCount() - 3) / 2) + _planet.transform.position;
            if ((chunkPosition - _transform.position).sqrMagnitude < Mathf.Pow(_activeChunkDistance, 2))
                chunk.gameObject.SetActive(true);
            else
                chunk.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1);
        StartCoroutine(CheckChunks());
    }
}
