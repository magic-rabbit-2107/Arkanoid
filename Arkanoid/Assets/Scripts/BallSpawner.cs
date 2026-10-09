using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject _ballPrefab;

    public void SetBallPrefab(GameObject ballPrefab)
    {
        _ballPrefab = ballPrefab;
    }

    public GameObject createBall()
    {
        GameObject ball = Instantiate(_ballPrefab, transform.position, Quaternion.identity);

        return ball;
    }
}
