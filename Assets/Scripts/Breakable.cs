using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private BreakableConditionUI uiConditionPrefab;
    public int2 GridPosition;
    private float _health;
    private BreakableConditionUI _conditionUi;
    
    public float Health => _health;
    
    private void Start()
    {
        GridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MakeUnwalkable(GridPosition);
        transform.position = GridPosition.GetWorldPosition();
        
        GameObject ui = GameObject.Find("BreakableUI");
        _conditionUi = Instantiate(uiConditionPrefab, ui.transform);
        
        SetHealth(1.0f);
        
        _conditionUi.transform.localScale = Vector3.one;
        _conditionUi.Initialize(this);
    }

    public void SetHealth(float hp)
    {
        _health = Mathf.Clamp(hp, 0, 1.0f);
        _conditionUi.SetCondition(_health);
    }
}
