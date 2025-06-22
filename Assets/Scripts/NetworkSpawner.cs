using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class NetworkSpawner : NetworkBehaviour
{
    [Header("Prefabs disponibles")]
    [SerializeField] private List<GameObject> m_ObjectPrefabs;

    [Header("Configuración de spawn")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private bool m_OnlySpawnInView = false;
    [SerializeField] private float m_ViewportPeriphery = 0.1f;
    [SerializeField] private Camera m_CameraToFace;
    [SerializeField] private bool m_SpawnAsChildren = false;
    [SerializeField] private bool m_ApplyRandomAngleAtSpawn = false;
    [SerializeField] private float m_SpawnAngleRange = 30f;
    [SerializeField] private GameObject m_SpawnVisualizationPrefab;
    [SerializeField] private int m_SpawnOptionIndex = -1;
    [SerializeField] private XRInteractionManager interactionManager;
    [SerializeField] private string assignedSpawnableID;

    private GameObject currentObject;
    private bool isOccupied = false;
    private bool spawned = false;

    public event System.Action<GameObject> objectSpawned;

    public bool IsOccupied() => isOccupied;

    private void Update()
    {
        if (IsServer && !spawned && !isOccupied)
        {
            spawned = true;
            if (TrySpawnObject(spawnPoint.position, spawnPoint.up))
            {
                spawned = true;
            }
        }
    }

    public bool TrySpawnObject(Vector3 spawnPosition, Vector3 spawnNormal)
    {
        if (isOccupied) return false;

        if (m_OnlySpawnInView)
        {
            Vector3 viewportPoint = m_CameraToFace.WorldToViewportPoint(spawnPosition);
            float min = m_ViewportPeriphery;
            float max = 1f - m_ViewportPeriphery;

            if (viewportPoint.z < 0f || viewportPoint.x < min || viewportPoint.x > max || viewportPoint.y < min || viewportPoint.y > max)
                return false;
        }

        int index = IsSpawnOptionRandomized ? Random.Range(0, m_ObjectPrefabs.Count) : m_SpawnOptionIndex;
        GameObject prefab = m_ObjectPrefabs[index];
        GameObject obj = Instantiate(prefab, spawnPosition, Quaternion.identity);

        if (m_SpawnAsChildren)
            obj.transform.parent = transform;

        Vector3 forward = m_CameraToFace.transform.position - spawnPosition;
        Vector3 projectedForward = Vector3.ProjectOnPlane(forward, spawnNormal);
        obj.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);

        if (m_ApplyRandomAngleAtSpawn)
        {
            float randomAngle = Random.Range(-m_SpawnAngleRange, m_SpawnAngleRange);
            obj.transform.Rotate(Vector3.up, randomAngle);
        }

        if (m_SpawnVisualizationPrefab != null)
        {
            Instantiate(m_SpawnVisualizationPrefab, spawnPosition, obj.transform.rotation);
        }

        var grab = obj.GetComponent<XRGrabInteractable>();
        if (grab != null && interactionManager != null)
            grab.interactionManager = interactionManager;

        var respawn = obj.GetComponent<NetworkRespawn>();
        if (respawn != null)
        {
            Debug.Log($"Asignando spawner a NetworkRespawn con ID: {assignedSpawnableID}");
            respawn.spawner = this;
            respawn.spawnableID = assignedSpawnableID;
            respawn.SetSpawnPoint(spawnPosition, spawnNormal);
        }

        var netObj = obj.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }
        else
        {
            Debug.LogWarning("El objeto instanciado no tiene NetworkObject.");
        }

        currentObject = obj;
        isOccupied = true;
        objectSpawned?.Invoke(obj);
        return true;
    }

    public bool IsSpawnOptionRandomized => m_SpawnOptionIndex < 0 || m_SpawnOptionIndex >= m_ObjectPrefabs.Count;

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<XRGrabInteractable>();
        var respawn = other.GetComponent<Respawn>();

        if (interactable != null && respawn != null && respawn.spawnableID == assignedSpawnableID)
        {
            isOccupied = true;
            currentObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentObject != null && other.gameObject == currentObject)
        {
            isOccupied = false;
            currentObject = null;
        }
    }
}
