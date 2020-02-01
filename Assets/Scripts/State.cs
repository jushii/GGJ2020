using System;

namespace DefaultNamespace
{
    /// <summary>
    /// State is controlled by a StateMachine.
    /// </summary>
    [Serializable]
    public abstract class State
    {
        internal StateMachine StateMachine { get; set; }

        /// <summary>
        /// Called before update methods when entering the state.
        /// </summary>
        public abstract void EnterState(object args);

        /// <summary>
        /// Last method called before exiting the state.
        /// </summary>
        public abstract void ExitState();

        /// <summary>
        /// Run Update.
        /// </summary>
        public virtual void Tick() { }

        /// <summary>
        /// Run FixedUpdate.
        /// </summary>
        public virtual void FixedTick() { }

        /// <summary>
        /// Run LateUpdate.
        /// </summary>
        public virtual void LateTick() { }
    }
}