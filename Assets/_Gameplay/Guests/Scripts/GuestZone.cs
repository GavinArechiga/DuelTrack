using System;
using UnityEngine;

public class GuestZone : MonoBehaviour
{
    [SerializeField] private int guestsToSpawn;
    [SerializeField] private GuestSpawnPoint[] spawnPoints;
    [SerializeField] private GameObject[] guestPrefabs;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var col = GetComponent<Collider>();
        Gizmos.DrawWireCube(transform.position, col.bounds.size);
    }
}
