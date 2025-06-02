using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

internal class Respawn : MonoBehaviour
{
    public ObjectSpawner spawner;
    public float respawnDelay = 3f;
    private Vector3 spawnPoint;
    private Vector3 spawnNormal;

    private void Awake()
    {
        var grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            Debug.Log("XRGrabInteractable");

            // Create an attach transform that matches the current world pose
            GameObject attachPoint = new GameObject("AttachTransform");
            attachPoint.transform.position = transform.position;
            attachPoint.transform.rotation = transform.rotation;
            attachPoint.transform.SetParent(transform, true);

            grabInteractable.attachTransform = attachPoint.transform;

            // Optional: disable snapping behavior
            grabInteractable.attachEaseInTime = 0f;
            grabInteractable.movementType = XRBaseInteractable.MovementType.VelocityTracking;

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
        Debug.Log("Released");

        // Update attach transform to match current pose
        if (spawner != null && TryGetComponent(out XRGrabInteractable grabInteractable))
        {
            if (grabInteractable.attachTransform != null)
            {
                grabInteractable.attachTransform.position = transform.position;
                grabInteractable.attachTransform.rotation = transform.rotation;
            }
        }

        StartCoroutine(RespawnAfterDelay());
    }


    private System.Collections.IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (spawner != null)
        {
            // Only try to spawn if the spawner is not currently occupied
            var spawnSuccess = spawner.TrySpawnObject(spawnPoint, spawnNormal);
            Debug.Log(spawnSuccess ? "Object respawned." : "Spawner occupied, no respawn.");
        }
    }
}
