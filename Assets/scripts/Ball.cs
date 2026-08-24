using UnityEngine;

public enum BallColor
{
    White,
    Red,
    Yellow,
    Green,
    Brown,
    Blue,
    Pink,
    Black
}

public class Ball : MonoBehaviour
{
    [SerializeField]
    private int point;

    [SerializeField]
    private BallColor color;

    [SerializeField]
    private float stopSpeed = 0.3f;

    private MeshRenderer rd;
    private Rigidbody rb;
    private Vector3 spot;

    public int Point { get { return point; } }
    public BallColor ColorType { get { return color; } }
    public bool IsMoving { get { return rb.linearVelocity.magnitude > stopSpeed; } }

    void Awake()
    {
        rd = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezePositionY;
    }

    public void SetColorAndPoint(BallColor col)
    {
        color = col;

        switch (col)
        {
            case BallColor.White:
                point = 0;
                rd.material.color = Color.white;
                break;
            case BallColor.Red:
                point = 1;
                rd.material.color = Color.red;
                break;
            case BallColor.Yellow:
                point = 2;
                rd.material.color = Color.yellow;
                break;
            case BallColor.Green:
                point = 3;
                rd.material.color = Color.green;
                break;
            case BallColor.Brown:
                point = 4;
                rd.material.color = new Color(0.55f, 0.27f, 0.07f);
                break;
            case BallColor.Blue:
                point = 5;
                rd.material.color = Color.blue;
                break;
            case BallColor.Pink:
                point = 6;
                rd.material.color = new Color(1f, 0.75f, 0.8f);
                break;
            case BallColor.Black:
                point = 7;
                rd.material.color = Color.black;
                break;
        }
    }

    public void SetSpot(Vector3 pos)
    {
        spot = pos;
        transform.position = pos;
    }

    public void Stop()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void BackToSpot()
    {
        Stop();
        transform.position = spot;
        gameObject.SetActive(true);
    }

    public void Shoot(Vector3 dir, float power)
    {
        Stop();
        rb.AddForce(dir * power, ForceMode.VelocityChange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (color != BallColor.White)
            return;

        Ball other = collision.gameObject.GetComponent<Ball>();

        if (other == null)
            return;

        if (GameManger.instance != null)
            GameManger.instance.ReportFirstHit(other);

        if (AudioManager.instance != null)
            AudioManager.instance.PlayBallHit();
    }
}
