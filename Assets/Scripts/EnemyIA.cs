using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Usar Delegates e IEnumerators para el listado de estados del enemigo.
public class EnemyIA : MonoBehaviour
{
    public float Health = 100;
    public float Speed = 30;
    public float Damage = 10;
    public float FireRate = 0.2f;
    public float FiringRadius = 1;
    public float StateEvaluationFrequency = 0.05f; // 1/20 de segundo, el ciclo de ejecución de los estados.
    public float TargetThreshold = 0.01f;
    public float AttackNoTargetLimit = 1.2f;
    public List<Transform> _FollowRoute;
    public List<AudioClip> ShootingClips;
    public Color PatrolColor;
    public Color AlertColor;
    public Color AttackColor;
    public GameObject _explosionPrefab;
    //Delegar la definicion de corrutinas sin parámetros mediante State()
    delegate IEnumerator State();
    //El cambio de estados se manejará por cada IEnumerator presente en esta clase
    private int _currentTarget = 0;
    [SerializeField]
    private State _state;
    private NavMeshAgent _agent;
    private MeshRenderer _meshRenderer;
    private Light _light;
    private float _rotationTime = 5.0f;
    private float _currentRotationTime;
    private float _timeBetweenShots;
    private AudioSource _enemySoundSource;
    private ParticleSystem _explosionParticles;
    void Awake()
    {
        _agent = GetComponentInChildren<NavMeshAgent>();
        _meshRenderer = GetComponentInChildren<MeshRenderer>();
        _light = GetComponentInChildren<Light>();
        _enemySoundSource = GetComponent<AudioSource>();
        _explosionParticles = Instantiate(_explosionPrefab).GetComponent<ParticleSystem>();
        _explosionParticles.gameObject.SetActive(false);
        _meshRenderer.material.color = PatrolColor;
        _currentRotationTime = 0f;
        _timeBetweenShots = 0f;
        _agent.speed = Speed;
    }

    IEnumerator Start()
    {
        _state = PatrolState;
        while (enabled)
        {
            yield return StartCoroutine(_state());
        }
    }

    //Rutina para deambular. Cuando esta lo suficientemente cerca al punto objetivo, calculará un nuevo punto objetivo.
    //Tambien revisará si el deambulante puede descansar y encuentra una banca en su recorrido.
    IEnumerator PatrolState()
    {
        //Debug.Log("Patrol State");
        if (_agent.remainingDistance < AttackNoTargetLimit)
        {
            if (_currentTarget == _FollowRoute.Count - 1)
            {
                _currentTarget = 0;
            }
            else
            {
                _currentTarget++;
            }
        }
        _agent.destination = _FollowRoute[_currentTarget].position;
        _light.color = PatrolColor;
        yield return StateEvaluationFrequency;
    }


    //Rutina para dirigirse a la banca. En contraste con WanderState, no va a cambiar de ubicación.
    //Una vez esté cerca, pasa al estado Idle.
    IEnumerator AlertState()
    {
        //Debug.Log("Alert State");

        _agent.isStopped = true;
        _light.color = AlertColor;
        transform.rotation *= Quaternion.Euler(0f, Time.deltaTime * 60.0f, 0f);
        if (_currentRotationTime > _rotationTime)
        {
            _currentRotationTime = 0;
            _agent.isStopped = false;
            _state = PatrolState;
        }
        else
        {
            _currentRotationTime += Time.deltaTime;
        }
        if (IsPlayerInSight())
        {
            _state = AttackState;
        }
        yield return StateEvaluationFrequency;
    }

    //Rutina para mantenerse en reposo dado un tiempo determinado. Una vez cumplido el tiempo, volverá a deambular.
    IEnumerator AttackState()
    {
        _timeBetweenShots += Time.deltaTime;
        //Debug.Log("Attack State");
        _light.color = AttackColor;
        if(_timeBetweenShots > AttackNoTargetLimit)
        {
            _state = AlertState;
        }
        yield return StateEvaluationFrequency;
    }

    //Metodo para ser invocado cada que el enemigo sufra un disparo.
    public void Hit(float damage, Vector3 source)
    {
        Health -= damage;
        if (Health <= 0)
        {
            ProcessDeath();
        }
        else
        {
            Vector3 lookDirection = source - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, new Vector3(lookDirection.x, 0, lookDirection.z));
            if (_state != AttackState)
            {
                _agent.isStopped = true;
                _timeBetweenShots = 0;
                _state = AttackState;
            }
        }
    }

    protected void ProcessDeath()
    {
        _explosionParticles.transform.position = transform.position;
        _explosionParticles.gameObject.SetActive(true);
        _explosionParticles.Play();
        _enemySoundSource.Play();
        Destroy(this.gameObject);
    }


    protected bool IsPlayerInSight()
    {
        RaycastHit hit;
        Vector3 origin = new Vector3(transform.position.x, 1f, transform.position.z);
        Debug.DrawRay(origin, transform.forward * FiringRadius, Color.cyan, 1f);
        if (Physics.Raycast(new Ray(origin, transform.forward * FiringRadius), out hit, FiringRadius))
        {
            //Debug.Log($"Raycast hit with {hit.collider.tag}:{hit.collider.name}");
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _state = AlertState;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && _state == AttackState)
        {
            //El enemigo va a seguir mirando al jugador...
            Vector3 lookDirection =
                other.transform.position - transform.position;
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, new Vector3(lookDirection.x, 0, lookDirection.z));
            //... pero solo le va a disparar si realmente lo ve. Esto si el jugador se esconde y no esta en el rango de vision del enemigo
            if (IsPlayerInSight())
            {
                if (_timeBetweenShots > FireRate)
                {
                    _timeBetweenShots = 0;
                    other.SendMessage("ReceiveHit", Damage);
                    PlayShootSound();
                }
            }
        }
    }

    protected void PlayShootSound()
    {
        if (ShootingClips.Count > 1)
        {
            int n = Random.Range(1, ShootingClips.Count);
            AudioClip toPlayClip = ShootingClips[n];
            _enemySoundSource.PlayOneShot(toPlayClip);
            ShootingClips[n] = ShootingClips[0];
            ShootingClips[0] = toPlayClip;
        }
        else
        {
            _enemySoundSource.PlayOneShot(ShootingClips[0]);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(_state == AttackState)
        {
            _state = AlertState;
        }
    }

}
