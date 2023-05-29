using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkManagerUI : MonoBehaviour
{
    [SerializeField] private Relay relay;
    [SerializeField] private Button btnServer;
    [SerializeField] private Button btnHost;
    [SerializeField] private Button btnClient;
    [SerializeField] private TMP_InputField tbJoinCode;
    [SerializeField] private TextMeshProUGUI txtJoinCode;

    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        btnServer.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();
        });
        btnHost.onClick.AddListener(() =>
        {
            relay.CreateRelay();
            tbJoinCode.gameObject.SetActive(false);
        });
        btnClient.onClick.AddListener(() =>
        {
            relay.JoinRelay(tbJoinCode.text);
            tbJoinCode.gameObject.SetActive(false);
        });
    }

    private void Singleton_OnServerStarted()
    {
        txtJoinCode.text = "Join Code:\n" + relay.JoinCode;
        txtJoinCode.gameObject.SetActive(true);
    }
}
