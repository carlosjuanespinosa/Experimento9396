using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private ParticleSystem efectoActivacion;
    [SerializeField] private Sprite spriteActivado;
    private bool _estaActivado;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !_estaActivado)
        {
            _estaActivado = true;
            other.GetComponent<JugadorVida>().EstablecerCheckpoint(transform.position);
            
            if (efectoActivacion != null)
                efectoActivacion.Play();
            
            if (spriteActivado != null)
                _spriteRenderer.sprite = spriteActivado;
        }
    }
}

