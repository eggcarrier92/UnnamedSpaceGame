using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class PlanetControls : MonoBehaviour
{
    public List<SpriteShapeController> Chunks => GetComponentsInChildren<SpriteShapeController>(true).ToList();
    public void DisableChunks()
    {
        foreach (var chunk in Chunks)
            chunk.gameObject.SetActive(false);
    }
    public void EnableChunks()
    {
        foreach (var chunk in Chunks)
            chunk.gameObject.SetActive(true);
    }
    public void DeleteChunks()
    {
        foreach (var chunk in Chunks)
            DestroyImmediate(chunk.gameObject);
    }
}
