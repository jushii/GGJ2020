using System;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

public class Breakable : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private BoxCollider2D myCollider;
    [SerializeField] private BreakableConditionUI uiConditionPrefab;
    public Transform sprite;
    public SpriteRenderer spriteRenderer;
    public int2 GridPosition;
    private float _health;
    public BreakableConditionUI conditionUi;
    private Color originalColor;
    
    public float Health => _health;
    private Entity _entity;
    
    private void Start()
    {
        GridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MakeUnwalkable(GridPosition);
        transform.position = GridPosition.GetWorldPosition();
        originalColor = spriteRenderer.color;
        GameObject ui = GameObject.Find("BreakableUI");
        conditionUi = Instantiate(uiConditionPrefab, ui.transform);
        
        SetHealth(1.0f);
        
        conditionUi.transform.localScale = Vector3.one;
        conditionUi.Initialize(this);
    }

    public bool CanTarget()
    {
        if (_entity != null && _entity.isBeingCarried) return false;

        if (_health <= 0) return false;

        return true;
    }
    
    public void ReduceHealth(float amount)
    {
        float currentHealth = _health;
        float nextHealth = Mathf.Clamp(currentHealth - amount, 0, 1.0f);
        SetHealth(nextHealth);
        sprite.transform.DOKill();
        spriteRenderer.DOKill();
        spriteRenderer.DOColor(Color.white, 0.07f).SetEase(Ease.Flash).OnComplete(() =>
            {
                spriteRenderer.color = originalColor;
            });
        sprite.transform.localPosition = Vector3.zero;
        sprite.transform.DOShakePosition(0.04f, 0.2f, 5);
    }

    public void IncreaseHealth(float amount)
    {
        float currentHealth = _health;
        float nextHealth = Mathf.Clamp(currentHealth + amount, 0, 1.0f);
        SetHealth(nextHealth);
        sprite.transform.DOKill();
        // spriteRenderer.DOKill();
        // spriteRenderer.DOColor(Color.white, 0.07f).SetEase(Ease.Flash).OnComplete(() =>
        // {
        //     spriteRenderer.color = originalColor;
        // });
        sprite.transform.localPosition = Vector3.zero;
        sprite.transform.DOShakePosition(0.04f, 0.1f, 5);
    }
    
    public void SetHealth(float hp)
    {
        _health = Mathf.Clamp(hp, 0, 1.0f);
        conditionUi.SetCondition(_health);

        if (_health <= 0.0f)
        {
            sprite.transform.DOKill();
            spriteRenderer.DOKill();
            DestroyThis();
        }
        else
        {
            sprite.transform.DOKill();
            spriteRenderer.DOKill();
            ReviveThis();
        }

        if (_health >= 1.0f)
        {
            conditionUi.Hide();
        }
        else
        {
            conditionUi.Show();
        }
    }

    public void DestroyThis()
    {
        myCollider.isTrigger = true;
        spriteRenderer.enabled = false;
        int2 gridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MakeWalkable(gridPosition);
    }

    public void ReviveThis()
    {
        myCollider.isTrigger = false;
        spriteRenderer.enabled = true;
        int2 gridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MakeUnwalkable(gridPosition);
    }

    private void Update()
    {
        GridPosition = myCollider.bounds.center.GetGridPosition();
            
        // if (isBeingCarried)
        // {
        //     Vector3 carryPosition = _carrierCollider.bounds.center + Vector3.up * _carryOffsetY;
        //     carryPosition.z = 0.0f;
        //     transform.position = carryPosition;
        // }
    }
}
