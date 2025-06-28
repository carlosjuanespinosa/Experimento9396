using UnityEngine;

public class Proyectil : MonoBehaviour
{
    private float _damage;
    private bool _esCargado;
    
    public void SetDaño(float cantidad) => _damage = cantidad;
    public void SetEsCargado(bool cargado) => _esCargado = cargado;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemigo"))
        {
            other.GetComponent<VidaEnemigos>().RecibirDaño(_damage);
            Destroy(gameObject);
        }
        else if (_esCargado && other.CompareTag("Obstaculo"))
        {
            Destroy(other.gameObject); // Destruye obstáculos
            Destroy(gameObject);
        }
        else if (!other.CompareTag("Player") && !other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
