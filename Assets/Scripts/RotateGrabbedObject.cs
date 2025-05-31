using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RotateGrabbedObject : MonoBehaviour
{
    public float rotationVelocity = 100f;
    private XRGrabInteractable grabInteractable;
    private bool isGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(_ => isGrabbed = true);
        grabInteractable.selectExited.AddListener(_ => isGrabbed = false);
    }

    private void Update()
    {
        if (!isGrabbed) return;

        Vector2 input = Gamepad.current?.rightStick.ReadValue() ?? Vector2.zero;

        if (input != Vector2.zero)
        {
            transform.Rotate(Vector3.up, input.x * rotationVelocity * Time.deltaTime, Space.World);
            transform.Rotate(Vector3.right, -input.y * rotationVelocity * Time.deltaTime, Space.World);
        }
    }
}
