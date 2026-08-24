using System.Collections.Generic;
using UnityEngine;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;

    [SerializeField]
    private GameObject ballPrefab;

    [SerializeField]
    private float ballY = 0.425f;

    [SerializeField]
    private float baulkZ = -17f;

    [SerializeField]
    private float dRadius = 4.4f;

    [SerializeField]
    private float redApexZ = 15.4f;

    [SerializeField]
    private float redGapX = 0.88f;

    [SerializeField]
    private float redGapZ = 0.77f;

    [SerializeField]
    private int foulPoint = 4;

    [SerializeField]
    private float minShotTime = 0.4f;

    [SerializeField]
    private float maxShotTime = 25f;

    [SerializeField]
    private int[] playerScore = new int[2];

    private List<Ball> balls = new List<Ball>();
    private List<Ball> potted = new List<Ball>();
    private Ball cueBall;
    private Ball firstHit;

    private int turn;
    private bool needRed = true;
    private bool shotRunning;
    private bool gameOver;
    private float shotTimer;

    public int PlayerScore { get { return playerScore[turn]; } set { playerScore[turn] = value; } }
    public int Turn { get { return turn; } }
    public bool NeedRed { get { return needRed; } }
    public bool CanShoot { get { return !shotRunning && !gameOver; } }
    public Ball CueBall { get { return cueBall; } }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SaveData save = SaveSystem.LoadOnNextStart ? SaveSystem.Load() : null;
        SaveSystem.LoadOnNextStart = false;

        if (save != null)
            RestoreFrom(save);
        else
            SetAllBalls();

        ShowState();
    }

    void Update()
    {
        if (!shotRunning)
            return;

        shotTimer += Time.deltaTime;

        if (shotTimer < minShotTime)
            return;

        if (shotTimer < maxShotTime && BallsMoving())
            return;

        EndShot();
    }

    private void SetAllBalls()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("Ball Prefab is empty", this);
            return;
        }

        cueBall = SpawnBall(BallColor.White, ColorSpot(BallColor.White));
        SpawnBall(BallColor.Yellow, ColorSpot(BallColor.Yellow));
        SpawnBall(BallColor.Green, ColorSpot(BallColor.Green));
        SpawnBall(BallColor.Brown, ColorSpot(BallColor.Brown));
        SpawnBall(BallColor.Blue, ColorSpot(BallColor.Blue));
        SpawnBall(BallColor.Pink, ColorSpot(BallColor.Pink));
        SpawnBall(BallColor.Black, ColorSpot(BallColor.Black));
        SpawnReds();
    }

    /// <summary>Where a named colour racks and where it goes back to when potted.</summary>
    private Vector3 ColorSpot(BallColor col)
    {
        switch (col)
        {
            case BallColor.White: return new Vector3(-2f, ballY, baulkZ - 3f);
            case BallColor.Yellow: return new Vector3(dRadius, ballY, baulkZ);
            case BallColor.Green: return new Vector3(-dRadius, ballY, baulkZ);
            case BallColor.Brown: return new Vector3(0f, ballY, baulkZ);
            case BallColor.Blue: return new Vector3(0f, ballY, 0f);
            case BallColor.Pink: return new Vector3(0f, ballY, 14.5f);
            case BallColor.Black: return new Vector3(0f, ballY, 23.7f);
            default: return Vector3.zero;
        }
    }

    /// <summary>Writes both scores, the turn and every remaining ball to the save slot.</summary>
    public void SaveGame()
    {
        SaveData data = new SaveData
        {
            score0 = playerScore[0],
            score1 = playerScore[1],
            turn = turn,
            needRed = needRed,
        };

        for (int i = 0; i < balls.Count; i++)
        {
            Ball ball = balls[i];
            if (ball.gameObject.activeSelf)
                data.balls.Add(BallState.From(ball.ColorType, ball.transform.position));
        }

        SaveSystem.Save(data);
    }

    /// <summary>
    /// Rebuilds the table from a save instead of racking a fresh frame. Reds
    /// come back exactly where they sat; colours come back at that position too,
    /// but their respot point is reset to the canonical spot rather than
    /// wherever they happened to be sitting when the game was saved.
    /// </summary>
    private void RestoreFrom(SaveData save)
    {
        playerScore[0] = save.score0;
        playerScore[1] = save.score1;
        turn = save.turn;
        needRed = save.needRed;

        foreach (BallState state in save.balls)
        {
            Ball ball = SpawnBall(state.Colour, state.Position);

            if (state.Colour == BallColor.White)
            {
                cueBall = ball;
                ball.SetSpot(ColorSpot(BallColor.White));
            }
            else if (state.Colour != BallColor.Red)
            {
                ball.SetSpot(ColorSpot(state.Colour));
            }
        }

        Debug.Log($"[GameManger] Restored save: P1 {playerScore[0]} - P2 {playerScore[1]}, " +
                  $"{balls.Count} balls left");
    }

    private void SpawnReds()
    {
        for (int row = 0; row < 5; row++)
        {
            for (int i = 0; i <= row; i++)
            {
                float x = (i - row * 0.5f) * redGapX;
                float z = redApexZ + row * redGapZ;
                SpawnBall(BallColor.Red, new Vector3(x, ballY, z));
            }
        }
    }

    private Ball SpawnBall(BallColor col, Vector3 pos)
    {
        GameObject obj = Instantiate(ballPrefab, pos, Quaternion.identity);
        Ball ball = obj.GetComponent<Ball>();
        ball.SetColorAndPoint(col);
        ball.SetSpot(pos);
        balls.Add(ball);
        return ball;
    }

    public void Shoot(Vector3 dir, float power)
    {
        if (!CanShoot)
            return;

        firstHit = null;
        potted.Clear();
        cueBall.Shoot(dir, power);
        shotRunning = true;

        if (AudioManager.instance != null)
            AudioManager.instance.PlayCueHit();
        shotTimer = 0f;
    }

    public void ReportFirstHit(Ball ball)
    {
        if (firstHit == null)
            firstHit = ball;
    }

    public void BallPotted(Ball ball)
    {
        if (potted.Contains(ball))
            return;

        potted.Add(ball);
        ball.Stop();

        if (AudioManager.instance != null)
            AudioManager.instance.PlayPot();
        ball.gameObject.SetActive(false);
    }

    private bool BallsMoving()
    {
        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].gameObject.activeSelf && balls[i].IsMoving)
                return true;
        }

        return false;
    }

    private void EndShot()
    {
        shotRunning = false;

        bool foul = false;
        bool pottedRed = false;
        bool pottedColor = false;
        int gain = 0;

        if (firstHit == null)
            foul = true;
        else if (needRed && firstHit.ColorType != BallColor.Red)
            foul = true;
        else if (!needRed && firstHit.ColorType == BallColor.Red)
            foul = true;

        for (int i = 0; i < potted.Count; i++)
        {
            Ball ball = potted[i];

            if (ball.ColorType == BallColor.White)
            {
                foul = true;
                continue;
            }

            if (ball.ColorType == BallColor.Red)
                pottedRed = true;
            else
                pottedColor = true;

            gain += ball.Point;
        }

        if (needRed && pottedColor)
            foul = true;

        if (!needRed && pottedRed)
            foul = true;

        if (foul)
            DoFoul();
        else if (potted.Count > 0)
            DoPot(gain, pottedRed);
        else
            DoMiss();

        potted.Clear();
        firstHit = null;

        if (RedLeft() == 0 && ColorLeft() == 0)
            EndGame();
        else
            ShowState();
    }

    private void DoFoul()
    {
        RespotAll();

        if (potted.Contains(cueBall))
            cueBall.BackToSpot();

        playerScore[1 - turn] += foulPoint;
        needRed = RedLeft() > 0;
        SwitchTurn();

        if (AudioManager.instance != null)
            AudioManager.instance.PlayFoul();

        if (UIManager.instance != null)
            UIManager.instance.ShowMessage("Foul +" + foulPoint);
    }

    private void DoPot(int gain, bool pottedRed)
    {
        playerScore[turn] += gain;

        if (pottedRed)
        {
            needRed = false;
        }
        else
        {
            if (RedLeft() > 0)
                RespotColors();

            needRed = RedLeft() > 0;
        }

        if (UIManager.instance != null)
            UIManager.instance.ShowMessage("Pot +" + gain);
    }

    private void DoMiss()
    {
        needRed = RedLeft() > 0;
        SwitchTurn();

        if (UIManager.instance != null)
            UIManager.instance.ShowMessage("Miss");
    }

    private void RespotColors()
    {
        for (int i = 0; i < potted.Count; i++)
        {
            Ball ball = potted[i];

            if (ball.ColorType != BallColor.Red && ball.ColorType != BallColor.White)
                ball.BackToSpot();
        }
    }

    private void RespotAll()
    {
        for (int i = 0; i < potted.Count; i++)
        {
            Ball ball = potted[i];

            if (ball.ColorType == BallColor.Red)
                continue;

            if (ball.ColorType == BallColor.White)
                continue;

            if (RedLeft() > 0)
                ball.BackToSpot();
        }
    }

    private int RedLeft()
    {
        int count = 0;

        for (int i = 0; i < balls.Count; i++)
        {
            if (balls[i].gameObject.activeSelf && balls[i].ColorType == BallColor.Red)
                count++;
        }

        return count;
    }

    private int ColorLeft()
    {
        int count = 0;

        for (int i = 0; i < balls.Count; i++)
        {
            Ball ball = balls[i];

            if (!ball.gameObject.activeSelf)
                continue;

            if (ball.ColorType != BallColor.Red && ball.ColorType != BallColor.White)
                count++;
        }

        return count;
    }

    private void SwitchTurn()
    {
        turn = 1 - turn;
    }

    private void ShowState()
    {
        if (UIManager.instance == null)
            return;

        UIManager.instance.ShowScore(playerScore[0], playerScore[1]);
        UIManager.instance.ShowTurn(turn, needRed);
    }

    private void EndGame()
    {
        gameOver = true;
        ShowState();

        if (AudioManager.instance != null)
            AudioManager.instance.PlayWin();

        if (UIManager.instance == null)
            return;

        if (playerScore[0] > playerScore[1])
            UIManager.instance.ShowGameOver("Player 1 Win");
        else if (playerScore[1] > playerScore[0])
            UIManager.instance.ShowGameOver("Player 2 Win");
        else
            UIManager.instance.ShowGameOver("Draw");
    }
}
