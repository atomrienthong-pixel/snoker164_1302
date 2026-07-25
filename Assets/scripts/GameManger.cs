using UnityEngine;

public class GameManger : MonoBehaviour
{
    [SerializeField]
    private int playerscore;
    public int PlayerScore { get { return playerscore; } set { playerscore = value; } }

    public static GameManger instance;


    [SerializeField]
    private GameObject[] ballPositions;

    [SerializeField]
    private GameObject ballPrefab;

    // สีของลูกแต่ละใบ เรียงตาม index เดียวกับ ballPositions
    [SerializeField]
    private BallColor[] ballOrder =
    {
        BallColor.White,
        BallColor.Yellow,
        BallColor.Green,
        BallColor.Brown,
        BallColor.Blue,
        BallColor.Pink,
        BallColor.Black,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red,
        BallColor.Red
    };

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetAllBalls();
    }

    private void SetAllBalls()
    {
        if (ballPrefab == null)
        {
            Debug.LogError("ยังไม่ได้ลาก Ball Prefab ใส่ใน Inspector", this);
            return;
        }

        if (ballPositions == null || ballPositions.Length == 0)
        {
            Debug.LogError("ยังไม่ได้ใส่ Ball Positions ใน Inspector", this);
            return;
        }

        if (ballPositions.Length != ballOrder.Length)
        {
            Debug.LogWarning("จำนวน Ball Positions (" + ballPositions.Length +
                             ") ไม่เท่ากับ Ball Order (" + ballOrder.Length + ") จะวางเท่าที่มี", this);
        }

        int count = Mathf.Min(ballPositions.Length, ballOrder.Length);
        for (int i = 0; i < count; i++)
        {
            SetBall(ballOrder[i], i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetBall(BallColor col, int i)
    {
        if (ballPositions[i] == null)
        {
            Debug.LogWarning("Ball Positions ช่องที่ " + i + " ยังว่างอยู่", this);
            return;
        }

        GameObject ball = Instantiate(ballPrefab, ballPositions[i].transform.position, Quaternion.identity);
        ball.GetComponent<Ball>().SetColorAndPoint(col);
    }
}
