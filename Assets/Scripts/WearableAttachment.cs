using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using System.Collections;
using System.Collections.Generic;

public class WearableAttachment : MonoBehaviour, IAttachmentOwner
{
    public Transform eyesTransform;
    public Transform headTransform;
    public float reattachCooldown = 1.0f;

    private Dictionary<HoldableObject, float> cooldownTimers = new Dictionary<HoldableObject, float>();

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
            Debug.Log("HatObject detected: " + other.name);
        }
        else
        {
            return;
        }

        HoldableObject holdable = other.GetComponent<HoldableObject>();
        XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();

        if (holdable == null || grabInteractable == null) return;
        if (cooldownTimers.ContainsKey(holdable)) return;

        if (!holdable.isAttached)
        {
            StartCoroutine(AttachToTargetCoroutine(holdable, grabInteractable, targetTransform));
        }
    }

    private IEnumerator AttachToTargetCoroutine(HoldableObject holdable, XRGrabInteractable grabInteractable, Transform targetTransform)
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

        grabInteractable.enabled = false;

        grabInteractable.transform.SetParent(targetTransform);
        Debug.Log(targetTransform);
        grabInteractable.transform.localPosition = Vector3.zero;
        grabInteractable.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

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
    }
}
