using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using unityroom.Api;

public class GameMainPresenter : MonoBehaviour
{
    [SerializeField] private GameMainView view;
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int mineCount;
    [SerializeField] private List<Sprite> normalSpritePreset;
    [SerializeField] private Sprite mineSprite;

    private FieldModel field;

    void Start()
    {
        view.Initialize(width, height, index => OnClickFieldItem(index % width, index / width));
        Reset();
    }

    void Update()
    {

    }

    private void OnClickFieldItem(int x, int y)
    {
        if (!field.CanOpen(x, y))
        {
            return;
        }

        if (!field.IsOpenedAny())
        {
            field.ResetRandomMines(x, y);
            // TODO: Fix mode
        }

        field.Open(x, y);

        if (field.IsMine(x, y))
        {
            GameOver();
        }
        else if (field.IsCompleted())
        {
            GameClear();
        }

        UpdateView();
    }

    private void Reset()
    {
        field = new FieldModel(width, height, mineCount);
        UpdateView();
    }

    private void GameOver()
    {

    }

    private void GameClear()
    {
        field.FlagAllMines();
        UnityroomApiClient.Instance.SendScore(1, 123.45f, ScoreboardWriteMode.HighScoreAsc);
    }

    private void UpdateView()
    {
        view.UpdateView(Enumerable.Range(0, height).SelectMany(y => Enumerable.Range(0, width).Select(x =>
            new FieldItemView.Parameter
            {
                sprite = field.IsMine(x, y) ? mineSprite : normalSpritePreset[field.NearByMineCount(x, y)],
                isOpened = field.IsOpened(x, y),
            })));
    }
}
