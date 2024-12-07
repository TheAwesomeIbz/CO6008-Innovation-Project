using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_LineRenderer : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCollider;
    [SerializeField] private Transform[] _vertices;

    List<Vector2> edges = new List<Vector2>();
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _lineRenderer.positionCount = _vertices.Length;
    }

    private void LineRendererUpdate()
    {
        for (int i = 0; i < _vertices.Length; i++)
        {
            _lineRenderer.SetPosition(i, _vertices[i].position);
        }
    }

    private void EdgeRendererUpdate()
    {
        edges = new List<Vector2>();
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            edges.Add(_lineRenderer.GetPosition(i));
        }
        _edgeCollider.SetPoints(edges);

    }
    void Update()
    {
        LineRendererUpdate();
        EdgeRendererUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print(collision);
    }
}
