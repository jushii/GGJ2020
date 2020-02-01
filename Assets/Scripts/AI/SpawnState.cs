using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnState : State
    {
        public SpawnState(StateMachine stateMachine)
        : base(stateMachine)
        {
            
        }
        
        public override void EnterState(object args)
        {
            Debug.Log("Spawned!");
        }

        public override void ExitState()
        {
            
        }
    }
}