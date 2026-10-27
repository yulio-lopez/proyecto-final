using UnityEngine;

public class Bala : MonoBehaviour
{
    public int dano = 1;
    public float velocidad = 8;
    public float direccion = 1f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(this.gameObject, 10);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(direccion * velocidad, 0);
    }

    public void establecerDireccion(float nuevaDireccion)
    {
        direccion = nuevaDireccion;
        if (direccion > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direccion < 0)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }

    }

    private void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Enemigo"))
        {
            Destroy(gameObject);
        }
        if (otro.gameObject.layer == LayerMask.NameToLayer("capaSuelo"))
        {
            Destroy(gameObject);
        }
    }

}
