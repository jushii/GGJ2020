using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class InPitState : State
    {
        private Player _player;
        
        public InPitState(StateMachine stateMachine, Player player)
            : base(stateMachine)
        {
            _player = player;
        }
        
        public override void EnterState(object args)
        {
            _player.playerInput.Horizontal = 0.0f;
            _player.playerInput.Vertical = 0.0f;
            GameManager.Instance.currentNpcCount--;
            Debug.Log("NPC DIED! :(");
            _player.gameObject.SetActive(false);
        }

        public override void ExitState()
        {
            
        }
    }
}