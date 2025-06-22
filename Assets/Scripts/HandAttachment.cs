using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandAttachment : MonoBehaviour, IAttachmentOwner
{
    public Transform handTransform;
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

        // Verificar si ya hay un objeto en la mano
        if (handTransform.childCount > 0) return;

        HoldableObject holdable = other.GetComponent<HoldableObject>();
        XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();

        if (holdable == null || grabInteractable == null) return;

        // Si está en cooldown, no hacer nada
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
            StartCoroutine(AttachToHandCoroutine(holdable, grabInteractable));
        }
    }

    private void DetachIfNeeded(XRGrabInteractable grabInteractable, HoldableObject holdable)
    {
        if (grabInteractable.transform.parent == handTransform)
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

    private IEnumerator AttachToHandCoroutine(HoldableObject holdable, XRGrabInteractable grabInteractable)
    {
        grabInteractable.enabled = true;

        // Forzar soltado si está agarrado
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

        // Rotar el brazo justo después de acoplar el objeto
        RotateArm rotateArm = GetComponent<RotateArm>();
        rotateArm.rotateHold();

        // Desactivar temporalmente el interactable
        grabInteractable.enabled = false;

        // Colocar en la mano del personaje
        grabInteractable.transform.SetParent(handTransform);
        grabInteractable.transform.localPosition = Vector3.zero;
        grabInteractable.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        holdable.isAttached = true;
        holdable.currentOwner = this;

        // Desactivar física
        Rigidbody rb = grabInteractable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Esperar un poco y volver a activar el interactable
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
