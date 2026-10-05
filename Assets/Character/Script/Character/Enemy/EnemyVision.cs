using System.Collections.Generic;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public List<Transform> Targets;
    public LayerMask BarrierLayer;

    private void Update()
    {
        Targets.RemoveAll(item => item == null);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Vector3 directionToPlayer = other.transform.Find("CharacterTarget").position - transform.position;

            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, directionToPlayer.magnitude, BarrierLayer))
            {
                Targets.Remove(other.transform);
            }
            else
            {
                if (!Targets.Contains(other.transform))
                {
                    Targets.Add(other.transform);
                }
                else
                {
                    Debug.DrawRay(transform.position, directionToPlayer, Color.red);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Targets.Remove(other.transform);
        }
    }
}
