using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BestHTTP;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Authorization : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _username;
    [SerializeField] private TextMeshProUGUI _password;
    [SerializeField] private Button _registrationButton;

    private HTTPRequest _request;
    private string _url = "http://localhost:3000/";

    private void Awake()
    {
        _registrationButton.onClick.AddListener(SendRequestOnServer);
    }

    private void SendRequestOnServer()
    {
        if (_username.text != null && _password.text != null)
        {
            HTTPRequest request = new HTTPRequest(new Uri(_url), OnRequestFinished);
            request.Send();
        }
    }
    
    void OnRequestFinished(HTTPRequest request, HTTPResponse response)
    {
        Debug.Log("Request Finished! Text received: " + response.DataAsText);
    }
}
