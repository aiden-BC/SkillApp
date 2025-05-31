using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OpenSuitcase : MonoBehaviour
{
    public Animator animator;
    private bool isOpen = false;

    private XRBaseInteractable interactable;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();
        Debug.Log("OpenSuitcase script initialized");
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelect);
        Debug.Log("OpenSuitcase script enabled, listener added");
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelect);
        Debug.Log("OpenSuitcase script disabled, listener removed");
    }

    private void OnSelect(SelectEnterEventArgs args)
    {
        Debug.Log("Suitcase selected");
        if (!isOpen)
        {
            Debug.Log("Suitcase opened");
            animator.SetBool("Open", true);
            isOpen = true;
        }
        else
        {
            Debug.Log("Suitcase closed");
            animator.SetBool("Close", true);
            isOpen = false;
        }
        
    }
}
