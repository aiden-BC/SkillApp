using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class DeleteObject : NetworkBehaviour
{
    [SerializeField] private float delay = 2.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Solo el servidor debe ejecutar esto

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned)
        {
            StartCoroutine(DestroyAfterDelay(netObj));
        }
    }

    private IEnumerator DestroyAfterDelay(NetworkObject netObj)
    {
        yield return new WaitForSeconds(delay);

        if (netObj != null && netObj.IsSpawned)
        {
            netObj.Despawn(true); // true = también destruye el GameObject
        }
    }
}
