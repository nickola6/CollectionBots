using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using static UnityEditor.ObjectChangeEventStream;

public class ResourceScoreView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private ResourceScore _score;

    private readonly Dictionary<int, int> _values = new Dictionary<int, int>();

    private void OnEnable()
    {
        _score.ScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        _score.ScoreChanged -= OnScoreChanged;
        _values.Clear();

        if (_text != null)
            _text.text = string.Empty;
    }

    private void OnScoreChanged(int number, int value)
    {
        _values[number] = value;
        RefreshView();
    }

    private void RefreshView()
    {
        if (_text == null)
            return;

        _text.text = string.Empty;

        foreach (KeyValuePair<int, int> pair in _values)
            _text.text += $"Base {pair.Key}: {pair.Value}\n";
    }
}