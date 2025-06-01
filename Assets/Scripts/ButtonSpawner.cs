using UnityEngine;
using UnityEngine.UI;

public class ButtonSpawner : MonoBehaviour
{
    public Transform spawnPoint; // Lugar donde aparecerán los objetos
    public string prefabFolder = "Prefabs";

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(SpawnObject);
        }
    }

    void SpawnObject()
    {
        string objectName = transform.parent.name;
        GameObject prefab = Resources.Load<GameObject>($"{prefabFolder}/{objectName}");

        if (prefab != null)
        {
            Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log($"Instanciado: {objectName}");
        }
        else
        {
            Debug.LogWarning($"No se encontró prefab con nombre: {objectName}");
        }
    }

}
