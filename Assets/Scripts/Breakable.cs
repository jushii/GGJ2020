using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public int2 GridPosition;
    private float _health;

    public float Health => _health;
    
    private void Start()
    {
        GridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MakeUnwalkable(GridPosition);
        transform.position = GridPosition.GetWorldPosition();
        SetHealth(1.0f);
    }

    public void SetHealth(float hp)
    {
        _health = hp;
    }
}
