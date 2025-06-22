using System.Collections;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Object {other.gameObject.name} entered trigger zone.");
        StartCoroutine(DestroyAfterDelay(other.gameObject));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }
}
