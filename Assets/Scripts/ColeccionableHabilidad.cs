using UnityEngine;

public class ColeccionableHabilidad : MonoBehaviour
{
 public enum TipoHabilidad { DobleSalto, Dash, DisparoCargado }
    public TipoHabilidad habilidad;

    [Header("Efectos")]
    [SerializeField] private ParticleSystem efectoRecoleccion;
    [SerializeField] private AudioClip sonidoRecoleccion;
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            MovimientosJugador jugador = other.GetComponent<MovimientosJugador>();
            
            if (jugador == null)
            {
                Debug.LogError("MovimientosJugador no encontrado. Buscando en padres...");
                jugador = other.GetComponentInParent<MovimientosJugador>();
            }

            if (jugador != null)
            {
                switch (habilidad)
                {
                    case TipoHabilidad.DobleSalto:
                        jugador.DesbloquearDobleSalto();
                        break;
                    case TipoHabilidad.Dash:
                        jugador.DesbloquearDash();
                        break;
                    case TipoHabilidad.DisparoCargado:
                        jugador.DesbloquearDisparoCargado();
                        break;
                }

                // Efectos
                if (efectoRecoleccion != null)
                    Instantiate(efectoRecoleccion, transform.position, Quaternion.identity);

                if (sonidoRecoleccion != null)
                    AudioSource.PlayClipAtPoint(sonidoRecoleccion, transform.position);

                Destroy(gameObject);
            }
        }
    }
    
}


