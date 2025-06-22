using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Unity.Netcode;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(NetworkObject))]
public class HoldableObject : MonoBehaviour
{
    public bool isAttached = false;
    public IAttachmentOwner currentOwner;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnSelectEntered);
        grabInteractable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDestroy()
    {
        grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
        grabInteractable.selectExited.RemoveListener(OnSelectExited);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Transferir ownership al jugador que agarra el objeto
        var netObj = GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned)
        {
            var interactor = args.interactorObject;
            var interactorNetObj = interactor.transform.GetComponentInParent<NetworkObject>();
            if (interactorNetObj != null)
            {
                netObj.ChangeOwnership(interactorNetObj.OwnerClientId);
                Debug.Log($"[HoldableObject] Ownership transferido a {interactorNetObj.OwnerClientId}");
            }
        }

        // Detener cualquier seguimiento activo
        ClearAllFollowers();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        isAttached = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // Detener seguimiento
        ClearAllFollowers();

        // Opcional: devolver ownership al servidor
        var netObj = GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned)
        {
            netObj.RemoveOwnership();
        }
    }

    private void ClearAllFollowers()
    {
        var hatFollower = GetComponent<HatFollower>();
        if (hatFollower != null)
        {
            hatFollower.SetFollowTarget(null);
        }

        var handFollower = GetComponent<HandFollower>();
        if (handFollower != null)
        {
            handFollower.SetFollowTarget(null);
        }
    }
}
