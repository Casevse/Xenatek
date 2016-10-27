using UnityEngine;
using System.Collections;

public class FiniteStateMachine<T> {

    public FSMState<T> currentState {
        get;
        private set;
    }

    private T owner;


    public void Init(T owner) {
        this.owner = owner;
        currentState = null;
    }

    public void Update() {
        if (currentState != null) {
            currentState.Execute();
        }
    }

    public void ChangeState(FSMState<T> newState) {
        // NOTE: Right now, you can pass null to this.
        if (currentState != null) {
            currentState.Exit();
        }
        currentState = newState;
        if (currentState != null) {
            currentState.Enter(owner);
        }
    }

}
