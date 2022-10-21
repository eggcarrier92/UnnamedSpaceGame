using UnityEngine;

// This class shifts the world origin when the player gets too far from it to prevent floating point precision errors
public class WorldOriginShifter : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _maxDistance = 10000f;

    private void Update()
    {
        CheckPosition();
    }
    private void CheckPosition()
    {
        if (Mathf.Abs(_playerTransform.position.x) > _maxDistance || Mathf.Abs(_playerTransform.position.y) > _maxDistance)
            ShiftWorldOriginToThePlayer();
    }
    private void ShiftWorldOriginToThePlayer()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);
        float xShift = -_playerTransform.position.x;
        float yShift = -_playerTransform.position.y;

        foreach (GameObject obj in allObjects)
        {
            if (obj.transform.parent != null)
                continue;
            obj.transform.Translate(xShift, yShift, 0, Space.World);
        }
    }
}