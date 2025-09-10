using UnityEngine;

/** Script para crear plataformas moviles sobre las cuales el jugador se pueda desplazar entre ellas.
 * Esteban.Hernandez
 */
public class MovingPlatform : MonoBehaviour
{
    public Transform startPoint; 
    public Transform endPoint; 
    public float speed = 1f; 
    public bool enableMovement = false;

    private Vector3 direction; 
    private Vector3 destination; 

    private void Start()
    {
        direction = (endPoint.position - startPoint.position).normalized;
        destination = endPoint.position;
    }

    private void Update()
    {
        if (enableMovement)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

            if (transform.position == destination)
            {
                destination = destination == endPoint.position ? startPoint.position : endPoint.position;
                direction = -direction;
            }
        }
    }

    public void EnablePlatform(bool isEnabled)
    {
        enableMovement = isEnabled;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(transform);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.SetParent(null);
        }
    }

}
