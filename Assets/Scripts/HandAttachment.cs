using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Unity.Netcode;

public class HandAttachment : MonoBehaviour, IAttachmentOwner
{
    public Transform handTransform;
    public RotateArm rotateArm;
    public float reattachCooldown = 2.0f;

    private Dictionary<HoldableObject, float> cooldownTimers = new Dictionary<HoldableObject, float>();
    private HashSet<XRGrabInteractable> registeredInteractables = new HashSet<XRGrabInteractable>();

    private void Update()
    {
        List<HoldableObject> keys = new List<HoldableObject>(cooldownTimers.Keys);
        foreach (var obj in keys)
        {
            cooldownTimers[obj] -= Time.deltaTime;
            if (cooldownTimers[obj] <= 0)
            {
                cooldownTimers.Remove(obj);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("HoldObject")) return;

        if (handTransform.childCount > 0) return;

        HoldableObject holdable = other.GetComponent<HoldableObject>();
        XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();

        if (holdable == null || grabInteractable == null) return;
        if (cooldownTimers.ContainsKey(holdable)) return;

        if (!registeredInteractables.Contains(grabInteractable))
        {
            grabInteractable.selectEntered.AddListener((args) => DetachIfNeeded(grabInteractable, holdable));
            grabInteractable.selectExited.AddListener((args) => DetachIfNeeded(grabInteractable, holdable));
            registeredInteractables.Add(grabInteractable);
        }

        if (!holdable.isAttached)
        {
            StartCoroutine(AttachToHandCoroutine(holdable, grabInteractable));
        }
    }

    private void DetachIfNeeded(XRGrabInteractable grabInteractable, HoldableObject holdable)
    {
        HandFollower follower = grabInteractable.GetComponent<HandFollower>();
        if (follower != null)
        {
            follower.SetFollowTarget(null);
        }

        Rigidbody rb = grabInteractable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
        }

        holdable.isAttached = false;
        holdable.currentOwner = null;
    }

    private IEnumerator AttachToHandCoroutine(HoldableObject holdable, XRGrabInteractable grabInteractable)
    {
        grabInteractable.enabled = true;

        if (grabInteractable.isSelected)
        {
            var interactor = grabInteractable.firstInteractorSelecting;
            var interactionManager = grabInteractable.interactionManager;

            if (interactor != null && interactionManager != null)
            {
                interactionManager.SelectExit(interactor, grabInteractable);
            }

            yield return null;
        }

        RotateArm rotateArm = GetComponent<RotateArm>();
        rotateArm.rotateHold();

        grabInteractable.enabled = false;

        // Posicionar el objeto en la mano
        grabInteractable.transform.position = handTransform.position;
        grabInteractable.transform.rotation = handTransform.rotation;

        // Asignar seguimiento
        HandFollower follower = grabInteractable.GetComponent<HandFollower>();
        if (follower != null)
        {
            follower.SetFollowTarget(handTransform);
        }

        // Transferir ownership al jugador
        var netObj = grabInteractable.GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsSpawned)
        {
            var ownerNetObj = GetComponentInParent<NetworkObject>();
            if (ownerNetObj != null)
            {
                netObj.ChangeOwnership(ownerNetObj.OwnerClientId);
                Debug.Log($"[HandAttachment] Ownership transferido a {ownerNetObj.OwnerClientId}");
            }
        }

        holdable.isAttached = true;
        holdable.currentOwner = this;

        Rigidbody rb = grabInteractable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        yield return new WaitForSeconds(0.5f);
        grabInteractable.enabled = true;
    }

    public void StartCooldown(HoldableObject holdable)
    {
        cooldownTimers[holdable] = reattachCooldown;
        RotateArm rotateArm = GetComponent<RotateArm>();
        rotateArm.resetHold();
    }
}
