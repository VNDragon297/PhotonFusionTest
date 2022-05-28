using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class SessionConfigUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField sessionNameInputField;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private void Start()
    {
        var netManager = GameObject.Find("NetworkGameManager").GetComponent<NetworkGameManager>();

        sessionNameInputField.onValueChanged.AddListener(delegate { SessionNameChanged(sessionNameInputField.text); });
        hostButton.onClick.AddListener(() => netManager.JoinOrCreateLobby());
        joinButton.onClick.AddListener(() => netManager.JoinOrCreateLobby());
    }

    private void OnEnable()
    {
        UIScreen.Focus(GetComponent<UIScreen>());
    }

    private void SessionNameChanged(string sessionName)
    {
        ServerInfo.LobbyName = sessionName;
    }

    private void OnDestroy()
    {
    }

}
