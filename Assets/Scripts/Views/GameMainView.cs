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
    [SerializeField] TMP_Dropdown modeDropdown;
    [SerializeField] GameObject touchGuard;
    [SerializeField] GameObject completedRoot;

    public class Parameter
    {
        public bool isPlayable;
        public bool isCompleted;
        public bool isPlaying;
        public int mineCount;
        public IEnumerable<FieldItemView.Parameter> itemParameters;
    }

    private List<FieldItemView> fieldItems;

    public void Initialize(int width, int height, Action<int> onClick, Action<int> onClickRight)
    {
        fieldItems = Enumerable.Range(0, width * height).Select(_ => Instantiate(fieldItemViewPrefab, fieldRoot)).ToList();
        foreach (var (item, index) in fieldItems.WithIndex())
        {
            item.OnButtonClickedAsObservable().Subscribe(u => onClick?.Invoke(index)).AddTo(this);
            item.OnRightButtonClickedAsObservable().Subscribe(e => onClickRight?.Invoke(index)).AddTo(this);
        }
    }

    public void UpdateView(Parameter parameter)
    {
        touchGuard.SetActive(!parameter.isPlayable);
        completedRoot.SetActive(parameter.isCompleted);
        modeDropdown.interactable = !parameter.isPlaying;
        mineCountLabel.text = parameter.mineCount.ToString(parameter.mineCount >= 0 ? "0000" : "000");
        foreach (var (itemParameter, index) in parameter.itemParameters.WithIndex())
        {
            fieldItems[index].UpdateView(itemParameter);
        }
    }

    public void UpdateTimer(float time)
    {
        timerLabel.text = Mathf.Clamp(time, 0, 999.999f).ToString("000.000");
    }

    public Observable<int> OnResetModeAsObservable()
    {
        return Observable.Merge(
            resetButton.OnClickAsObservable().Select(_ => modeDropdown.value),
            modeDropdown.OnValueChangedAsObservable());
    }
}
