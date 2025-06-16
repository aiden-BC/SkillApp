using UnityEngine;

public class XRPanelSwitcher : MonoBehaviour
{
    public GameObject panel1;
    public GameObject panel2;
    public GameObject panel3;
    public void ShowPanel1()
    {
        panel1.SetActive(true);
        panel3.SetActive(false);
        panel2.SetActive(false);
    }

    public void ShowPanel2()
    {
        panel1.SetActive(false);
        panel2.SetActive(true);
        panel3.SetActive(false);
    }

    public void ShowPanel3()
    {
        panel1.SetActive(false);
        panel2.SetActive(false);
        panel3.SetActive(true);
    }
}
