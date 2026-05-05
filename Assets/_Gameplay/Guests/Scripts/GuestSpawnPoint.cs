using System;
using UnityEngine;

public class GuestSpawnPoint : MonoBehaviour
{
    [field: SerializeField] public GuestType GuestType { get; private set; }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        
        Gizmos.color = Color.red;

        // rotation indicator
        
        Vector3 from = transform.position;
        from.y += 0.3f;
        
        Vector3 to = transform.position + transform.forward * 1f;
        to.y += 0.3f;
        
        Gizmos.DrawLine(from, to);
    }
}
