using R3;
using UnityEngine;
using UnityEngine.UI;

public class FieldItemView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Button button;

    public class Parameter
    {
        public Sprite sprite;
        public bool isOpened;
    }

    public void UpdateView(Parameter parameter)
    {
        image.sprite = parameter.sprite;
        button.gameObject.SetActive(!parameter.isOpened);
    }

    public Observable<Unit> OnButtonClickedAsObservable()
    {
        return button.OnClickAsObservable();
    }
}
