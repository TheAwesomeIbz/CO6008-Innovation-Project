using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SCR_EquationRenderer : MonoBehaviour
{
    const int C_MaximumNodes = 100;
    [Header("LINE UI ELEMENTS")]
    [SerializeField] GraphType _graphType;
    [SerializeField] TextMeshProUGUI _equationText;
    [SerializeField] GameObject _equationPointPrefab;
    [field : SerializeField] public Transform ParentObject { get; private set; }
    [SerializeField] Transform _pointsParentObject;

    [Header("COEFFICIENTS")]
    [SerializeField] float[] _polynomialCoefficients;

    [Header("SINUSOIDAL EQUATIONS")]
    [SerializeField] bool _continuousValue;
    [SerializeField] [Range(-20, 20)] float _continuousValueSpeed;
    [SerializeField] float valueOffset;
    [SerializeField] float[] sinusoidalCoefficients;

    bool animatingFunction = false;
    

    [SerializeField] private float _globalGraphQuotient;
    [SerializeField] private float _pointQuotient = 20;
    private LineRenderer _lineRenderer;
    private EdgeCollider2D _edgeCollider;
    private List<Transform> _nodes;
    private List<SpriteRenderer> _nodeSprites;
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _nodes = new List<Transform>();
        _nodeSprites = new List<SpriteRenderer>();


        for (int i = 0; i < C_MaximumNodes; i++)
        {
            GameObject obj = Instantiate(_equationPointPrefab);
            obj.transform.parent = _pointsParentObject;
            _nodes.Add(obj.transform);
            _nodeSprites.Add(obj.GetComponent<SpriteRenderer>());
        }
        
        _edgeCollider.points = new Vector2[100];
        if (_continuousValueSpeed == 0) { _continuousValueSpeed = 1; }
        _lineRenderer.positionCount = _nodes.Count;
    }

    private void UpdateLineRenderer()
    {
        List<Vector2> edgePositions = new List<Vector2>();
        for (int i = 0; i < _nodes.Count; i++)
        {
            _nodeSprites[i].enabled = i % _pointQuotient == 0;
            _lineRenderer.SetPosition(i, _nodes[i].localPosition);
            edgePositions.Add(_nodes[i].localPosition);
            _edgeCollider.SetPoints(edgePositions);
        }

        _edgeCollider.SetPoints(edgePositions);

    }

    private void RenderGraph()
    {

        if (_globalGraphQuotient == 0) { return; }

        if (_pointQuotient == 0) { return; }
        switch (_graphType)
        {
            case GraphType.POLYNOMIAL:
                if (_polynomialCoefficients.Length == 0 || _polynomialCoefficients[0] == 0) { return; }

                _equationText.transform.position = (_polynomialCoefficients[0] < 0 ?
                    new Vector3(_equationText.transform.position.x, 1) :
                    new Vector3(_equationText.transform.position.x, -1));
                Quadratic();
                Cubic();
                Quartic();
                
                break;
        }

        if (sinusoidalCoefficients.Length == 0 || sinusoidalCoefficients[0] == 0) { return; }
        switch (_graphType)
        {
            case GraphType.SINE:
                Sine();
                break;
        }

    }

    private void Quadratic()
    {
        if (_polynomialCoefficients.Length != 3) { return; }
        for (int i = - C_MaximumNodes / 2; i < C_MaximumNodes / 2; i++)
        {
            float indexValue = i / _pointQuotient;
            float a = Mathf.Pow(indexValue, 2) * _polynomialCoefficients[0];
            float b = indexValue * _polynomialCoefficients[1];
            float c = _polynomialCoefficients[2];
            _nodes[i + (C_MaximumNodes / 2)].transform.localPosition = new Vector2(indexValue, (a + b + c) / _globalGraphQuotient);
        }
        _equationText.text = $"f(x) = {_polynomialCoefficients[0]}x² + {_polynomialCoefficients[1]}x + {_polynomialCoefficients[2]}";
    }

    private void Cubic()
    {
        if (_polynomialCoefficients.Length != 4) { return; }
        for (int i = - C_MaximumNodes / 2; i < C_MaximumNodes / 2; i++)
        {
            float indexValue = i / _pointQuotient;
            float a = Mathf.Pow(indexValue, 3) * _polynomialCoefficients[0];
            float b = Mathf.Pow(indexValue, 2) * _polynomialCoefficients[1];
            float c = indexValue * _polynomialCoefficients[2];
            float d = _polynomialCoefficients[3];
            _nodes[i + (C_MaximumNodes / 2)].transform.localPosition = new Vector2(indexValue, (a + b + c + d) / _globalGraphQuotient);
        }
        _equationText.text = $"f(x) = {_polynomialCoefficients[0]}x³ + {_polynomialCoefficients[1]}x² + {_polynomialCoefficients[2]}x + {_polynomialCoefficients[3]}";
    }

    private void Quartic()
    {
        if (_polynomialCoefficients.Length != 5) { return; }
        for (int i = - C_MaximumNodes / 2; i < C_MaximumNodes / 2; i++)
        {
            float indexValue = i / _pointQuotient;
            float a = Mathf.Pow(indexValue, 4) * _polynomialCoefficients[0];
            float b = Mathf.Pow(indexValue, 3) * _polynomialCoefficients[1];
            float c = Mathf.Pow(indexValue, 2) * _polynomialCoefficients[2];
            float d = indexValue * _polynomialCoefficients[3];
            float e = _polynomialCoefficients[4];
            _nodes[i + (C_MaximumNodes / 2)].transform.localPosition = new Vector2(indexValue, (a + b + c + d + e) / _globalGraphQuotient);
        }
        _equationText.text = $"f(x) = {_polynomialCoefficients[0]}x⁴ + {_polynomialCoefficients[1]}x³ + {_polynomialCoefficients[2]}x² + {_polynomialCoefficients[3]}x⁴ + {_polynomialCoefficients[4]}";
    }

    private void Sine()
    {
        if (sinusoidalCoefficients.Length != 2) { return; }

        if (_continuousValue)
        {
            valueOffset += Time.deltaTime * _continuousValueSpeed;
            if (valueOffset > Mathf.PI * 2) { valueOffset = 0; }
        }
        for (int i = 0; i < C_MaximumNodes; i++)
        {
            float indexValue = i / _pointQuotient;
            _nodes[i].transform.localPosition = new Vector2(indexValue, sinusoidalCoefficients[0] * Mathf.Sin(sinusoidalCoefficients[1] * indexValue + valueOffset) / _globalGraphQuotient);
        }
        _equationText.text = $"f(x) = {sinusoidalCoefficients[0]} X SIN({sinusoidalCoefficients[1]}x + {valueOffset / Mathf.PI}π) + {transform.position.y}";
    }



    public void SetPolynomialCoefficients(float[] polynomialCoefficients) => _polynomialCoefficients = polynomialCoefficients;
    public void SetGlobalPosition(Vector3 position) => ParentObject.transform.position = position;
    public void AnimateGraph(float speed = 1)
    {
        
        StopAllCoroutines();
        StartCoroutine(DisplayGraphCoroutine(speed));

    }

    public IEnumerator DisplayGraphCoroutine(float speed)
    {
        if (_lineRenderer == null) { _lineRenderer = GetComponent<LineRenderer>(); }
        _lineRenderer.positionCount = 0;
        animatingFunction = true;
        List<Vector2> edgePositions = new List<Vector2>();
        
        for (int i = 0; i < _nodeSprites.Count; i++)
        {
            _nodeSprites[i].enabled = false;
        }

        for (int i = 0; i < C_MaximumNodes; i++)
        {
            _lineRenderer.positionCount += 1;
            _nodeSprites[i].enabled = i % _pointQuotient == 0;
            _lineRenderer.SetPosition(i, _nodes[i].localPosition);
            edgePositions.Add(_nodes[i].localPosition);
            _edgeCollider.SetPoints(edgePositions);
            yield return new WaitForSeconds(0.01f / speed);
        }
        animatingFunction = false;
    }

    void Update()
    {
        RenderGraph();
        if (!animatingFunction) { UpdateLineRenderer(); }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //print(collision);
    }

    private enum GraphType
    {
        POLYNOMIAL,
        SINE,
        COSINE,
        TANGENT,
        SECANT,
        COSECANT,
        COTANGENT
    }
}
