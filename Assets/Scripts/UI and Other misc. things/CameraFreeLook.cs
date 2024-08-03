using UnityEngine;
using TMPro;

public class CameraFreeLook : MonoBehaviour
{
    [Header("Focus Object")]
    [SerializeField, Tooltip ("Enable double-click to focus on objects?")] 
    private bool doFocus = false;
    [SerializeField] private float focusLimit = 100f;
    [SerializeField] private float minFocusDistance = 5.0f;
    private float doubleClickTime = .15f;
    private float cooldown = 0;
    
    [Header("Undo - Only undoes the Focus Object - The keys must be pressed in order.")]
    [SerializeField] private KeyCode firstUndoKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode secondUndoKey = KeyCode.Z;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float rotationSpeed = 10.0f;
    [SerializeField] private float zoomSpeed = 10.0f;

    Quaternion prevRot = new Quaternion();
    Vector3 prevPos = new Vector3();
    private float prevMoveSpeed;

    [Header("Axes Names")]
    [SerializeField, Tooltip("Otherwise known as the vertical axis")] private string mouseY = "Mouse Y";
    [SerializeField, Tooltip("AKA horizontal axis")] private string mouseX = "Mouse X";
    [SerializeField, Tooltip("The axis you want to use for zoom.")] private string zoomAxis = "Mouse ScrollWheel";

    [Header("Move Keys")]
    [SerializeField] private KeyCode forwardKey = KeyCode.W;
    [SerializeField] private KeyCode backKey = KeyCode.S;
    [SerializeField] private KeyCode leftKey = KeyCode.A;
    [SerializeField] private KeyCode rightKey = KeyCode.D;

    [Header("Flat Move"), Tooltip("Instead of going where the camera is pointed, the camera moves only on the horizontal plane (Assuming you are working in 3D with default preferences).")]
    [SerializeField] private KeyCode flatMoveKey = KeyCode.LeftShift;

    [Header("Anchored Movement"), Tooltip("By default in scene-view, this is done by right-clicking for rotation or middle mouse clicking for up and down")]
    [SerializeField] private KeyCode anchoredMoveKey = KeyCode.Mouse2;
    [SerializeField] private KeyCode anchoredRotateKey = KeyCode.Mouse1;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI textMeshProUGUI;

    private MeteorShooter MeteorShooting;

    private Transform focusedObject; 
    private Vector3 focusOffset;

    private void Start()
    {
        SavePosAndRot();
        MeteorShooting = gameObject.GetComponent<MeteorShooter>();

        // Ensure the TextMeshProUGUI component's raycast target is disabled
        if (textMeshProUGUI != null)
        {
            textMeshProUGUI.raycastTarget = false;
        }
    }

    void Update()
    {
        if (!doFocus)
            return;

        if (cooldown > 0 && Input.GetKeyDown(KeyCode.Mouse0))
            FocusObject();
        if (Input.GetKeyDown(KeyCode.Mouse0))
            cooldown = doubleClickTime;

        if (Input.GetKey(firstUndoKey)) 
        {
            if (Input.GetKeyDown(secondUndoKey))
                GoBackToLastPosition();
        }

        cooldown -= Time.deltaTime;
    }

    private void LateUpdate()
    {
        Vector3 move = Vector3.zero;
        
        if (Input.GetKey(forwardKey))
            move += Vector3.forward * moveSpeed;
        if (Input.GetKey(backKey))
            move += Vector3.back * moveSpeed;
        if (Input.GetKey(leftKey))
            move += Vector3.left * moveSpeed;
        if (Input.GetKey(rightKey))
            move += Vector3.right * moveSpeed;

        if (Input.GetKey(flatMoveKey))
        {
            float origY = transform.position.y;
            transform.Translate(move);
            transform.position = new Vector3(transform.position.x, origY, transform.position.z);
            return;
        }

        float mouseMoveY = Input.GetAxis(mouseY);
        float mouseMoveX = Input.GetAxis(mouseX);

        if (Input.GetKey(anchoredMoveKey)) 
        {
            move += Vector3.up * mouseMoveY * -moveSpeed;
            move += Vector3.right * mouseMoveX * -moveSpeed;
        }

        if (Input.GetKey(anchoredRotateKey)) 
        {
            transform.RotateAround(transform.position, transform.right, mouseMoveY * -rotationSpeed);
            transform.RotateAround(transform.position, Vector3.up, mouseMoveX * rotationSpeed);
        }

        transform.Translate(move);

        float mouseScroll = Input.GetAxis(zoomAxis);
        transform.Translate(Vector3.forward * mouseScroll * zoomSpeed);

        if (focusedObject != null)
        {
            Vector3 targetPos = focusedObject.position;

            if (Input.GetKey(anchoredRotateKey))
            {
                Quaternion rotation = Quaternion.Euler(-mouseMoveY * -rotationSpeed, mouseMoveX * rotationSpeed, 0);
                focusOffset = rotation * focusOffset;
            }

            transform.position = targetPos + focusOffset;
            transform.LookAt(focusedObject);

            if (Input.GetKeyDown(KeyCode.P)) 
            {   
                MeteorShooting.enabled = !MeteorShooting.enabled;
            }

        }
    }

    public void FocusObject()
    {
        SavePosAndRot();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, focusLimit))
        {
            GameObject target = hit.collider.gameObject;
            Vector3 targetPos = target.transform.position;
            Vector3 targetSize = hit.collider.bounds.size;

            if (target.CompareTag("Celestial"))
            {
                focusOffset = GetOffset(targetPos, targetSize);
                transform.position = targetPos + focusOffset;
                transform.LookAt(target.transform);

                focusedObject = target.transform;
            }
        }
    }

    private void SavePosAndRot() 
    {
        prevRot = transform.rotation;
        prevPos = transform.position;
    }

    private void GoBackToLastPosition() 
    {
        transform.position = prevPos;
        transform.rotation = prevRot;
        focusedObject = null; 
        if (MeteorShooting.enabled)
        {
            MeteorShooting.enabled = !MeteorShooting.enabled;
        } 
    }

    private Vector3 GetOffset(Vector3 targetPos, Vector3 targetSize)
    {
        Vector3 dirToTarget = targetPos - transform.position;

        float focusDistance = Mathf.Max(targetSize.x, targetSize.z);
        focusDistance = Mathf.Clamp(focusDistance, minFocusDistance, focusDistance);

        return -dirToTarget.normalized * focusDistance;
    }
    
    
    public void FocusObject(GameObject target)
    {
        SavePosAndRot();

        if (target != null)
        {
            Vector3 targetPos = target.transform.position;
            Vector3 targetSize = target.GetComponent<Collider>().bounds.size;

            focusOffset = GetOffset(targetPos, targetSize);
            transform.position = targetPos + focusOffset;
            transform.LookAt(target.transform);

            focusedObject = target.transform;
        }
    }
}
