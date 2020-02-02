using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Unity.Mathematics;
using UnityEngine;

public class Pit : MonoBehaviour
{
    private int2 GridPosition;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Sprite closePit;
    [SerializeField] Sprite openPit;

    int itemInside = 0;
    int pitCap = 2;

    [SerializeField]  private float resetTime = 20f;
    private float timer = 20f;
    public  bool isEnable = true;

    private void Start()
    {
        timer = resetTime;
        spriteRenderer.sprite = openPit;

        GridPosition = transform.position.GetGridPosition();
        
        GameManager.Instance.level.UpdateOutsideStatus(GridPosition, false);

        GameManager.Instance.pitPositions.Add(GridPosition);
        GameManager.Instance.pits.Add(this);
    }

    public void EatThings()
    {
        Debug.Log("EatThings");
        itemInside++;
        if (itemInside  >= pitCap)
        {
            isEnable = false;
            spriteRenderer.sprite = closePit;
            gameObject.layer = LayerMask.NameToLayer("Collision");
            Debug.Log("Full");
        }
    }

    private void Update()
    {
        if (!isEnable)
        {
            timer -= Time.deltaTime;

            if (timer <= 0.0f)
            {
                //Reset
                timer = 20;
                isEnable = true;
                spriteRenderer.sprite = openPit;
                itemInside = 0;

                gameObject.layer = LayerMask.NameToLayer("Default");
                return;
            }
        }
    }

}
