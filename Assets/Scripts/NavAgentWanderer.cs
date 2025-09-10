using System.Collections;
using UnityEngine;
using UnityEngine.AI;

//** Clase para entidades deambulantes con estados de descanso
//*/
public class NavAgentWanderer : MonoBehaviour
{
    public Transform _Surface; //Piso en el cual mueve el agente deambulante
    public float _routeCalculationFrequency = 0.05f; // 1/20 de segundo, el ciclo de ejecución de los estados.
    public float _idleTime = 2.0f; // Tiempo en el que el Agente se va a manterner descansando.
    public float _radiusRef = 1f;
    public float _offsetRef = 1f;
    public float _proximityThreshold = 1f; //Sensibilidad sobre la proximidad a la que debe estar en su destino antes de cambiar a otra posición
    public float _timeForRest = 3.0f; //Que tanto tiempo debe esperar el wanderer para volver a descansar
    public NavMeshAgent _agent;
    private float _horizontalLimit;
    private float _verticalLimit;
    private float _lastTimeSinceIdle;
    private WaitForSeconds _wanderingUpdateTime;
    private WaitForSeconds _idleWaitTime;
    //Delegar la definicion de corrutinas sin parámetros mediante State()
    delegate IEnumerator State();
    //El cambio de estados se manejará por cada IEnumerator presente en esta clase
    private State _state;
    private MeshRenderer _meshRenderer;

    void Awake()
    {
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _lastTimeSinceIdle = Time.time;
        _meshRenderer.material.color = Color.green;
    }

    IEnumerator Start()
    {
        _wanderingUpdateTime = new WaitForSeconds(_routeCalculationFrequency);
        _idleWaitTime = new WaitForSeconds(_idleTime);
        yield return _wanderingUpdateTime;
        _state = WanderState;
        _horizontalLimit = _Surface.localScale.x;
        _verticalLimit = _Surface.localScale.z;
        while (enabled)
        {
            yield return StartCoroutine(_state());
        }
    }

    //Rutina para deambular. Cuando esta lo suficientemente cerca al punto objetivo, calculará un nuevo punto objetivo.
    //Tambien revisará si el deambulante puede descansar y encuentra una banca en su recorrido.
    IEnumerator WanderState()
    {
        if (_agent.remainingDistance < _proximityThreshold)
        {
            _agent.destination = CalculateNextRandomPosition();
        };
        //Evalua si el Deambulante ya puede volver a descansar si encuentra otra banca.
        bool shouldWandererRest = false;
        if (_lastTimeSinceIdle + _timeForRest < Time.time)
        {
            shouldWandererRest = true;
            _meshRenderer.material.color = Color.yellow;
        }
        ProcessWandererRest(shouldWandererRest);
        yield return _wanderingUpdateTime;
    }


    //Rutina para dirigirse a la banca. En contraste con WanderState, no va a cambiar de ubicación.
    //Una vez esté cerca, pasa al estado Idle.
    IEnumerator ToBenchState()
    {
        _meshRenderer.material.color = new Color(0.9f, 0.45f, 0.1f);
        if (_agent.remainingDistance < _proximityThreshold / 4)
        {
            _state = IdleState;
        };
        yield return _wanderingUpdateTime;
    }

    //Rutina para mantenerse en reposo dado un tiempo determinado. Una vez cumplido el tiempo, volverá a deambular.
    IEnumerator IdleState()
    {
        _meshRenderer.material.color = Color.red;
        _agent.isStopped = true;
        yield return _idleWaitTime;
        _lastTimeSinceIdle = Time.time;
        _meshRenderer.material.color = Color.green;
        _agent.destination = CalculateNextRandomPosition();
        _agent.isStopped = false;
        _state = WanderState;
    }

    private void ProcessWandererRest(bool shouldWandererRest)
    {
        Debug.DrawRay(transform.position, Vector3.forward, Color.cyan);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, 30, 0) * Vector3.forward, Color.cyan);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, -30, 0) * Vector3.forward, Color.cyan);
        GameObject forwardHit = CheckRaycastHitTag(transform.position, Vector3.forward);
        GameObject leftHit = CheckRaycastHitTag(transform.position, Quaternion.Euler(0, 30, 0) * Vector3.forward);
        GameObject rightHit = CheckRaycastHitTag(transform.position, Quaternion.Euler(0, -30, 0) * Vector3.forward);
        //Si en alguno de los raycast toca una de las bancas, el deambulante se dirige a la banca
        if (forwardHit && forwardHit.CompareTag("Player") && shouldWandererRest)
        {
            _agent.destination = forwardHit.transform.position;
            _state = ToBenchState;
        }
        else if (leftHit && leftHit.CompareTag("Player") && shouldWandererRest)
        {
            _agent.destination = leftHit.transform.position;
            _state = ToBenchState;
        }
        else if (rightHit && rightHit.CompareTag("Player") && shouldWandererRest)
        {
            _agent.destination = rightHit.transform.position;
            _state = ToBenchState;
        }
    }

    private GameObject CheckRaycastHitTag(Vector3 origin, Vector3 raycast)
    {
        if (Physics.Raycast(_agent.transform.position, raycast, out RaycastHit hit))
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private Vector3 CalculateNextRandomPosition()
    {

        Vector3 localTarget = Random.insideUnitCircle * _radiusRef;
        //Si el punto objetivo se sale de los límites del terreno de caminado, se modifica la posición objetivo.
        localTarget += new Vector3(0, 0, _offsetRef);
        Vector3 worldTarget = transform.TransformPoint(localTarget);
        if (worldTarget.x > _horizontalLimit * 0.5)
        {
            worldTarget.x -= _offsetRef;
        }
        if (worldTarget.x < _horizontalLimit * -0.5)
        {
            worldTarget.x += _offsetRef;
        }
        if (worldTarget.z > _verticalLimit * 0.5)
        {
            worldTarget.z -= _offsetRef;
        }
        if (worldTarget.z < _verticalLimit * -0.5)
        {
            worldTarget.z += _offsetRef;
        }
        worldTarget.y = 0f;
        return worldTarget;
    }
}
