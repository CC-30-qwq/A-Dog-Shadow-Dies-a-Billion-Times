using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseBehaviour
{
    protected FSM FSM;
    protected float behaviourTimer;
    private readonly List<Coroutine> activeCoroutines = new();

    public BaseBehaviour(FSM machine)
    {
        FSM = machine;
    }

    public virtual void EnterState()
    {
        behaviourTimer = 0f;
    }

    public virtual void UpdateState()
    {
        behaviourTimer += Time.deltaTime;
    }

    public virtual void FixedUpdateState() { }

    public virtual void ExitState()
    {
        foreach (var coroutine in activeCoroutines)
        {
            if (coroutine != null)
            {
                FSM.StopCoroutine(coroutine);
            }
        }
        activeCoroutines.Clear();
    }

    protected Coroutine StartStateCoroutine(IEnumerator routine)
    {
        var coroutine = FSM.StartCoroutine(routine);
        activeCoroutines.Add(coroutine);
        return coroutine;
    }

    protected void StopStateCoroutine(Coroutine coroutine)
    {
        if (coroutine == null) return;
        FSM.StopCoroutine(coroutine);
        activeCoroutines.Remove(coroutine);
    }
}
