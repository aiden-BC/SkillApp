using UnityEngine;

public class RotateArm : MonoBehaviour
{
    public Transform upperArmBone; // Hueso del brazo
    public Transform lowerArmBone; // Hueso del brazo
    public Vector3 upperArmExtendedRotation; // Rotación cuando el brazo se extiende
    public Vector3 lowerArmExtendedRotation; // Rotación cuando el brazo se extiende
    public Vector3 upperArmDefaultRotation; // Rotación normal del brazo
    public Vector3 lowerArmDefaultRotation; // Rotación normal del brazo

    private void Start()
    {
        resetHold();
    }

    public void rotateHold()
    {
        if (upperArmBone != null)
        {
            Debug.Log("Rotación del brazo superior: ");
            upperArmBone.localRotation = Quaternion.Euler(upperArmExtendedRotation);
            Debug.Log("Rotación del brazo superior: " + upperArmBone.localRotation.eulerAngles);
        }
        if (lowerArmBone != null)
        {
            Debug.Log("Rotación del brazo inferior: ");
            lowerArmBone.localRotation = Quaternion.Euler(lowerArmExtendedRotation);
            Debug.Log("Rotación del brazo inferior: " + lowerArmBone.localRotation.eulerAngles);
        }
    }

    public void resetHold()
    {
        if (upperArmBone != null)
        {
            upperArmBone.localRotation = Quaternion.Euler(upperArmDefaultRotation);
        }
        if (lowerArmBone != null)
        {
            lowerArmBone.localRotation = Quaternion.Euler(lowerArmDefaultRotation);
        }
    }
}
