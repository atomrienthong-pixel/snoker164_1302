using UnityEngine;
using UnityEngine.InputSystem;

public class CueController : MonoBehaviour
{
    [SerializeField]
    private Transform cue;

    [SerializeField]
    private Transform cam;

    [SerializeField]
    private Transform aimLine;

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

    [SerializeField]
    private Vector3 topPosition = new Vector3(0f, 55f, 0f);

    [SerializeField]
    private float topPitch = 90f;

    [SerializeField]
    private float ballRadius = 0.75f;

    [SerializeField]
    private float lineWidth = 0.2f;

    [SerializeField]
    private float maxLineLength = 40f;

    private float aimAngle;
    private float chargeTime;
    private float power;
    private bool charging;
    private bool topView = true;

    private Vector3 AimDir { get { return Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward; } }

    void Update()
    {
        if (GameManger.instance == null)
            return;

        Ball cueBall = GameManger.instance.CueBall;

        if (cueBall == null)
            return;

        Vector3 ballPos = cueBall.transform.position;

        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            topView = !topView;

        if (GameManger.instance.CanShoot)
        {
            Aim();
            Charge();
            ShowCue(ballPos);
            ShowAimLine(ballPos);
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

        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
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

    private void ShowAimLine(Vector3 ballPos)
    {
        if (aimLine == null)
            return;

        if (!aimLine.gameObject.activeSelf)
            aimLine.gameObject.SetActive(true);

        Vector3 dir = AimDir;
        Vector3 origin = ballPos + dir * ballRadius;
        float length = maxLineLength;
        RaycastHit hit;

        if (Physics.Raycast(origin, dir, out hit, maxLineLength, ~0, QueryTriggerInteraction.Ignore))
            length = hit.distance;

        aimLine.position = origin + dir * (length * 0.5f);
        aimLine.rotation = Quaternion.LookRotation(dir);
        aimLine.localScale = new Vector3(lineWidth, lineWidth, length);
    }

    private void HideCue()
    {
        if (cue != null && cue.gameObject.activeSelf)
            cue.gameObject.SetActive(false);

        if (aimLine != null && aimLine.gameObject.activeSelf)
            aimLine.gameObject.SetActive(false);

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

        Vector3 target;
        Quaternion rot;

        if (topView)
        {
            target = topPosition;
            rot = Quaternion.Euler(topPitch, 0f, 0f);
        }
        else
        {
            Vector3 dir = AimDir;
            target = ballPos - dir * camDistance + Vector3.up * camHeight;
            Vector3 look = ballPos + dir * camLookAhead - cam.position;

            if (look.sqrMagnitude < 0.01f)
                return;

            rot = Quaternion.LookRotation(look);
        }

        cam.position = Vector3.Lerp(cam.position, target, camFollowSpeed * Time.deltaTime);
        cam.rotation = Quaternion.Slerp(cam.rotation, rot, camFollowSpeed * Time.deltaTime);
    }
}
