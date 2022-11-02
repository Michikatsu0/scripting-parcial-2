using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public enum GizmoType { Never, SelectedOnly, Always }

    public Boid _prefab;
    public float _spawnRadius = 10;
    public int _spawnCount = 10;
    public Color _colour;
    public GizmoType _showSpawnRegion;

    void Awake()
    {
        for (int i = 0; i < _spawnCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * _spawnRadius;
            Boid boid = Instantiate(_prefab);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            boid.SetColour(_colour);
        }
    }

    private void OnDrawGizmos()
    {
        if (_showSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (_showSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    void DrawGizmos()
    {

        Gizmos.color = new Color(_colour.r, _colour.g, _colour.b, 0.3f);
        Gizmos.DrawSphere(transform.position, _spawnRadius);
    }
}
