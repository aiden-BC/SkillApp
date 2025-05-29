
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;


internal class Respawn : MonoBehaviour
{
    public ObjectSpawner spawner;
    public float respawnDelay = 3f;
    private Vector3 spawnPoint;
    private Vector3 spawnNormal;

    private void Awake()
    {
        spawnPoint = transform.position;
        spawnNormal = Vector3.up;

        var grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            Debug.Log("XRGrabInteractable");
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("Released");
        StartCoroutine(RespawnAfterDelay());
    }

    private System.Collections.IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(respawnDelay);

        if (spawner != null)
        {
            Debug.Log("spawner");
            spawner.TrySpawnObject(spawnPoint, spawnNormal);
        }

        //Destroy(gameObject); // Optional: remove the current object
    }
}