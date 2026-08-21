using UnityEngine;

public class Pocket : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball == null)
            return;

        if (GameManger.instance != null)
            GameManger.instance.BallPotted(ball);
    }
}
