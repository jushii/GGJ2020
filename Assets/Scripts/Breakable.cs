﻿using System;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

public class Breakable : MonoBehaviour
{

    [SerializeField] private Entity entity;
    [SerializeField] private BoxCollider2D myCollider;
    [SerializeField] private BreakableConditionUI uiConditionPrefab;
    [SerializeField] private float amplify_damage_taken = 1;
    [SerializeField] private GameObject reviveCollider;

    public Transform sprite;
    public SpriteRenderer spriteRenderer;
    [SerializeField] Sprite[] _sprites;
    [SerializeField] ParticleSystem fxDamaged;

    public int2 GridPosition;
    private float _health;
    public BreakableConditionUI conditionUi;
    private Color originalColor;

    internal float startHealth;
    
    public float Health => _health;
    private Entity _entity;
    internal bool inPit = false;

    public bool isWindow;
    public List<GameObject> windowTiles;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip brakingSfx;

    private void Start()
    {
        if (isWindow)
        {
            EnableLight(false);
        }
        
        GridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MakeUnwalkable(GridPosition);
        transform.position = GridPosition.GetWorldPosition();
        originalColor = spriteRenderer.color;
        GameObject ui = GameObject.Find("BreakableUI");
        conditionUi = Instantiate(uiConditionPrefab, ui.transform);

        SetHealth(startHealth);

        conditionUi.transform.localScale = Vector3.one;
        conditionUi.Initialize(this);
    }

    public void EnableLight(bool isEnabled)
    {
        foreach (var window in windowTiles)
        {
            window.SetActive(isEnabled);
        }
    }

    public bool CanTarget()
    {
        if (_entity != null && _entity.isBeingCarried) return false;

        if (_health <= 0) return false;

        if (inPit) return false;
        
        return true;
    }

    public void ReduceHealth(float amount)
    {
        Debug.Log("Reduce hp");
        float currentHealth = _health;
        float nextHealth = Mathf.Clamp(currentHealth - (amount* amplify_damage_taken), 0, 1.0f);
        SetHealth(nextHealth);
        sprite.transform.DOKill();
        spriteRenderer.DOKill();
        spriteRenderer.DOColor(Color.white, 0.07f).SetEase(Ease.Flash).OnComplete(() =>
            {
                spriteRenderer.color = originalColor;
            });
        sprite.transform.localPosition = Vector3.zero;
        sprite.transform.DOShakePosition(0.04f, 0.2f, 5);

        if(fxDamaged != null){
            fxDamaged.Stop();
            fxDamaged.Play();
        }

        if (audioSource != null && brakingSfx != null) audioSource.PlayOneShot(brakingSfx);

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
        sprite.transform.DOShakePosition(0.1f, 0.2f, 5);
    }

    public void SetHealth(float hp)
    {
        _health = Mathf.Clamp(hp, 0, 1.0f);
        conditionUi.SetCondition(_health);
        UpdateSpriteState(_health);

        if (_health <= 0.0f)
        {
            sprite.transform.DOKill();
            spriteRenderer.DOKill();
            DestroyThis();
        }
        // else
        // {
        //     sprite.transform.DOKill();
        //     spriteRenderer.DOKill();
        //     ReviveThis();
        // }

        if (_health >= 0.98f)
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
        //spriteRenderer.enabled = false;
        int2 gridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MakeWalkable(gridPosition);

        if (reviveCollider != null)
        {
            reviveCollider.SetActive(true);
        }
        else
        {
            inPit = true;
            gameObject.SetActive(false);
        }

        if (isWindow)
        {
            EnableLight(true);
        }
    }

    public void ReviveThis()
    {
        if (reviveCollider != null)
        {
            reviveCollider.SetActive(false);
        }
        
        SetHealth(0.02f);
        
        myCollider.isTrigger = false;
        spriteRenderer.enabled = true;
        int2 gridPosition = transform.position.GetGridPosition();
        GameManager.Instance.level.MakeUnwalkable(gridPosition);
        
        if (isWindow)
        {
            EnableLight(false);
        }
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

    private void UpdateSpriteState(float _health)
    {
        if (_health <= 0)
        {
            spriteRenderer.sprite = _sprites[0];
            return;
        }

        if (_sprites.Length > 2)
        {
            float p = 1.0f / (float)(_sprites.Length-1);
            for (int i =0; i < _sprites.Length-1; i++)
            {
                if (_health < (p * (i)))
                {
                    spriteRenderer.sprite = _sprites[i];
                    break;
                }
            }
        }
    }
}
