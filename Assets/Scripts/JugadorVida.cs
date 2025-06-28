
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class JugadorVida : MonoBehaviour
{
    [SerializeField] private int vidaMaxima = 100;
    [SerializeField] private float tiempoInvencibilidad = 1f;
    [SerializeField] private AudioClip sonidoDaño;
    
    private int _vidaActual;
    private bool _esInvencible;
    private Vector3 _ultimoCheckpoint;
    private AudioSource _audioSource;

    private void Start()
    {
        _vidaActual = vidaMaxima;
        _ultimoCheckpoint = transform.position;
        _audioSource = GetComponent<AudioSource>();
    }

    public void RecibirDaño(int cantidad)
    {
        if (_esInvencible) return;

        _vidaActual -= cantidad;
        Debug.Log($"Vida restante: {_vidaActual}");

        if (sonidoDaño != null)
        {
            _audioSource.PlayOneShot(sonidoDaño);
        }

        if (_vidaActual <= 0)
        {
            Respawn();
        }
        else
        {
            StartCoroutine(ActivarInvencibilidad());
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private System.Collections.IEnumerator ActivarInvencibilidad()
    {
        _esInvencible = true;
        GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
        yield return new WaitForSeconds(tiempoInvencibilidad);
        GetComponent<SpriteRenderer>().color = Color.white;
        _esInvencible = false;
    }

    public void EstablecerCheckpoint(Vector3 posicion)
    {
        _ultimoCheckpoint = posicion;
        Debug.Log($"Checkpoint establecido en: {posicion}");
    }

    private void Respawn()
    {
        // Restaurar vida
        _vidaActual = vidaMaxima;
        
        // Mover al checkpoint
        transform.position = _ultimoCheckpoint;
        
        // Restaurar efectos visuales
        StopAllCoroutines();
        GetComponent<SpriteRenderer>().color = Color.white;
        _esInvencible = false;
    }
}

