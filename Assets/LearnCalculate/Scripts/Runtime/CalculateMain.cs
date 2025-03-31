using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CalculateMain : MonoBehaviour
{
    private Vector3 _randomPos;
    private const float ScreenOffset = 300;

    private readonly Vector3[] _corners = new Vector3[4]
    {
        new Vector3(0, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(1, 1, 0),
        new Vector3(1, 0, 0),
    };

    private readonly Vector3[] _worldCorners = new Vector3[4];

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnGUI()
    {
        if (!Camera.main)
        {
            return;
        }

        if (GUILayout.Button("生成随机点"))
        {
            float viewEdgeX = ScreenOffset / Screen.width;
            float viewEdgeY = ScreenOffset / Screen.height;
            Vector3 randomView = new Vector3(Random.Range(viewEdgeX, 1 - viewEdgeX),
                Random.Range(viewEdgeY, 1 - viewEdgeY), 0);
            Ray ray = Camera.main.ViewportPointToRay(randomView);
            float distance = -ray.origin.y / ray.direction.y;
            _randomPos = ray.origin + distance * ray.direction;
        }
    }

    private void OnDrawGizmos()
    {
        if (!Camera.main)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(_randomPos, 20);
        Gizmos.DrawLine(transform.position, _randomPos);

        Gizmos.color = Color.green;
        for (int i = 0; i < _corners.Length; i++)
        {
            Ray ray = Camera.main.ViewportPointToRay(_corners[i]);
            float distance = -ray.origin.y / ray.direction.y;
            _worldCorners[i] = ray.origin + distance * ray.direction;
            Gizmos.DrawLine(transform.position, _worldCorners[i]);
        }
        Gizmos.DrawLine(_worldCorners[0], _worldCorners[1]);
        Gizmos.DrawLine(_worldCorners[1], _worldCorners[2]);
        Gizmos.DrawLine(_worldCorners[2], _worldCorners[3]);
        Gizmos.DrawLine(_worldCorners[3], _worldCorners[0]);
    }
}