using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

public class GameMainView : MonoBehaviour
{
    [SerializeField] Transform fieldRoot;
    [SerializeField] FieldItemView fieldItemViewPrefab;

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
}
