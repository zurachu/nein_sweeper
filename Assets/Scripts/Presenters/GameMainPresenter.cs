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
    [SerializeField] private List<Sprite> reverseSpritePreset;
    [SerializeField] private Sprite mineSprite;
    [SerializeField] private Sprite flagSprite;
    [SerializeField] private Sprite questionSprite;

    private List<Sprite> spritePreset;
    private FieldModel field;
    private TimerModel timer;
    private int scoreBoardNo;

    void Start()
    {
        view.Initialize(width, height,
            index => OnClickFieldItem(index % width, index / width),
            index => OnRightClickFieldItem(index % width, index / width));
        SubscribeTimer();
        ResetMode(0);
        view.OnResetModeAsObservable().Subscribe(i => ResetMode(i)).AddTo(this);
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

    private void ResetMode(int mode)
    {
        field = new FieldModel(width, height, mineCount);
        timer.Reset();

        spritePreset = mode switch
        {
            1 => normalSpritePreset,
            2 => ShuffleSpritePreset(),
            _ => reverseSpritePreset,
        };

        scoreBoardNo = mode switch
        {
            1 => 0,
            2 => 2,
            _ => 1,
        };

        UpdateView();
    }

    private void SubscribeTimer()
    {
        timer = new TimerModel();
        timer.RunningUpdateObservable().Subscribe(_ => timer.CurrentTime.Value += Time.deltaTime).AddTo(this);
        timer.CurrentTime.Subscribe(view.UpdateTimer).AddTo(this);
    }

    private List<Sprite> ShuffleSpritePreset()
    {
        // "0" 画像を含む逆プリセットを正順に戻してから、一つも同じ数値に対応しないようにシャッフルする
        var list = reverseSpritePreset.Reverse<Sprite>().ToList();
        for (var i = 0; i < list.Count - 1; i++)
        {
            var swapIndex = Random.Range(i + 1, list.Count);
            (list[i], list[swapIndex]) = (list[swapIndex], list[i]);
        }
        return list;
    }

    private void GameOver()
    {
        timer.Stop();
    }

    private void GameClear()
    {
        field.FlagAllMines();
        timer.Stop();
        if (scoreBoardNo > 0)
        {
            UnityroomApiClient.Instance.SendScore(scoreBoardNo, timer.CurrentTime.Value, ScoreboardWriteMode.HighScoreAsc);
        }
    }

    private void UpdateView()
    {
        view.UpdateView(new GameMainView.Parameter
        {
            isPlayable = !field.IsCompleted() && !field.IsMineOpened(),
            isCompleted = field.IsCompleted(),
            isPlaying = field.IsOpenedAny(),
            mineCount = field.NoFlagMineCount(),
            itemParameters = Enumerable.Range(0, height).SelectMany(y => Enumerable.Range(0, width).Select(x =>
            new FieldItemView.Parameter
            {
                sprite = field.IsMine(x, y) ? mineSprite : spritePreset[field.NearByMineCount(x, y)],
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
