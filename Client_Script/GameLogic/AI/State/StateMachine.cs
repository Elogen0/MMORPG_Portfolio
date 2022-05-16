namespace Kame.AI
{
    public class StateMachine<T> where T : StateController 
    {
        T context;

        public StateSO CurrentState { get; set; }
        public StateSO PrevState { get; set; }

        public StateMachine(T context, StateSO initialState)
        {
            this.context = context;
            CurrentState = initialState;
            CurrentState?.OnEnterState(context);
            PrevState = initialState;
        }

        public float elapsedTimeInState { get; set; } = 0f;

        public void Update(float deltaTime)
        {
            elapsedTimeInState += deltaTime;
            CurrentState.UpdateState(context);
        }

        public void ChangeState(StateSO nextState)
        {
            if (nextState != null)
            {
                if (nextState == CurrentState)
                    return;
                CurrentState?.OnExitState(context);
                PrevState = CurrentState;
                CurrentState = nextState;
                CurrentState?.OnEnterState(context);
                elapsedTimeInState = 0;
            }
        }
    }
}