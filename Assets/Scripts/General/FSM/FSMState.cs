using UnityEngine;
using System.Collections;

abstract public class FSMState<T> {

    protected T entity;

    abstract public void Enter(T entity);

    abstract public void Execute();

    abstract public void Exit();

}