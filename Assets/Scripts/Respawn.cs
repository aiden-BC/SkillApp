using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
public class Respawn : MonoBehaviour
{
    public ObjectSpawner spawner;
    public float respawnDelay = 3f;
    public string spawnableID;

    private Vector3 spawnPoint;
    private Vector3 spawnNormal;
    private Coroutine respawnCoroutine;

    private void Awake()
    {
        var grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            GameObject attachPoint = new GameObject("AttachTransform");
            attachPoint.transform.position = transform.position;
            attachPoint.transform.rotation = transform.rotation;
            attachPoint.transform.SetParent(transform, true);

            grabInteractable.attachTransform = attachPoint.transform;
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    public void SetSpawnPoint(Vector3 point, Vector3 normal)
    {
        spawnPoint = point;
        spawnNormal = normal;
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        if (spawner != null && TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            if (grabInteractable.attachTransform != null)
            {
                grabInteractable.attachTransform.position = transform.position;
                grabInteractable.attachTransform.rotation = transform.rotation;
            }

            if (respawnCoroutine != null)
                StopCoroutine(respawnCoroutine);

            respawnCoroutine = StartCoroutine(RespawnAfterDelay());
        }
    }

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (spawner != null && !spawner.IsOccupied())
        {
            var success = spawner.TrySpawnObject(spawnPoint, spawnNormal);
            Debug.Log(success ? "Object respawned." : "Spawner still occupied, no respawn.");
        }

        respawnCoroutine = null;
    }
}
