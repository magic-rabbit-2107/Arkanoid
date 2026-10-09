using System;
using UnityEngine;

public class WinState : BaseState
{
    public override void OnStateEnter(GameObject forGameObject)
    {
        Debug.Log("WinState-OnStateEnter");

        base.OnStateEnter(forGameObject);

    }
    
    public override Type Update(GameObject forGameObject)
    {
        return GetType();
    }
}