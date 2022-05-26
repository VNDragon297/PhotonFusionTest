using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainUIScreen : MonoBehaviour
{
    [SerializeField] private TMP_InputField displayNameInput;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private void Awake()
    {
        UIScreen.Focus(GetComponent<UIScreen>());
        displayNameInput.text = PlayerPrefs.GetString(Constants.DISPLAYNAMEPREF, string.Empty);
    }

    private void Start()
    {
        displayNameInput.onValueChanged.AddListener(delegate { DisplayNameChanged(displayNameInput.text); });
    }

    private void DisplayNameChanged(string displayName)
    {
        PlayerPrefs.SetString(Constants.DISPLAYNAMEPREF, displayName);
    }
}
