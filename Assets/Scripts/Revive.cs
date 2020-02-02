using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Revive : MonoBehaviour
{
    [SerializeField] private Breakable _breakable;

    public Breakable breakable => _breakable;
    
    public void ReviveParent()
    {
        _breakable.ReviveThis();
    }
}
