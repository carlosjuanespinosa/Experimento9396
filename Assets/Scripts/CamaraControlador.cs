using UnityEngine;

public class CamaraControlador : MonoBehaviour
{
     [System.Serializable] public class ZonaCamara
    {
        public Transform trigger;
        public Transform cameraPosition;
    }

    [Header("Zonas")]
    public ZonaCamara[] zones; // Asigna tus triggers y posiciones en el Inspector

    [Header("Config")]
    [SerializeField] private float moveSpeed = 3f;
    //[SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float sizeChangeSpeed = 2f;

    private Camera _mainCamera;
    private Transform _currentTarget;
    private float _targetSize;

    private void Start()
    {
        _mainCamera = Camera.main;
        if (_mainCamera != null) _targetSize = _mainCamera.orthographicSize;

        // Configura todos los triggers
        foreach (var zone in zones)
        {
            var boxCollider2D = zone.trigger.gameObject.AddComponent<BoxCollider2D>();
            boxCollider2D.isTrigger = true;
            var triggerHandler = zone.trigger.gameObject.AddComponent<CameraTriggerHandler>();
            triggerHandler.controller = this;
            triggerHandler.targetPosition = zone.cameraPosition;
        }
    }

    private void Update()
    {
        if (_currentTarget != null)
        {
            // Movimiento suavizado
            _mainCamera.transform.position = Vector3.Lerp(
                _mainCamera.transform.position,
                _currentTarget.position,
                moveSpeed * Time.deltaTime);

            // Tamaño de cámara (2D)
            _mainCamera.orthographicSize = Mathf.Lerp(
                _mainCamera.orthographicSize,
                _targetSize,
                sizeChangeSpeed * Time.deltaTime);
        }
    }

    public void SetNewTarget(Transform newTarget, float newSize)
    {
        _currentTarget = newTarget;
        _targetSize = newSize;
    }
}

public class CameraTriggerHandler : MonoBehaviour
{
    public CamaraControlador controller;
    public Transform targetPosition;
    public float cameraSize = 5f; // Tamaño de cámara para esta zona

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            controller.SetNewTarget(targetPosition, cameraSize);
        }
    }
    
}
  
