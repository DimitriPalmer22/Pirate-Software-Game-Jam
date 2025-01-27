using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugManagerHelper : MonoBehaviour, IDamager, IDebugged
{
    #region Serialized Fields

    [Tooltip("A Canvas object to display debug text")] [SerializeField]
    private Canvas debugCanvas;

    [Tooltip("A TMP_Text object to display debug text")] [SerializeField]
    private TMP_Text debugText;

    #endregion

    #region Private Fields

    private float _healthChange;
    private float _toleranceChange;

    #endregion

    #region Getters

    public GameObject GameObject => gameObject;

    private Player Player => Player.Instance;

    #endregion

    #region Initialization Functions

    private void Awake()
    {
        // Add this to the debug manager
        DebugManager.Instance.AddDebuggedObject(this);
    }

    private void OnDestroy()
    {
        // Remove this from the debug manager
        DebugManager.Instance.RemoveDebuggedObject(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Set the visibility of the debug text
        SetDebugVisibility(DebugManager.Instance.IsDebugMode);
        
        Player.Instance.PlayerControls.Debug.Debug.performed += ToggleDebugMode;
    }

    private void ToggleDebugMode(InputAction.CallbackContext obj)
    {
        DebugManager.Instance.IsDebugMode = !DebugManager.Instance.IsDebugMode;
        SetDebugVisibility(DebugManager.Instance.IsDebugMode);
    }

    #endregion

    // Update is called once per frame
    private void Update()
    {
        // Update the text
        UpdateText();
    }

    private void UpdateText()
    {
        // If the debug text is null, return
        if (debugText == null)
            return;

        // Create a new string from the debug managed objects
        StringBuilder textString = new();
        foreach (var obj in DebugManager.Instance.DebuggedObjects)
        {
            textString.Append(obj.GetDebugText());
            textString.Append('\n');
        }

        // Set the text
        debugText.text = textString.ToString();
    }

    private void SetDebugVisibility(bool isVisible)
    {
        // Set the debug canvas's visibility
        debugCanvas.enabled = isVisible;
    }

    public string GetDebugText()
    {
        var sb = new StringBuilder();

        return sb.ToString();
    }
}