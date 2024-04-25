using System.Collections;
using System.Collections.Generic;
using System;
using Mirror;
using Edgegap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HelloWorldManager : MonoBehaviour
{
    [SerializeField] private string _relayProfileToken = "RELAY_PROFILE_TOKEN";
    [SerializeField] private TMP_InputField _sessionIdInputField;
    [SerializeField] private Button _createSessionButton;
    [SerializeField] private Button _joinSessionButton;
    [SerializeField] private EdgegapKcpTransport _EdgegapTransport;

    private void Awake()
    {
        EdgegapRelayService.Initialize(_relayProfileToken);
        _createSessionButton.onClick.AddListener(CreateSession);
        _joinSessionButton.onClick.AddListener(JoinSession);
        _EdgegapTransport.OnClientDisconnected += (() => SetButtonsInteractable(true));
    }

    private async void CreateSession()
    {
        SetButtonsInteractable(false);
        ApiResponse data = await EdgegapRelayService.CreateSessionAsync(2);
        _sessionIdInputField.text = data.session_id;

        //Convert uint? to uint
        uint sessionAuthorizationToken = data.authorization_token ?? 0;
        uint userAuthorizationToken = data.session_users?[0].authorization_token ?? 0;

        _EdgegapTransport.relayAddress = data.relay.ip;
        _EdgegapTransport.relayGameServerPort = data.relay.ports.server.port;
        _EdgegapTransport.relayGameClientPort = data.relay.ports.client.port;
        _EdgegapTransport.sessionId = sessionAuthorizationToken;
        _EdgegapTransport.userId = userAuthorizationToken;

        NetworkManager.singleton.StartHost();
    }

    private async void JoinSession()
    {
        SetButtonsInteractable(false);
        ApiResponse data = await EdgegapRelayService.JoinSessionAsync(_sessionIdInputField.text);

        uint sessionAuthorizationToken = data.authorization_token ?? 0;
        uint userAuthorizationToken = data.session_users?[1].authorization_token ?? 0;

        _EdgegapTransport.relayAddress = data.relay.ip;
        _EdgegapTransport.relayGameServerPort = data.relay.ports.server.port;
        _EdgegapTransport.relayGameClientPort = data.relay.ports.client.port;
        _EdgegapTransport.sessionId = sessionAuthorizationToken;
        _EdgegapTransport.userId = userAuthorizationToken;

        NetworkManager.singleton.StartClient();
    }

    private void SetButtonsInteractable(bool interactable)
    {
        _createSessionButton.gameObject.SetActive(interactable);
        _joinSessionButton.gameObject.SetActive(interactable);
    }
}
