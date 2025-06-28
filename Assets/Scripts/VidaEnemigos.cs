using UnityEngine;

public class VidaEnemigos : MonoBehaviour
{
    [SerializeField] private float vidaMaxima = 30f;
    private float _vidaActual;

    private void Start() => _vidaActual = vidaMaxima;

    public void RecibirDaño(float cantidad)
    {
        _vidaActual -= cantidad;
        Debug.Log($"{gameObject.name} recibe {cantidad} de daño. Vida restante: {_vidaActual}");
        
        if (_vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        Debug.Log($"{gameObject.name} muerto!");
        Destroy(gameObject);
    }
}
