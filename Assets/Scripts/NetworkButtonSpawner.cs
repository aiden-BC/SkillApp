using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkButtonSpawner : NetworkBehaviour
{
    public Transform spawnPoint;
    public string prefabFolder;

    private void Awake()
    {
        Button button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        string objectName = transform.parent.name;
        SpawnObjectServerRpc(objectName);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnObjectServerRpc(string objectName)
    {
        GameObject prefab = Resources.Load<GameObject>($"{prefabFolder}/{objectName}");

        if (prefab != null)
        {
            GameObject obj = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

            NetworkObject netObj = obj.GetComponent<NetworkObject>();
            if (netObj != null)
            {
                netObj.Spawn(); // sincroniza con todos los clientes
                Debug.Log($"[Server] Instanciado: {objectName}");
            }
            else
            {
                Debug.LogError($"El prefab '{objectName}' no tiene un componente NetworkObject.");
            }
        }
        else
        {
            Debug.LogWarning($"No se encontró prefab con nombre: {objectName}");
        }
    }
}
