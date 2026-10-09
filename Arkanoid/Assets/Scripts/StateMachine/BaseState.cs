using System;
using UnityEngine;

// namespace Pacmania.Utilities.StateMachines
// {
    public abstract class BaseState
{
        protected Arena _arena { get; private set; }
        public BaseState()
        {          
        }

        public virtual void OnStateEnter(GameObject forGameObject)
        {
            _arena = forGameObject.GetComponent<Arena>();

        }
        public virtual void OnStateLeave(GameObject forGameObject) { }
        public abstract Type Update(GameObject forGameObject);
    }
// }
