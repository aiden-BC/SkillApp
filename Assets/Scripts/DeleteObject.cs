using System.Collections;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    [SerializeField] private float delay;

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(DestroyAfterDelay(other.gameObject));
    }

    private IEnumerator DestroyAfterDelay(GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        Destroy(obj);
    }
}
