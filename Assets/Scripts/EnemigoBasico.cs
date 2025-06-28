using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemigoBasico : MonoBehaviour
{
     [Header("Movimiento")]
    [SerializeField] private float velocidad = 2f;
    [SerializeField] private Transform puntoA;
    [SerializeField] private Transform puntoB;
    [SerializeField] private float distanciaDeteccion = 5f;

    [Header("Ataque")]
    [SerializeField] private GameObject proyectilEnemigo;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreDisparos = 2f;
    [FormerlySerializedAs("dañoPorContacto")] [SerializeField] private int damagePorContacto = 10;
    [FormerlySerializedAs("dañoProyectil")] [SerializeField] private int damageProyectil = 15;

    private Transform _objetivoActual;
    private Transform _jugador;
    private float _tiempoUltimoDisparo;
    private bool _jugadorDetectado;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _objetivoActual = puntoA;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _jugador = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Movimiento patrulla
        if (!_jugadorDetectado)
        {
            PatrolMovement();
        }

        // Detección del jugador
        DetectarJugador();

        // Rotar punto de disparo hacia el jugador
        if (_jugadorDetectado)
        {
            RotarHaciaJugador();
        }

        // Disparar si detecta al jugador
        if (_jugadorDetectado && Time.time > _tiempoUltimoDisparo + tiempoEntreDisparos)
        {
            Disparar();
            _tiempoUltimoDisparo = Time.time;
        }
    }

    private void PatrolMovement()
    {
        transform.position = Vector2.MoveTowards(transform.position, _objetivoActual.position, velocidad * Time.deltaTime);

        if (Vector2.Distance(transform.position, _objetivoActual.position) < 0.1f)
        {
            _objetivoActual = _objetivoActual == puntoA ? puntoB : puntoA;
            RotarEnDireccion(_objetivoActual == puntoA);
        }
    }

    private void RotarEnDireccion(bool mirandoIzquierda)
    {
        _spriteRenderer.flipX = mirandoIzquierda;
        
        // Rotar punto de disparo según la dirección
        if (mirandoIzquierda)
        {
            puntoDisparo.localPosition = new Vector3(-Mathf.Abs(puntoDisparo.localPosition.x), puntoDisparo.localPosition.y, 0);
        }
        else
        {
            puntoDisparo.localPosition = new Vector3(Mathf.Abs(puntoDisparo.localPosition.x), puntoDisparo.localPosition.y, 0);
        }
    }

    private void DetectarJugador()
    {
        float distanciaAlJugador = Vector2.Distance(transform.position, _jugador.position);
        _jugadorDetectado = distanciaAlJugador <= distanciaDeteccion;
    }

    private void RotarHaciaJugador()
    {
        // Determinar si el jugador está a la izquierda o derecha
        bool jugadorALaIzquierda = _jugador.position.x < transform.position.x;
        
        // Rotar sprite y punto de disparo
        RotarEnDireccion(jugadorALaIzquierda);
    }

    // ReSharper disable Unity.PerformanceAnalysis
    [Obsolete("Obsolete")]
    private void Disparar()
    {
        GameObject proyectil = Instantiate(proyectilEnemigo, puntoDisparo.position, Quaternion.identity);
        
        // Calcular dirección hacia el jugador
        Vector2 direccion = (_jugador.position - puntoDisparo.position).normalized;
        
        proyectil.GetComponent<Rigidbody2D>().velocity = direccion * velocidad * 1.5f;
        proyectil.GetComponent<ProyectilEnemigo>().SetDamage(damageProyectil);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<JugadorVida>().RecibirDaño(damagePorContacto);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);
    }
}
