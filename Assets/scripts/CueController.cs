using UnityEngine;
using UnityEngine.InputSystem;

public class CueController : MonoBehaviour
{
    [SerializeField]
    private Transform cue;

    [SerializeField]
    private float maxPower = 25f;

    [SerializeField]
    private float chargeSpeed = 20f;

    [SerializeField]
    private float cueGap = 1.2f;

    [SerializeField]
    private float cueHalfLength = 4f;

    [SerializeField]
    private float ballY = 0.75f;

    private Camera cam;
    private Plane tablePlane;
    private Vector3 aimDir = Vector3.forward;
    private float power;
    private bool charging;

    void Start()
    {
        cam = Camera.main;
        tablePlane = new Plane(Vector3.up, new Vector3(0f, ballY, 0f));
    }

    void Update()
    {
        if (GameManger.instance == null || Mouse.current == null)
            return;

        if (!GameManger.instance.CanShoot)
        {
            HideCue();
            return;
        }

        Ball cueBall = GameManger.instance.CueBall;

        if (cueBall == null)
            return;

        Aim(cueBall.transform.position);
        Charge(cueBall.transform.position);
    }

    private void Aim(Vector3 ballPos)
    {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        float dist;

        if (!tablePlane.Raycast(ray, out dist))
            return;

        Vector3 dir = ray.GetPoint(dist) - ballPos;
        dir.y = 0f;

        if (dir.sqrMagnitude < 0.01f)
            return;

        aimDir = dir.normalized;
    }

    private void Charge(Vector3 ballPos)
    {
        if (Mouse.current.leftButton.isPressed)
        {
            charging = true;
            power = Mathf.Min(power + chargeSpeed * Time.deltaTime, maxPower);
        }
        else if (charging)
        {
            charging = false;
            GameManger.instance.Shoot(aimDir, power);
            power = 0f;
        }

        ShowCue(ballPos);

        if (UIManager.instance != null)
            UIManager.instance.ShowPower(power / maxPower);
    }

    private void ShowCue(Vector3 ballPos)
    {
        if (cue == null)
            return;

        if (!cue.gameObject.activeSelf)
            cue.gameObject.SetActive(true);

        float back = cueGap + power * 0.08f + cueHalfLength;
        cue.position = ballPos - aimDir * back;
        cue.rotation = Quaternion.LookRotation(aimDir) * Quaternion.Euler(90f, 0f, 0f);
    }

    private void HideCue()
    {
        if (cue != null && cue.gameObject.activeSelf)
            cue.gameObject.SetActive(false);

        power = 0f;
        charging = false;

        if (UIManager.instance != null)
            UIManager.instance.ShowPower(0f);
    }
}
