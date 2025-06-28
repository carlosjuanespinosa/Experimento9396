

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class MovimientosJugador : MonoBehaviour
{
  [Header("Movimiento")]
    [SerializeField] private float velocidad = 5f;
    [SerializeField] private float fuerzaSalto = 36f;
    [SerializeField] private float friccionSuelo = 0.2f;
    [SerializeField] private float friccionAire = 0.5f;
    
    [Header("Ataque")]
    [SerializeField] private GameObject proyectilNormalPrefab;
    [SerializeField] private GameObject proyectilCargadoPrefab;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float damageProyectil = 10f;
    [SerializeField] private float damageProyectilCargado = 25f;
    [SerializeField] private float velocidadProyectil = 15f;
    [SerializeField] private float cooldownAtaque = 0.5f;
    [SerializeField] private float tiempoCargaDisparo = 1.5f;
    
    [Header("Dash")]
    [SerializeField] private float duracionDash = 0.2f;
    [SerializeField] private float velocidadDash = 20f;
    [SerializeField] private float cooldownDash = 1f;
    
    // Estado del jugador
    private Vector2 _direccion;
    private bool _saltando;
    private bool _puedeDobleSaltar;
    private bool _enDash;
    private bool _disparoCargando;
    private float _ultimoAtaqueTime;
    private float _tiempoCargaActual;
    private float _ultimoDashTime;
    private bool _enSuelo;
    
    // Componentes
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private CapsuleCollider2D _collider;
    
    // Habilidades
    private bool _dobleSaltoDesbloqueado;
    private bool _dashDesbloqueado;
    private bool _disparoCargadoDesbloqueado;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        // Rotar punto de disparo
        if (_direccion.x != 0)
        {
            bool mirandoDerecha = _direccion.x > 0;
            _spriteRenderer.flipX = !mirandoDerecha;
            
            puntoDisparo.localPosition = new Vector3(
                mirandoDerecha ? Mathf.Abs(puntoDisparo.localPosition.x) : -Mathf.Abs(puntoDisparo.localPosition.x),
                puntoDisparo.localPosition.y,
                puntoDisparo.localPosition.z
            );
        }
        
        // Carga del disparo
        if (_disparoCargando)
        {
            _tiempoCargaActual += Time.deltaTime;
        }
        
        // Verificar si está en el suelo
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        if (_enDash) return;
        
        // Movimiento horizontal con física mejorada
        float targetVelocityX = _direccion.x * velocidad;
        float velocityX = Mathf.Lerp(_rb.linearVelocity.x, targetVelocityX, _enSuelo ? friccionSuelo : friccionAire);
        
        _rb.linearVelocity = new Vector2(velocityX, _rb.linearVelocity.y);
    }

    private void CheckGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position, 
            Vector2.down, 
            _collider.bounds.extents.y + 0.1f, 
            LayerMask.GetMask("Suelo"));
        
        _enSuelo = hit.collider != null;
        
        if (_enSuelo && _rb.linearVelocity.y <= 0)
        {
            _saltando = false;
            _puedeDobleSaltar = _dobleSaltoDesbloqueado;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _direccion = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_enSuelo)
            {
                Saltar();
            }
            else if (_puedeDobleSaltar && _dobleSaltoDesbloqueado)
            {
                Saltar();
                _puedeDobleSaltar = false;
            }
        }
    }
    
    private void Saltar()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        _rb.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
        _saltando = true;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && _dashDesbloqueado && !_enDash && Time.time > _ultimoDashTime + cooldownDash)
        {
            StartCoroutine(Dash());
            _ultimoDashTime = Time.time;
        }
    }

    private System.Collections.IEnumerator Dash()
    {
        _enDash = true;
        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0;
        
        Vector2 dashDirection = _spriteRenderer.flipX ? Vector2.left : Vector2.right;
        _rb.linearVelocity = dashDirection * velocidadDash;
        
        yield return new WaitForSeconds(duracionDash);
        
        _rb.gravityScale = originalGravity;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x * 0.5f, _rb.linearVelocity.y);
        _enDash = false;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (_disparoCargadoDesbloqueado)
        {
            if (context.started)
            {
                _disparoCargando = true;
                _tiempoCargaActual = 0f;
            }
            else if (context.canceled && _disparoCargando)
            {
                _disparoCargando = false;
                Disparar(_tiempoCargaActual >= tiempoCargaDisparo);
            }
        }
        else if (context.performed && Time.time > _ultimoAtaqueTime + cooldownAtaque)
        {
            Disparar(false);
        }
    }

    private void Disparar(bool cargado)
    {
        _ultimoAtaqueTime = Time.time;
        
        GameObject proyectilPrefab = cargado ? proyectilCargadoPrefab : proyectilNormalPrefab;
        float damage = cargado ? damageProyectilCargado : damageProyectil;
        
        Vector2 direccion = _spriteRenderer.flipX ? Vector2.left : Vector2.right;
        
        GameObject proyectil = Instantiate(proyectilPrefab, puntoDisparo.position, Quaternion.identity);
        proyectil.GetComponent<Rigidbody2D>().linearVelocity = direccion * velocidadProyectil;
        proyectil.GetComponent<Proyectil>().SetDaño(damage);
        proyectil.GetComponent<Proyectil>().SetEsCargado(cargado);
    }

    // Métodos para desbloquear habilidades
    public void DesbloquearDobleSalto() => _dobleSaltoDesbloqueado = true;
    public void DesbloquearDash() => _dashDesbloqueado = true;
    public void DesbloquearDisparoCargado() => _disparoCargadoDesbloqueado = true;
}
