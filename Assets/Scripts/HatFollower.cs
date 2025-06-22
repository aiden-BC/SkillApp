using Unity.Netcode;
using UnityEngine;

public class HatFollower : NetworkBehaviour
{
    public HoldableObject holdable;
    private Transform target;

    public void SetFollowTarget(Transform followTarget)
    {
        target = followTarget;
        Debug.Log($"[HatFollower] Target asignado: {target?.name}");
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (holdable == null)
        {
            Debug.LogWarning("[HatFollower] HoldableObject no asignado.");
            return;
        }

        if (!holdable.isAttached)
        {
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("[HatFollower] Target es null.");
            return;
        }

        // Debug de posición antes y después
        Vector3 beforePos = transform.position;
        transform.position = target.position;
        transform.rotation = target.rotation;
        Debug.Log($"[HatFollower] Posición actualizada de {beforePos} a {transform.position}");
    }
}
