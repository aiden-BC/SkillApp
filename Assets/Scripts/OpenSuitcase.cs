using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OpenSuitcase : MonoBehaviour
{
    public Animator animator;
    private bool isOpen = false;

    private void OnEnable()
    {
        var simpleInteractable = GetComponent<XRSimpleInteractable>();
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.AddListener(OnSelect);
        }
    }

    private void OnDisable()
    {
        var simpleInteractable = GetComponent<XRSimpleInteractable>();
        if (simpleInteractable != null)
        {
            simpleInteractable.selectEntered.RemoveListener(OnSelect);
        }
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log("Select detectado en: " + gameObject.name);

        if (animator == null) return;

        if (!isOpen)
        {
            animator.SetBool("Open", true);
        }
        else
        {
            animator.SetBool("Close", true);
        }

        isOpen = !isOpen;
    }
}
