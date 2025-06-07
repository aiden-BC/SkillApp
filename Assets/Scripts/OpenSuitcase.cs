using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class OpenSuitcase : MonoBehaviour
{
    public Animator animator;
    private bool isOpen = false;

    private void OnEnable()
    {
        Debug.Log("Enabled");
        var simpleInteractable = GetComponent<XRSimpleInteractable>();
        
        //if (simpleInteractable != null)
        //{
        //    //simpleInteractable.selectEntered.AddListener(OnSelect);
        //    Debug.Log("Listener added to: " + gameObject.name);
        //    simpleInteractable.activated.AddListener(OnActivate);
        //}
    }

    //private void OnDisable()
    //{
    //    var simpleInteractable = GetComponent<XRSimpleInteractable>();
    //    if (simpleInteractable != null)
    //    {
    //        //simpleInteractable.selectEntered.RemoveListener(OnSelect);
    //        simpleInteractable.activated.RemoveListener(OnActivate);
    //    }
    //}

    public void OnActivate(ActivateEventArgs args)
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
