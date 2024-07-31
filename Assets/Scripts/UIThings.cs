using UnityEngine;
using TMPro;

public class UIThings : MonoBehaviour
{
    private Transform _cameraTransform;
    private TextMeshProUGUI _textMeshPro;
    private Canvas _canvas;
    private float _baseFontSize = 14f; // Base font size
    private float _distanceMultiplier = 100f; // Multiplier for scaling the font size

    private void Start()
    {
        _cameraTransform = Camera.main.transform; // Get the main camera transform
        _canvas = GetComponentInChildren<Canvas>(); // Get the Canvas component from the children
        _textMeshPro = _canvas.GetComponentInChildren<TextMeshProUGUI>(); // Get the TextMeshPro component from the canvas
        _canvas.renderMode = RenderMode.WorldSpace; // Ensure the canvas is in World Space
    }

    private void Update()
    {
        // Make the canvas always face the camera
        _canvas.transform.LookAt(_cameraTransform);
        _canvas.transform.Rotate(Vector3.up, 180f); // Rotate to face the camera properly

        // Calculate the distance from the camera
        float distance = Vector3.Distance(_canvas.transform.position, _cameraTransform.position);

        // Adjust the font size based on the distance
        if (distance < 3000)
        {
            _textMeshPro.fontSize = _baseFontSize * (distance / _distanceMultiplier);
        }
    }
}