using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ChangeBoardMaterial : MonoBehaviour
{
    public Renderer boardPlane;      // El plano grande
    public Material newMaterial;     // Material que se aplicará

    private XRBaseInteractable interactable;

    // Material compartido por todos los tableros
    private static Material materialInstanciado;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        // Solo una vez se instancia el material
        if (boardPlane != null && materialInstanciado == null)
        {
            materialInstanciado = new Material(boardPlane.sharedMaterial);
            boardPlane.material = materialInstanciado;
        }
    }

    private void OnEnable()
    {
        interactable.selectEntered.AddListener(OnSelected);
    }

    private void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnSelected);
    }

    private void OnSelected(SelectEnterEventArgs args)
    {
        Debug.Log("Tablero seleccionado: " + gameObject.name);

        if (materialInstanciado != null && newMaterial != null)
        {
            materialInstanciado.CopyPropertiesFromMaterial(newMaterial);
        }
    }
}
