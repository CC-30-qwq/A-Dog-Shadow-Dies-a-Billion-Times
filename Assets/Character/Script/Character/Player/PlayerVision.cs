using System.Collections.Generic;
using UnityEngine;

public class PlayerVision : MonoBehaviour
{
    public List<Transform> Targets;
    public new Camera camera;
    public LayerMask BarrierLayer;

    private void Update()
    {
        Targets.RemoveAll(item => item == null);

        if (Targets.Count != 0)
        {
            foreach (var target in Targets)
            {
                if (target.GetComponent<StateMachine>().currentState == CharacterState.Dead)
                {
                    Targets.Remove(target);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
            if (GeometryUtility.TestPlanesAABB(planes, other.transform.Find("CharacterTarget").GetComponent<Renderer>().bounds) && !Physics.Linecast(camera.transform.position, other.transform.Find("CharacterTarget").transform.position, BarrierLayer))
            {
                if (!Targets.Contains(other.transform) && other.GetComponent<StateMachine>().currentState != CharacterState.Dead)
                {
                    Targets.Add(other.transform);
                }
                else
                {
                    Debug.DrawRay(transform.position, other.transform.Find("CharacterTarget").position - transform.position, Color.yellow);
                }

            }
            else
            {
                Targets.Remove(other.transform);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Targets.Remove(other.transform);
        }
    }
}
