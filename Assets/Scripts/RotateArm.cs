using UnityEngine;

public class RotateArm : MonoBehaviour
{
    public Transform upperArmBone; // Hueso del brazo
    public Transform lowerArmBone; // Hueso del brazo
    public Vector3 upperArmExtendedRotation; // Rotaci�n cuando el brazo se extiende
    public Vector3 lowerArmExtendedRotation; // Rotaci�n cuando el brazo se extiende
    public Vector3 upperArmDefaultRotation; // Rotaci�n normal del brazo
    public Vector3 lowerArmDefaultRotation; // Rotaci�n normal del brazo

    public void rotateHold()
    {
        if (upperArmBone != null)
        {
            upperArmBone.localRotation = Quaternion.Euler(upperArmExtendedRotation);
        }
        if (lowerArmBone != null)
        {
            lowerArmBone.localRotation = Quaternion.Euler(lowerArmExtendedRotation);
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
