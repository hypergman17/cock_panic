namespace Zenith;

public partial class Controller {
    [Property, Feature("Debug")]
    [Title("State")]
    [Sync]
    [ReadOnly]
    public State CurrentState { get; set; } = null;

    [Property, Feature("Debug")]
    [ReadOnly]
    protected State NextState { get; set; } = null;

    public Integrator CurrentIntegrator => CurrentState?.Integrator;

    public bool IsInState<T>() where T : State
        => CurrentState.GetType() == typeof(T);

    public State SetNextState(State state) {
        if (!state.IsValid()) { return state; }
        if (NextState.IsValid()) { return state; }
        NextState = state;
        return state;
    }

    public T SetNextState<T>() where T : State
        => (T)SetNextState(GetComponent<T>());

    protected void CheckNextState() {
        if (!NextState.IsValid()) {
            foreach (var state in GetComponents<State>()) {
                if (state == CurrentState) { continue; }
                if (state.TryEnter()) {
                    NextState = state;
                    break;
                }
            }
        }

        if (!NextState.IsValid()) { return; }
        CurrentState?.Exit();
        CurrentState = NextState;
        NextState = null;
        CurrentState?.Enter();
    }

    public abstract class State : Component, IEvents {
        [RequireComponent] public Controller Controller { get; set; }

        public bool IsCurrent => Controller.CurrentState == this;
        public bool IsInState<T>() where T : State => Controller.IsInState<T>();
        public void SetNextState(State state) => Controller.SetNextState(state);
        public T SetNextState<T>() where T : State => Controller.SetNextState<T>();

        public abstract Integrator Integrator { get; }

        /// <summary>
        /// Handle extra global logic.
        /// Called right before `Move` regardless of we are the CurrentState or not.
        /// </summary>
        public virtual void Process() { }

        /// <summary>
        /// Main function that should modify Controller.Position and Controller.Velocity.
        /// </summary>
        public abstract void Move();

        /// <summary>
        /// Test for transitions.
        /// Called right before `Move` regardless of we are the CurrentState or not.
        /// </summary>
        public virtual bool TryEnter() => false;
        public virtual void Enter() { }
        public virtual void Exit() { }
    }
}
