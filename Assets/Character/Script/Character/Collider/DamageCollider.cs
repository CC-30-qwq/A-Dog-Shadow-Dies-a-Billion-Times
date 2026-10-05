using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    public FSM FSM;
    public StateMachine state;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Weapon"))
        {
            Vector3 closestPointOnThis = GetComponent<Collider>().ClosestPoint(other.transform.position);

            FSM.DamagePosition = closestPointOnThis;

            state.StateUpdate();
        }
    }
}
