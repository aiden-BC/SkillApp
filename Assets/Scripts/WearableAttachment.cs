using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class WearableAttachment : MonoBehaviour, IAttachmentOwner
{
    public Transform eyesTransform;
    public Transform headTransform;
    public float reattachCooldown = 1.0f;

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

        // Verificar si ya hay un objeto en el punto de destino
        if (targetTransform.childCount > 0) return;

        HoldableObject holdable = other.GetComponent<HoldableObject>();
        XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();

        if (holdable == null || grabInteractable == null) return;
        if (cooldownTimers.ContainsKey(holdable)) return;

        // Registrar listeners solo una vez
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
        }
    }

    private IEnumerator AttachToTargetCoroutine(HoldableObject holdable, XRGrabInteractable grabInteractable, Transform targetTransform)
    {
        var netObj = grabInteractable.GetComponent<NetworkObject>();
        if (netObj != null && netObj.enabled && (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening))
        {
            Debug.LogWarning("Desactivando NetworkObject en tiempo de ejecución para evitar errores.");
            netObj.enabled = false;
        }

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
