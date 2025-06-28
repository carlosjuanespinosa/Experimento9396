using UnityEngine;

public class ProyectilEnemigo : MonoBehaviour
{
    private int _damage;

    public void SetDamage(int cantidad)
    {
        _damage = cantidad;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<JugadorVida>().RecibirDaño(_damage);
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
