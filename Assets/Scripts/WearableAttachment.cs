using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WearableAttachment : MonoBehaviour, IAttachmentOwner
{
    public Transform eyesTransform;
    public Transform headTransform;
    public float reattachCooldown = 1.0f;

    private Dictionary<HoldableObject, float> cooldownTimers = new Dictionary<HoldableObject, float>();
    private HashSet<XRGrabInteractable> registeredInteractables = new HashSet<XRGrabInteractable>();
    private HoldableObject currentHat = null;

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
        string tag = other.tag;
        Transform targetTransform = null;

        if (tag == "EyesObject")
        {
            targetTransform = eyesTransform;
        }
        else if (tag == "HatObject")
        {
            targetTransform = headTransform;

            if (currentHat != null && currentHat.isAttached)
                return;
        }
        else
        {
            return;
        }

        if (targetTransform.childCount > 0) return;

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
            StartCoroutine(AttachToTargetCoroutine(holdable, grabInteractable, targetTransform));
        }
    }

    private void DetachIfNeeded(XRGrabInteractable grabInteractable, HoldableObject holdable)
    {
        if (grabInteractable.transform.parent == eyesTransform || grabInteractable.transform.parent == headTransform)
        {
            grabInteractable.transform.SetParent(null);

            Rigidbody rb = grabInteractable.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            holdable.isAttached = false;
            holdable.currentOwner = null;

            if (currentHat == holdable)
            {
                currentHat = null;
            }

            HatFollower follower = grabInteractable.GetComponent<HatFollower>();
            if (follower != null)
            {
                follower.SetFollowTarget(null);
            }
        }
    }

    private IEnumerator AttachToTargetCoroutine(HoldableObject holdable, XRGrabInteractable grabInteractable, Transform targetTransform)
    {
        Debug.Log($"Intentando colocar {grabInteractable.name} en {targetTransform.name}");

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

        grabInteractable.enabled = false;

        grabInteractable.transform.position = targetTransform.position;
        grabInteractable.transform.rotation = targetTransform.rotation;

        HatFollower follower = grabInteractable.GetComponent<HatFollower>();
        if (follower != null)
        {
            follower.SetFollowTarget(targetTransform);
        }

        holdable.isAttached = true;
        holdable.currentOwner = this;
        currentHat = holdable;

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
    }
}
