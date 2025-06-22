using Unity.Netcode;
using UnityEngine;

public class HandFollower : NetworkBehaviour
{
    private Transform target;
    public HoldableObject holdable;

    /// <summary>
    /// Asigna el transform que el objeto debe seguir (la mano del personaje).
    /// </summary>
    /// <param name="followTarget">Transform de la mano</param>
    public void SetFollowTarget(Transform followTarget)
    {
        target = followTarget;
        Debug.Log($"[HandFollower] Target asignado: {target?.name}");
    }

    private void Update()
    {
        // Solo el cliente propietario debe mover el objeto
        if (!IsOwner || target == null || holdable == null || !holdable.isAttached)
            return;

        // Actualiza la posición y rotación para que siga la mano
        transform.position = target.position;

        // Aplica una rotación adicional de 90 grados en el eje X, por ejemplo
        Quaternion offsetRotation = Quaternion.Euler(0f, 0f, 0f);
        transform.rotation = target.rotation * offsetRotation;

    }
}
