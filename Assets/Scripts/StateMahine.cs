using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    /// <summary>
    /// StateMachine updates the current State.
    /// </summary>
    public abstract class StateMachine
    {
        internal State CurrentState { get; set; }
        internal State PreviousState { get; set; }

        protected List<State> states = new List<State>();

        internal void AddState(State state)
        {
            state.StateMachine = this;
            states.Add(state);
        }

        /// <summary>
        /// Exit current state.
        /// </summary>
        internal virtual void Exit()
        {
            CurrentState.ExitState();
        }

        /// <summary>
        /// Run Update.
        /// </summary>
        internal void Tick()
        {
            CurrentState?.Tick();
        }

        /// <summary>
        /// Run FixedUpdate.
        /// </summary>
        internal void FixedTick()
        {
            CurrentState?.FixedTick();
        }

        /// <summary>
        /// Run LateUpdate.
        /// </summary>
        internal void LateTick()
        {
            CurrentState?.LateTick();
        }

        public virtual bool ChangeState(Type stateType, object args = null)
        {
            // Return if we try to change to the already active State.
            if (CurrentState != null && CurrentState.GetType() == stateType)
            {
                Debug.LogWarning("@StateMachine: Trying to change to State of type: " + stateType +
                                 " but it's already the active State.");
                return false;
            }

            // Find the State by type.
            for (int i = 0; i < states.Count; i++)
            {
                // Found the state we want to transition to.
                if (states[i].GetType() == stateType)
                {
                    // Exit previous State.
                    PreviousState = CurrentState;
                    if (PreviousState != null)
                    {
                        PreviousState.ExitState();
                    }

                    // Enter next State.
                    CurrentState = states[i];
                    CurrentState.EnterState(args);
                    return true;
                }
            }

            Debug.LogError("@StateMachine: Can't find State of type: " + stateType);
            return false;
        }
    }
}