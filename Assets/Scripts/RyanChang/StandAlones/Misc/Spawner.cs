using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// A spawner for objects.
/// </summary>
/// 
/// <remarks>
/// Authors: Ryan Chang (2024)
/// </remarks>
public class Spawner : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// If true, start spawning as soon as this script is initialized.
    /// </summary>
    [Tooltip("If true, start spawning as soon as this script is initialized.")]
    public bool autostart = true;

    /// <summary>
    /// If true, then destroy all spawned objects when this spawner is
    /// destroyed.
    /// </summary>
    [Tooltip("If true, then destroy all spawned objects when this spawner " +
        "is destroyed.")]
    public bool cleanUpSpawns = false;

    /// <summary>
    /// List of objects to spawn.
    /// </summary>
    [Tooltip("List of objects to spawn.")]
    public List<GameObject> objectsToSpawn = new();

    /// <summary>
    /// Objects that have been spawned.
    /// </summary>
    [Tooltip("Objects that have been spawned.")]
    [HideInInspector]
    public List<GameObject> objectsSpawned = new();

    /// <summary>
    /// Number of objects to spawn.
    /// </summary>
    [Tooltip("Number of objects to spawn.")]
    public RNGPattern spawnNum = new(1);

    /// <summary>
    /// Spawn objects after how many seconds?
    /// </summary>
    [Tooltip("Spawn objects after how many seconds?")]
    public float spawnTime = 0.1f;

    /// <summary>
    /// Maximum range of the spawner.
    /// </summary>
    [Tooltip("Maximum range of the spawner.")]
    public float spawnRadius;

    /// <summary>
    /// The force to apply to the spawned object.
    /// </summary>
    [Tooltip("The force to apply to the spawned object.")]
    public float maxStartForce;

    /// <summary>
    /// The rotation to start at.
    /// </summary>
    [Tooltip("The rotation to start at.")]
    public RNGPattern startRotation;
    #endregion

    #region Methods
    private void Start()
    {
        if (autostart)
        {
            Invoke(nameof(StartSpawn), 0.1f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    /// <summary>
    /// For some reason, the entities fail to spawn in the correct location
    /// if the spawning is not deferred.
    /// </summary>
    public void StartSpawn()
    {
        if (spawnTime > 0)
        {
            StartCoroutine(SpawnObjectsCoroutine());
        }
        else
        {
            for (int i = 0; i < spawnNum.Select(); i++)
            {
                SpawnObjects();
            }
        }
    }

    private IEnumerator SpawnObjectsCoroutine()
    {
        for (int i = 0; i < spawnNum.Select(); i++)
        {
            SpawnObjects();

            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void SpawnObjects()
    {
        foreach (var toSpawn in objectsToSpawn)
        {
            TransformSet(Instantiate(toSpawn));
        }
    }

    private void TransformSet(GameObject enemyObj)
    {
        enemyObj.SetActive(true);

        Vector3 offset = new(0, 0, RNGExt.RandomFloat(0, spawnRadius));
        offset = Quaternion.Euler(0, RNGExt.RandomFloat(0, 360), 0) * offset;

        if (enemyObj.HasComponent(out NavMeshAgent nma))
        {
            nma.Warp(transform.position + offset);
            enemyObj.transform.rotation =
                Quaternion.Euler(0, startRotation.Evaluate(), 0);
        }
        else
        {
            enemyObj.transform.SetPositionAndRotation(
                transform.position + offset,
                Quaternion.Euler(0, startRotation.Evaluate(), 0)
            );
        }

        if (enemyObj.HasComponent(out Rigidbody2D r2d))
        {
            r2d.AddForce(RNGExt.WithinCircle(Mathf.Abs(maxStartForce)));
        }

        if (cleanUpSpawns)
        {
            objectsSpawned.Add(enemyObj);
        }

        /// Uncomment for spawn debug
        //print($"Spawn at {enemyObj.transform.position} from spawner at {transform.position}");
    }

    private void OnDestroy()
    {
        foreach (var obj in objectsSpawned)
        {
            Destroy(obj);
        }
    }
    #endregion
}
