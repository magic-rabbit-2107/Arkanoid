using UnityEngine;
using System.Collections.Generic;
public class BallSpawner : MonoBehaviour
{
    private GameObject _ballPrefab;

    public List<GameObject> ballObjects {get; set; } = new List<GameObject>();

    private float _initialBallSpeed = 250;

    private Arena _arena;

    public void SetBallPrefab(GameObject ballPrefab)
    {
        _ballPrefab = ballPrefab;
        _arena = FindFirstObjectByType<Arena>();
    }

    public GameObject createBall()
    {
        GameObject ball = Instantiate(_ballPrefab, transform.position, Quaternion.identity);

        return ball;
    }

    public int getNumberOfBalls()
    {
        return this.ballObjects.Count;
    }

    public void SpawnBalls(Vector3 position, int numBalls)
    {
        for (int ballIndex = 0; ballIndex < numBalls; ballIndex++)
        {
            GameObject spawnedBall = createBall();

            spawnedBall.transform.SetParent(_arena.transform, false);

            spawnedBall.transform.localPosition = position;

            
        
            Rigidbody2D spawnedBallRb = spawnedBall.GetComponent<Rigidbody2D>();
            spawnedBallRb.isKinematic = false;
            spawnedBallRb.AddForce(new Vector2(0, _initialBallSpeed));

            ballObjects.Add(spawnedBall);

        }
    }
}
