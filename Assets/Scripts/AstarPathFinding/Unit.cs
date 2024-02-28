using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private bool displayPath;

    private float speed = 1;
    private Vector3[] path;
    private int targetIndex = 0;
    private float stoppingDistance;
    protected Vector3 CurrentDirection { get; private set; }

    protected void MoveToPosition(Vector3 _targetPosition, float _stoppingDistance = 0.1f)
    {
        stoppingDistance = _stoppingDistance;
        PathRequestManager.RequestPath(transform.parent.position, _targetPosition, OnPathFound);
    }

    private void OnPathFound(Vector3[] _newPath, bool pathSucessful)
    {
        if (pathSucessful)
        {
            path = _newPath;
            if (path.Length != 0)
            {
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
            else 
            {
                Debug.Log("Already reached target");
            }
        }
        else
        {
            Debug.Log("No path found");
            OnPathNotFound();
        }
    }

    private IEnumerator FollowPath()
    {
        Vector3 currentWayPoint = path[0];
        targetIndex = 0;
        while (true)
        {
            float distance = Vector3.Distance(transform.parent.position, currentWayPoint);
            if (distance <= stoppingDistance)
            {
                ++targetIndex;
                if (targetIndex >= path.Length)
                {                   
                    OnArrive();
                    yield break;
                }
                currentWayPoint = path[targetIndex];
            }
            MoveToWavepoint(currentWayPoint);
            yield return null;
        }
    }

    private void MoveToWavepoint(Vector3 _wayPoint)
    {
        CurrentDirection = (_wayPoint - transform.parent.position);
        CurrentDirection.Normalize();
        OnMove();
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, _wayPoint, speed * Time.deltaTime);
    }

    protected virtual void OnMove()
    {
        // ...
    }     

    protected virtual void OnArrive()
    {
        // ...
    }

    protected virtual void OnPathNotFound()
    {
        // ...
    }

    private void OnDrawGizmos()
    {
        if (path != null && displayPath)
        {
            for (int i = targetIndex; i < path.Length; ++i)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], new Vector3(0.8f, 0.8f, 0.8f));

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.parent.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }
}
