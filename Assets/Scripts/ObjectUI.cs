using UnityEngine;

public class XRPanelSwitcher : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;

    public void ShowPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
    }

    public void ShowPanel1()
    {
        panel2.SetActive(false);
        panel1.SetActive(true);
    }
}
