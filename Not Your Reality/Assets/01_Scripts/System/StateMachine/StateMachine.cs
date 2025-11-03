using UnityEngine;
using _01_Scripts._07_Enums._01_StateMachine;

namespace _01_Scripts._06_System._01_StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        public PlayerStates currentState = PlayerStates.Idle;

        private CharacterController _charCon;

        private void Awake()
        {
            _charCon = GetComponent<CharacterController>();
        }

        private void Update()
        {
            switch (currentState)
            {
                case PlayerStates.Idle:
                case PlayerStates.Walking:
                case PlayerStates.Locked: break;
            }
        }

        private void HandeIdleState()
        {
            
        }

        private void HandleWalkingState()
        {
            
        }

        private void HandleLockedState()
        {
            
        }

    }
}
