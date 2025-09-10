using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//** Clase para definir una patrulla de circuito para un NavAgent, basado en waypoints
//*/
public class NavMeshPatroller : MonoBehaviour
{
    public List<Transform> _FollowRoute;
    public float _TargetThreshold;
    public float _minSpeed;
    public float _maxSpeed;
    private NavMeshAgent _Agent;
    [SerializeField]
    private int _CurrentTarget;
    [SerializeField]
    private bool _Clockwise;
    [SerializeField]
    public float _Speed;

    void Awake()
    {
        _Agent = GetComponentInParent<NavMeshAgent>();
        _CurrentTarget = 0;
        _Agent.speed = Random.Range(_minSpeed, _maxSpeed);
        _Clockwise = Random.Range(0.0f, 1.0f) > 0.5f;
        _Agent.updateRotation = true;
        _Speed = _Agent.speed;
        DrawPatrollingCircuit();
    }

    void Update()
    {
        if (_Agent.remainingDistance < _TargetThreshold)
        {
            if (_Clockwise)
            {
                if (_CurrentTarget == _FollowRoute.Count - 1)
                {
                    _CurrentTarget = 0;
                }
                else
                {
                    _CurrentTarget++;
                }
            }
            else
            {
                if (_CurrentTarget == 0)
                {
                    _CurrentTarget = _FollowRoute.Count - 1;
                }
                else
                {
                    _CurrentTarget--;
                }
            }
        }
        _Agent.destination = _FollowRoute[_CurrentTarget].position;
        DrawPatrollingCircuit();
    }

    private void DrawPatrollingCircuit()
    {
        for (int i = 0; i < _FollowRoute.Count - 1; i++)
        {
            Debug.DrawLine(_FollowRoute[i].position, _FollowRoute[i + 1].position, Color.red);
        }
        Debug.DrawLine(_FollowRoute[0].position, _FollowRoute[_FollowRoute.Count - 1].position, Color.red);
    }
}
