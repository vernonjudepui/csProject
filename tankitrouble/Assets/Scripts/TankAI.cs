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
    bool followingPath = false;

    void Start() {
        pathfinding = GetComponent<Pathfinding>();
        StartCoroutine("UpdatePath");
    }

    Path path;

    void Update() {
        // TODO: avoid enemy if in line of fire, avoid bullets a certain distance away by moving out of line of movement
        if (IsTargetInLineOfSight()) {
            // stop following path and fire
            // StopCoroutine("FollowPath");
            followingPath = false;

            // prioritize staying alive over firing
            if (!AvoidBullets()) {
                Vector2 direction = pathfinding.target.position - transform.position;
                transform.rotation = Quaternion.Lerp(transform.rotation, DirectionToQuaternion(direction), Time.deltaTime * turnSpeed);
                // Debug.DrawRay(transform.position, direction, Color.red);
                if (timeToNextFire > 0) {
                    timeToNextFire -= Time.deltaTime;
                } else {
                    FireBullet();
                    timeToNextFire = .5f;
                }
            }
        } else {
            // if cant see target and dont have a path, find a new path
            if (!followingPath) {
                StartCoroutine("UpdatePath");
            }
        }
    }

    IEnumerator UpdatePath() {
        yield return new WaitForSeconds(0.2f);
        List<Node> pathNodes = pathfinding.FindPath();
        GameObject.Find("AStar").GetComponent<Grid>().path = pathNodes;
        List<Vector2> waypoints = pathNodes.ConvertAll<Vector2>(n => n.position);
        path = new Path(waypoints.ToArray(), transform.localPosition, 0.8f, stoppingDist);
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    bool IsTargetInLineOfSight() {
        Vector2 direction = pathfinding.target.position - transform.position;
        RaycastHit2D[] results = new RaycastHit2D[4];
        if (Physics2D.Raycast(transform.localPosition, direction, new ContactFilter2D().NoFilter(), results, 20f) > 0) {
            if (results[1].collider.gameObject.transform == pathfinding.target) {
                return true;
            }
        }
        return false;
    }

    Quaternion DirectionToQuaternion(Vector2 direction) {
        if (direction.x == 0) {
            return Quaternion.Euler(0, 0, 0);
        }
        float angle = Mathf.Rad2Deg * Mathf.Atan(direction.y / direction.x) - 90;
        if (direction.x < 0) angle += 180;
        return Quaternion.Euler(0, 0, angle);
    }

    // avoid bullets
    // 

    void FireBullet() {
        float launchSpeed = 200f;
        GameObject ball = Instantiate(bulletPrefab, transform.position + transform.rotation * Vector2.up * 1.2f, transform.rotation);
        ball.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, launchSpeed));
    }

    IEnumerator FollowPath() {
        followingPath = true;
        int pathIndex = 0;
        Vector2 startDir = path.lookPoints[0] - (Vector2)transform.position;
        Quaternion startRotation = DirectionToQuaternion(startDir);
        while (Quaternion.Angle(transform.rotation, startRotation) > 2f) {
            transform.rotation = Quaternion.Lerp(transform.rotation, startRotation, Time.deltaTime * turnSpeed);
        }

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

            if (followingPath) {

                // if (pathIndex >= path.slowDownIndex && stoppingDist > 0) {
                //     speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(transform.position) / stoppingDist);
                //     if (speedPercent < 0.01f) {
                //         followingPath = false;
                //     }
                // }

                if (!AvoidBullets()) {
                    Vector2 dir = path.lookPoints[pathIndex] - (Vector2)transform.position;
                    Quaternion targetRotation = DirectionToQuaternion(dir);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    transform.Translate(Vector2.up * Time.deltaTime * speed * speedPercent, Space.Self);
                }
            }

            yield return null;
        }
    }

    float startAvoidingAtDistance = 6f; // only care about bullets within 6 units

    bool AvoidBullets() {
        // returns true if moved to avoid bullets
        GameObject[] allBullets = GameObject.FindGameObjectsWithTag("bullet");
        int numClosest = Mathf.Min(4, allBullets.Length); // checks 4 nearest bullets

        SortedList<float, GameObject> bulletDistances = new SortedList<float, GameObject>(numClosest);
        foreach (GameObject bullet in allBullets) {
            float distance = Vector2.Distance(transform.localPosition, bullet.transform.localPosition);
            if (distance < startAvoidingAtDistance) {
                bulletDistances.Add(distance, bullet);
            }
        }
        foreach (GameObject bullet in bulletDistances.Values) {
            Vector2 velocity = bullet.GetComponent<Rigidbody2D>().velocity;
            Vector2 direction = transform.position - bullet.transform.position;
            float angle = Mathf.Deg2Rad * Vector2.Angle(velocity, direction);
            // calculate distance from center of tank it is headed to
            float perpDistance = Mathf.Tan(angle) * direction.magnitude;

            if (perpDistance < 0.8f) { // beyond this should not hit the tank
                // move perpendicular to the velocity of the bullet to avoid it
                Vector2 targetDirection = new Vector2(velocity.y, -velocity.x);
                transform.rotation = Quaternion.Lerp(transform.rotation, DirectionToQuaternion(targetDirection), Time.deltaTime * turnSpeed);
                transform.Translate(Vector2.up * Time.deltaTime * speed, Space.Self);
                return true;
            }
        }
        return false;
    }

    void OnDrawGizmos() {
        if (!showGizmos) return;
        if (path != null) {
            path.DrawWithGizmos();
        }
    }
}