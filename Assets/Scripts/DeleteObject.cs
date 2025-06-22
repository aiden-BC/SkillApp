using System.Collections;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    [SerializeField] private float delay = 2.0f; // Delay before destruction

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Object {other.gameObject.name} entered trigger zone.");
        StartCoroutine(DestroyAfterDelay(other.gameObject));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
