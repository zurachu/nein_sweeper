using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameMainView : MonoBehaviour
{
    [SerializeField] Transform fieldRoot;
    [SerializeField] FieldItemView fieldItemViewPrefab;
    [SerializeField] TextMeshProUGUI timerLabel;
    [SerializeField] TextMeshProUGUI mineCountLabel;
    [SerializeField] Button resetButton;

    private List<FieldItemView> fieldItems;

    public void Initialize(int width, int height, Action<int> onClick)
    {
        fieldItems = Enumerable.Range(0, width * height).Select(_ => Instantiate(fieldItemViewPrefab, fieldRoot)).ToList();
        foreach (var (item, index) in fieldItems.WithIndex())
        {
            item.OnButtonClickedAsObservable().Subscribe(u => onClick?.Invoke(index)).AddTo(this);
        }
    }

    public void UpdateView(IEnumerable<FieldItemView.Parameter> parameters)
    {
        foreach (var (parameter, index) in parameters.WithIndex())
        {
            fieldItems[index].UpdateView(parameter);
        }
    }

    public void UpdateTimer(float time)
    {
        timerLabel.text = Mathf.Clamp(time, 0, 999.999f).ToString("000.000");
    }

    public void UpdateMineCount(int count)
    {
        mineCountLabel.text = count.ToString("000");
    }

    public Observable<Unit> OnResetButtonClickedAsObservable()
    {
        return resetButton.OnClickAsObservable();
    }
}
