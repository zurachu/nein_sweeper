using System.Collections.Generic;
using System.Linq;
using R3;
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
    [SerializeField] private Sprite flagSprite;
    [SerializeField] private Sprite questionSprite;

    private FieldModel field;
    private TimerModel timer;

    void Start()
    {
        view.Initialize(width, height,
            index => OnClickFieldItem(index % width, index / width),
            index => OnRightClickFieldItem(index % width, index / width));
        SubscribeTimer();
        Reset();
        view.OnResetButtonClickedAsObservable().Subscribe(_ => Reset()).AddTo(this);
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
            timer.Run();
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

    private void OnRightClickFieldItem(int x, int y)
    {
        if (field.IsOpened(x, y))
        {
            return;
        }

        field.RotateMark(x, y);
        UpdateView();
    }

    private void Reset()
    {
        field = new FieldModel(width, height, mineCount);
        timer.Reset();
        UpdateView();
    }

    private void SubscribeTimer()
    {
        timer = new TimerModel();
        timer.RunningUpdateObservable().Subscribe(_ => timer.CurrentTime.Value += Time.deltaTime).AddTo(this);
        timer.CurrentTime.Subscribe(view.UpdateTimer).AddTo(this);
    }

    private void GameOver()
    {
        timer.Stop();
    }

    private void GameClear()
    {
        field.FlagAllMines();
        timer.Stop();
        UnityroomApiClient.Instance.SendScore(1, timer.CurrentTime.Value, ScoreboardWriteMode.HighScoreAsc);
    }

    private void UpdateView()
    {
        view.UpdateView(new GameMainView.Parameter
        {
            isPlayable = !field.IsCompleted() && !field.IsMineOpened(),
            mineCount = field.NoFlagMineCount(),
            itemParameters = Enumerable.Range(0, height).SelectMany(y => Enumerable.Range(0, width).Select(x =>
            new FieldItemView.Parameter
            {
                sprite = field.IsMine(x, y) ? mineSprite : normalSpritePreset[field.NearByMineCount(x, y)],
                markSprite = MarkSprite(x, y),
                isOpened = field.IsOpened(x, y),
            })),
        });
    }

    private Sprite MarkSprite(int x, int y)
    {
        return field.GetMark(x, y) switch
        {
            Mark.Flag => flagSprite,
            Mark.Question => questionSprite,
            _ => null,
        };
    }
}
