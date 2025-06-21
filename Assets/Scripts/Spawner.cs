using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ObjectSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private List<GameObject> m_ObjectPrefabs = new List<GameObject>();
    [SerializeField] private int m_SpawnOptionIndex = -1;
    [SerializeField] private bool m_SpawnAsChildren = true;
    [SerializeField] private GameObject m_SpawnVisualizationPrefab;

    [Header("View Constraints")]
    [SerializeField] private bool m_OnlySpawnInView = true;
    [SerializeField] private float m_ViewportPeriphery = 0.15f;

    [Header("Rotation Settings")]
    [SerializeField] private bool m_ApplyRandomAngleAtSpawn = true;
    [SerializeField] private float m_SpawnAngleRange = 45f;

    [Header("Interaction")]
    [SerializeField] private XRInteractionManager interactionManager;

    [Header("Camera")]
    [SerializeField] private Camera m_CameraToFace;

    [Header("Spawner Identity")]
    [SerializeField] private string assignedSpawnableID;

    private GameObject currentObject;
    private bool isOccupied = false;

    public event Action<GameObject> objectSpawned;

    public bool IsOccupied() => isOccupied;

    private void Awake()
    {
        EnsureFacingCamera();
    }

    private void Start()
    {
        TrySpawnObject(transform.position, Vector3.up);
    }

    private void EnsureFacingCamera()
    {
        if (m_CameraToFace == null)
            m_CameraToFace = Camera.main;
    }

    public bool TrySpawnObject(Vector3 spawnPoint, Vector3 spawnNormal)
    {
        if (isOccupied) return false;

        if (m_OnlySpawnInView)
        {
            var inViewMin = m_ViewportPeriphery;
            var inViewMax = 1f - m_ViewportPeriphery;
            var pointInViewportSpace = m_CameraToFace.WorldToViewportPoint(spawnPoint);
            if (pointInViewportSpace.z < 0f || pointInViewportSpace.x > inViewMax || pointInViewportSpace.x < inViewMin ||
                pointInViewportSpace.y > inViewMax || pointInViewportSpace.y < inViewMin)
            {
                return false;
            }
        }

        int objectIndex = isSpawnOptionRandomized ? UnityEngine.Random.Range(0, m_ObjectPrefabs.Count) : m_SpawnOptionIndex;
        GameObject prefabToSpawn = m_ObjectPrefabs[objectIndex];
        GameObject newObject = Instantiate(prefabToSpawn);
        currentObject = newObject;

        if (m_SpawnAsChildren)
            newObject.transform.parent = transform;

        newObject.transform.position = spawnPoint;

        Vector3 facePosition = m_CameraToFace.transform.position;
        Vector3 forward = facePosition - spawnPoint;
        Vector3 projectedForward = Vector3.ProjectOnPlane(forward, spawnNormal);
        newObject.transform.rotation = Quaternion.LookRotation(projectedForward, spawnNormal);

        if (m_ApplyRandomAngleAtSpawn)
        {
            float randomRotation = UnityEngine.Random.Range(-m_SpawnAngleRange, m_SpawnAngleRange);
            newObject.transform.Rotate(Vector3.up, randomRotation);
        }

        if (m_SpawnVisualizationPrefab != null)
        {
            Transform visualizationTrans = Instantiate(m_SpawnVisualizationPrefab).transform;
            visualizationTrans.position = spawnPoint;
            visualizationTrans.rotation = newObject.transform.rotation;
        }

        var grabInteractable = newObject.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null && interactionManager != null)
            grabInteractable.interactionManager = interactionManager;

        var respawnComponent = newObject.GetComponent<Respawn>();
        if (respawnComponent != null)
        {
            respawnComponent.spawner = this;
            respawnComponent.spawnableID = assignedSpawnableID;
            respawnComponent.SetSpawnPoint(spawnPoint, spawnNormal);
        }

        objectSpawned?.Invoke(newObject);
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        var interactable = other.GetComponent<XRGrabInteractable>();
        if (interactable != null)
        {
            var respawn = other.GetComponent<Respawn>();
            if (respawn != null && respawn.spawnableID == assignedSpawnableID)
            {
                isOccupied = true;
                currentObject = other.gameObject;
            }
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

    public bool isSpawnOptionRandomized => m_SpawnOptionIndex < 0 || m_SpawnOptionIndex >= m_ObjectPrefabs.Count;
}
