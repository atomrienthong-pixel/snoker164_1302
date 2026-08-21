using UnityEngine;
using UnityEngine.InputSystem;

public class CueController : MonoBehaviour
{
    [SerializeField]
    private Transform cue;

    [SerializeField]
    private Transform cam;

    [SerializeField]
    private float maxPower = 45f;

    [SerializeField]
    private float minPower = 5f;

    [SerializeField]
    private float chargeSpeed = 35f;

    [SerializeField]
    private float aimSpeed = 90f;

    [SerializeField]
    private float mouseAimSpeed = 0.2f;

    [SerializeField]
    private float cueGap = 1.2f;

    [SerializeField]
    private float cueHalfLength = 4f;

    [SerializeField]
    private float camDistance = 11f;

    [SerializeField]
    private float camHeight = 5f;

    [SerializeField]
    private float camLookAhead = 9f;

    [SerializeField]
    private float camFollowSpeed = 8f;

    private float aimAngle;
    private float chargeTime;
    private float power;
    private bool charging;

    private Vector3 AimDir { get { return Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward; } }

    void Update()
    {
        if (GameManger.instance == null)
            return;

        Ball cueBall = GameManger.instance.CueBall;

        if (cueBall == null)
            return;

        Vector3 ballPos = cueBall.transform.position;

        if (GameManger.instance.CanShoot)
        {
            Aim();
            Charge();
            ShowCue(ballPos);
        }
        else
        {
            HideCue();
        }

        MoveCamera(ballPos);
    }

    private void Aim()
    {
        if (charging)
            return;

        float move = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
                move -= 1f;

            if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
                move += 1f;
        }

        aimAngle += move * aimSpeed * Time.deltaTime;

        if (Mouse.current != null)
            aimAngle += Mouse.current.delta.ReadValue().x * mouseAimSpeed;
    }

    private void Charge()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.isPressed)
        {
            charging = true;
            chargeTime += Time.deltaTime;
            power = Mathf.PingPong(chargeTime * chargeSpeed, maxPower);
        }
        else if (charging)
        {
            charging = false;
            GameManger.instance.Shoot(AimDir, Mathf.Max(power, minPower));
            chargeTime = 0f;
            power = 0f;
        }

        ShowPower();
    }

    private void ShowCue(Vector3 ballPos)
    {
        if (cue == null)
            return;

        if (!cue.gameObject.activeSelf)
            cue.gameObject.SetActive(true);

        Vector3 dir = AimDir;
        float back = cueGap + power * 0.08f + cueHalfLength;
        cue.position = ballPos - dir * back;
        cue.rotation = Quaternion.LookRotation(dir) * Quaternion.Euler(90f, 0f, 0f);
    }

    private void HideCue()
    {
        if (cue != null && cue.gameObject.activeSelf)
            cue.gameObject.SetActive(false);

        charging = false;
        chargeTime = 0f;
        power = 0f;
        ShowPower();
    }

    private void ShowPower()
    {
        if (UIManager.instance != null)
            UIManager.instance.ShowPower(power / maxPower);
    }

    private void MoveCamera(Vector3 ballPos)
    {
        if (cam == null)
            return;

        Vector3 dir = AimDir;
        Vector3 target = ballPos - dir * camDistance + Vector3.up * camHeight;
        Vector3 lookAt = ballPos + dir * camLookAhead;

        cam.position = Vector3.Lerp(cam.position, target, camFollowSpeed * Time.deltaTime);

        Vector3 look = lookAt - cam.position;

        if (look.sqrMagnitude < 0.01f)
            return;

        cam.rotation = Quaternion.Slerp(cam.rotation, Quaternion.LookRotation(look), camFollowSpeed * Time.deltaTime);
    }
}
