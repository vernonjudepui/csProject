// ai that moves via translation (no rotation)

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class OldTankAI : MonoBehaviour {
    Pathfinding pathfinding;
    public GameObject bulletPrefab;
    public bool showGizmos = false;
    public float stoppingDist = 2.4f;
    public float speed = 2f;
    public float turnSpeed = 4f;

    float timeToNextFire = 0f;

    void Start() {
        pathfinding = GetComponent<Pathfinding>();
        // List<Node> pathNodes = pathfinding.FindPath();
        // List<Vector2> waypoints = pathNodes.ConvertAll<Vector2>(n => n.position);
        // path = new Path(waypoints.ToArray(), transform.localPosition, 0.8f, stoppingDist);
        // StopCoroutine("FollowPath");
        // StartCoroutine("FollowPath");
    }

    // Path path;

    void Update() {
        List<Node> pathNodes = pathfinding.FindPath();
        GameObject.Find("AStar").GetComponent<Grid>().path = pathNodes;
        if (pathNodes.Count < 1) return;
        Vector2 direction = (pathNodes[0].position - (Vector2)transform.position).normalized;

        if (Vector2.Distance(transform.position, pathfinding.target.transform.position) < stoppingDist) {
            direction = Vector2.zero;
        }

        // dodge bullets
        GameObject[] allBullets = GameObject.FindGameObjectsWithTag("bullet");
        int numClosest = Mathf.Min(4, allBullets.Length); // checks 4 nearest bullets

        SortedList<float, GameObject> bulletDistances = new SortedList<float, GameObject>(numClosest);
        foreach (GameObject bullet in allBullets) {
            bulletDistances.Add(Vector2.Distance(transform.localPosition, bullet.transform.localPosition), bullet);
        }
        Vector2 nextPos = (Vector2)transform.position + direction * speed * Time.deltaTime;
        foreach (GameObject bullet in bulletDistances.Values) {
            Vector2 velocity = bullet.GetComponent<Rigidbody2D>().velocity;
            Vector2 nextBulletPos = (Vector2)bullet.transform.localPosition + velocity * Time.deltaTime;
            // check if will collide in next timedelta
            if (Vector2.Distance(nextPos, nextBulletPos) < 1f) {
                // move perpendicular to the velocity of the bullet to avoid it
                direction = new Vector2(velocity.y, -velocity.x).normalized;
                break;
            }
        }

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        FireIfSeeEnemy();
    }

    void FireIfSeeEnemy() {
        // check if enemy is in line of sight
        if (timeToNextFire > 0) {
            timeToNextFire -= Time.deltaTime;
            return;
        }
        Vector2 direction = pathfinding.target.position - transform.position;
        RaycastHit2D[] results = new RaycastHit2D[4];
        if (Physics2D.Raycast(transform.localPosition, direction, new ContactFilter2D().NoFilter(), results, 20f) > 0) {
            if (results[1].collider.gameObject.transform == pathfinding.target) {
                float angle = Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x) - 90;
                if (direction.x < 0) angle += 180;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                // Debug.DrawRay(transform.position, direction, Color.red);
                FireBullet();
                timeToNextFire = 2f;
            }
        }
    }

    // avoid bullets
    // 

    void FireBullet() {
        float launchSpeed = 200f;
        GameObject ball = Instantiate(bulletPrefab, transform.position + transform.rotation * Vector2.up * 2, transform.rotation);
        ball.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, launchSpeed));
    }

    // IEnumerator FollowPath() {
    //     bool followingPath = true;
    //     int pathIndex = 0;
    //     Vector2 startDir = path.lookPoints[0] - (Vector2)transform.position;
    //     transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * Mathf.Atan(startDir.y / startDir.x) - 90);
    //     Debug.Log(transform.position);

    //     float speedPercent = 1;

    //     while (followingPath) {
    //         Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
    //         while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D)) {
    //             if (pathIndex == path.finishLineIndex) {
    //                 followingPath = false;
    //                 break;
    //             } else {
    //                 pathIndex++;
    //             }
    //         }

    //         if (followingPath) {

    //             if (pathIndex >= path.slowDownIndex && stoppingDist > 0) {
    //                 speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDist);
    //                 if (speedPercent < 0.01f) {
    //                     followingPath = false;
    //                 }
    //             }

    //             Vector2 dir = path.lookPoints[pathIndex] - (Vector2)transform.position;
    //             float targetAngle = Mathf.Rad2Deg * Mathf.Atan(dir.y / dir.x) - 90;
    //             float currentAngle = transform.rotation.eulerAngles[2];
    //             if (targetAngle < currentAngle) {
    //                 transform.rotation.SetEulerRotation(0, 0, currentAngle - 360);
    //             }
    //             Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
    //             Debug.Log("next look point: " + path.lookPoints[pathIndex]);
    //             Debug.Log("next rotation: " + targetRotation.eulerAngles[2]);
    //             transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    //             // transform.Translate(0f, Time.deltaTime * speed * speedPercent, 0f);
    //             transform.Translate(Vector2.up * Time.deltaTime * speed * speedPercent, Space.Self);
    //         }

    //         yield return null;
    //     }
    // }

    // void OnDrawGizmos() {
    //     if (!showGizmos) return;
    //     if (path != null) {
    //         path.DrawWithGizmos();
    //     }
    // }
}