using Unity.VisualScripting;
using UnityEngine;

public class ColorOverPlayerPos : MonoBehaviour
{
    private GameObject _player;
    private ParticleSystem _ps;
    private float _distance;
    private float _t;
    private Color _color;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _ps = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (_player != null)
        {
            UpdateColor();
            Debug.Log($"Distance: {_distance}, Color: {_color}");
        }
    }

    private void UpdateColor()
    {
        _distance = Vector3.Distance(transform.position, _player.transform.position);
        _t = _distance / 50f;
        _color = new Color(1f, _t, _t, 1f);

        if (_ps == null)
            _ps = GetComponent<ParticleSystem>();

        if (_ps != null)
        {
            var main = _ps.main;
            main.startColor = _color;
        }
    }
}
