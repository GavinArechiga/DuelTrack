using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wastepool : MonoBehaviour
{
    [Header("References")] [SerializeField]
    private GameObject litterPrefab;

    [Header("Wandering")] 
    [SerializeField] private float speed = 0.5f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float wanderRadius;
    
    private Vector3 nextPosition;
    private Vector3 currentDirection;
    private const float TOLERANCE = 0.3f;
    
    [Header("Litter Spawning")] 
    [SerializeField] private LayerMask litterLayerMask;
    [SerializeField] private float spawnLitterTimer = 3f;

    private float elapsedTime;
    private Collider[] litterColliderArray;
    private Bounds litterBounds;
    
    [Header("Waste Pool Health")] 
    [SerializeField] private int wastePoolLives = 3;
    [SerializeField] private int litterSpawnAttemptsOnDestroy = 5;
    [SerializeField] private float litterSpawnRadius = 2f;
    
    private int hitCount;

    private void Start()
    {
        litterColliderArray = litterPrefab.GetComponentsInChildren<Collider>();
        CalculateBounds();

        CalculateNextPosition();
        currentDirection = (nextPosition - transform.position).normalized;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, nextPosition) <= TOLERANCE)
        {
            CalculateNextPosition();
        }

        MoveTowardsNextPosition();

        SpawnLitterOnTimer();
    }

    #region Wandering

    private void MoveTowardsNextPosition()
    {
        Vector3 desiredDirection = (nextPosition - transform.position).normalized;
        currentDirection = Vector3.Lerp(currentDirection, desiredDirection, turnSpeed * Time.deltaTime);
        currentDirection.Normalize();

        transform.position += currentDirection * (speed * Time.deltaTime);
    }

    private void CalculateNextPosition()
    {
        Vector3 randPoint = transform.position + Random.insideUnitSphere * wanderRadius;
        nextPosition = new Vector3(randPoint.x, 0, randPoint.z);
    }

    #endregion

    #region Spawning

    private void SpawnLitterOnTimer()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= spawnLitterTimer)
        {
            // Sets elapsed time to the remainder of elapsed time / spawnLitterTimer to avoid drift from time.DeltaTime.
            elapsedTime %= spawnLitterTimer;

            SpawnLitter(transform.position);
        }
    }

    private void SpawnLitterOnDestroy()
    {
        for (int i = 0; i <= litterSpawnAttemptsOnDestroy; i++)
        {
            Vector3 randPoint = transform.position + Random.insideUnitSphere * litterSpawnRadius;
            randPoint.y = transform.position.y;

            SpawnLitter(randPoint);
            Debug.Log("Spawned Litter: " + i);
        }
        
        Destroy(gameObject);
    }

    private void SpawnLitter(Vector3 position)
    {
        if (CheckIfValidSpawn(position))
        {
            Instantiate(litterPrefab, position, Quaternion.identity);
        }
    }

    private bool CheckIfValidSpawn(Vector3 position)
    {
        float radius = litterBounds.extents.magnitude;
        return !Physics.CheckSphere(position, radius, litterLayerMask);
    }

    private void CalculateBounds()
    {
        foreach (Collider col in litterColliderArray)
        {
            litterBounds.Encapsulate(col.bounds);
        }
    }
    
    #endregion

    public void ReduceWastePool()
    {
        hitCount += 1;
        Debug.Log("Waste Pool Hit Count: " + hitCount);
        if (hitCount >= wastePoolLives)
        {
            SpawnLitterOnDestroy();
        }
    }
}