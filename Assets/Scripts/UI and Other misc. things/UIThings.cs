using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UIThings : MonoBehaviour, IPointerClickHandler
{
    private Transform _cameraTransform;
    private TextMeshProUGUI _textMeshPro;
    private Canvas _canvas;
    private float _baseFontSize = 14f; 
    private float _distanceMultiplier = 100f; 
    
    private CameraFreeLook _cameraFreeLook; 

    private void Start()
    {
        _cameraTransform = Camera.main.transform; 
        _canvas = GetComponentInChildren<Canvas>(); 
        _textMeshPro = _canvas.GetComponentInChildren<TextMeshProUGUI>(); 
        _canvas.renderMode = RenderMode.WorldSpace; 

        _cameraFreeLook = FindObjectOfType<CameraFreeLook>(); 
        
        if (_textMeshPro.gameObject.GetComponent<BoxCollider>() == null)
        {
            _textMeshPro.gameObject.AddComponent<BoxCollider>();
        }
    }

    private void Update()
    {
        _canvas.transform.LookAt(_cameraTransform);
        _canvas.transform.Rotate(Vector3.up, 180f); 
        
        float distance = Vector3.Distance(_canvas.transform.position, _cameraTransform.position);
        
        if (distance < 3000)
        {
            _textMeshPro.fontSize = _baseFontSize * (distance / _distanceMultiplier);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_cameraFreeLook != null)
        {
            _cameraFreeLook.FocusObject(gameObject); 
        }
    }
}
