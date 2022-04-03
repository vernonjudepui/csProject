using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TankAI : MonoBehaviour {
    Pathfinding pathfinding;
    public GameObject bulletPrefab;
    public bool showGizmos = false;
    public float stoppingDist = 2.4f;
    public float speed = 2f;
    public float turnSpeed = 4f;

    float timeToNextFire = 0f;

    void Start() {
        pathfinding = GetComponent<Pathfinding>();
        StartCoroutine("UpdatePath");
    }

    Path path;

    void Update() {
        // TODO: avoid enemy if in line of fire / 
        FireIfSeeEnemy();
    }

    IEnumerator UpdatePath() {
        yield return new WaitForSeconds(0.5f);
        List<Node> pathNodes = pathfinding.FindPath();
        List<Vector2> waypoints = pathNodes.ConvertAll<Vector2>(n => n.position);
        path = new Path(waypoints.ToArray(), transform.localPosition, 0.8f, stoppingDist);
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
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

    IEnumerator FollowPath() {
        bool followingPath = true;
        int pathIndex = 0;
        Vector2 startDir = path.lookPoints[0] - (Vector2)transform.position;
        float lookToAngle = Mathf.Rad2Deg * Mathf.Atan(startDir.y / startDir.x) - 90;
        if (startDir.x < 0) lookToAngle += 180;
        transform.rotation = Quaternion.Euler(0, 0, lookToAngle);

        float speedPercent = 1;

        while (followingPath) {
            while (path.turnBoundaries[pathIndex].HasCrossedLine(transform.position)) {
                if (pathIndex == path.finishLineIndex) {
                    followingPath = false;
                    break;
                } else {
                    pathIndex++;
                }
            }

            Debug.Log(pathIndex);

            if (followingPath) {

                if (pathIndex >= path.slowDownIndex && stoppingDist > 0) {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(transform.position) / stoppingDist);
                    if (speedPercent < 0.01f) {
                        followingPath = false;
                    }
                }

                Vector2 dir = path.lookPoints[pathIndex] - (Vector2)transform.position;
                float targetAngle = Mathf.Rad2Deg * Mathf.Atan(dir.y / dir.x) - 90;
                if (dir.x < 0) targetAngle += 180;
                float currentAngle = transform.rotation.eulerAngles[2];
                Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector2.up * Time.deltaTime * speed * speedPercent, Space.Self);
            }

            yield return null;
        }
    }

    void OnDrawGizmos() {
        if (!showGizmos) return;
        if (path != null) {
            path.DrawWithGizmos();
        }
    }
}