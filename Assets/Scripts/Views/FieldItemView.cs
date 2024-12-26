using R3;
using R3.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldItemView : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Image markImage;
    [SerializeField] private Button button;

    public class Parameter
    {
        public Sprite sprite;
        public Sprite markSprite;
        public bool isOpened;
    }

    public void UpdateView(Parameter parameter)
    {
        image.sprite = parameter.sprite;
        markImage.sprite = parameter.markSprite;
        markImage.gameObject.SetActive(markImage.sprite != null);
        button.gameObject.SetActive(!parameter.isOpened);
    }

    public Observable<Unit> OnButtonClickedAsObservable()
    {
        return button.OnClickAsObservable();
    }

    public Observable<PointerEventData> OnRightButtonClickedAsObservable()
    {
        return button.OnPointerClickAsObservable().Where(eventData => eventData.button == PointerEventData.InputButton.Right);
    }
}
