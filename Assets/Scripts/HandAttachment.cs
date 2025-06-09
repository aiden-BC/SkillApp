using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class HandAttachment : MonoBehaviour
{
    public Transform handTransform;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("HoldObject"))
        {
            RotateArm rotateArm = GetComponent<RotateArm>();
            rotateArm.rotateHold();

            HoldableObject holdable = other.GetComponent<HoldableObject>();
            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();

            if (holdable != null && !holdable.isAttachedToHand)
            {
                StartCoroutine(AttachToHandCoroutine(holdable, grabInteractable));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {

        if (!other.CompareTag("HoldObject")) return;

        HoldableObject holdable = other.GetComponent<HoldableObject>();
        if (holdable == null || !holdable.isAttachedToHand) return;

        // Verifica si aún está dentro del trigger
        if (Vector3.Distance(other.transform.position, transform.position) < 0.2f)
        {
            // Aún está cerca, probablemente fue un falso positivo
            return;
        }

        Debug.Log("Trigger Exit: " + other.name);

        RotateArm rotateArm = GetComponent<RotateArm>();
        rotateArm.resetHold();

        if (other.CompareTag("HoldObject"))
        {
            XRGrabInteractable grabInteractable = other.GetComponent<XRGrabInteractable>();

            if (holdable != null && holdable.isAttachedToHand)
            {
                // Desvincular de la mano
                holdable.transform.SetParent(null);
                holdable.isAttachedToHand = false;

                // Reactivar física
                Rigidbody rb = holdable.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = false;
                }

                // Asegurar que el interactable esté activo
                if (!grabInteractable.enabled)
                {
                    grabInteractable.enabled = true;
                }
            }
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

            // Esperar un frame para asegurar que se suelte
            yield return null;
        }

        // Desactivar temporalmente el interactable para evitar que se vuelva a agarrar
        grabInteractable.enabled = false;

        // Colocar en la mano
        grabInteractable.transform.SetParent(handTransform);
        grabInteractable.transform.localPosition = Vector3.zero;
        grabInteractable.transform.localRotation = Quaternion.identity;
        holdable.isAttachedToHand = true;

        // Opcional: desactivar física
        Rigidbody rb = grabInteractable.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        // Esperar un poco y volver a activar el interactable
        yield return new WaitForSeconds(0.5f);
        grabInteractable.enabled = true;
    }
}
