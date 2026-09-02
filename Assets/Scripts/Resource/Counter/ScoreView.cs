using TMPro;
using UnityEngine;

public class ScoreView : MonoBehaviour
{
    [SerializeField] private ResourceCounter _resourceCounter;
    [SerializeField] private TMP_Text _text;

    private void OnEnable()
    {
        _resourceCounter.CountChanged += OnCountChanged;
        OnCountChanged(_resourceCounter.Count);
    }

    private void OnDisable()
    {
        _resourceCounter.CountChanged -= OnCountChanged;
    }

    private void OnCountChanged(int count)
    {
        _text.text = count.ToString();
    }
}