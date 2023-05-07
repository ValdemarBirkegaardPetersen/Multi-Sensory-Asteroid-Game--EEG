using UnityEngine;

public class Asteroid : MonoBehaviour
{

    public GameObject rock;
    public GameObject gameplay;

    private float maxRotation;
    private float rotationX;
    private float rotationY;
    private float rotationZ;
    private Rigidbody rb;
    private Camera mainCam;
    private float maxSpeed;
    private float maxDamage;
    private int incDamage;

    void Start()
    {

        mainCam = Camera.main;

        maxRotation = 25f;
        rotationX = Random.Range(-maxRotation, maxRotation);
        rotationY = Random.Range(-maxRotation, maxRotation);
        rotationZ = Random.Range(-maxRotation, maxRotation);

        maxDamage = 25f;

        rb = rock.GetComponent<Rigidbody>();

        float speedX = Random.Range(200f, 800f);
        int selectorX = Random.Range(0, 2);
        float dirX = 0;
        if (selectorX == 1) { dirX = -1; }
        else { dirX = 1; }
        float finalSpeedX = speedX * dirX;
        rb.AddForce(transform.right * finalSpeedX);

        float speedY = Random.Range(200f, 800f);
        int selectorY = Random.Range(0, 2);
        float dirY = 0;
        if (selectorY == 1) { dirY = -1; }
        else { dirY = 1; }
        float finalSpeedY = speedY * dirY;
        rb.AddForce(transform.up * finalSpeedY);

        // damage calculation, based on speed of asteroid
        // incDamage = Mathf.RoundToInt(Mathf.Clamp(Mathf.Abs(finalSpeedX) + Mathf.Abs(finalSpeedY), 0f, maxDamage));
        
        float speedMagnitude = new Vector2(finalSpeedX, finalSpeedY).magnitude;

        // Calculate the normalized speed value between 0 and 1
        float normalizedSpeed = Mathf.Clamp01((speedMagnitude - 200) / (800 - 200));

        // Calculate the damage using linear interpolation
        incDamage = Mathf.RoundToInt(Mathf.Lerp(0f, maxDamage, normalizedSpeed));
    }

    void Update()
    {
        rock.transform.Rotate(new Vector3(rotationX, rotationY, 0) * Time.deltaTime);
        CheckPosition();
        float dynamicMaxSpeed = 3f;
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -dynamicMaxSpeed, dynamicMaxSpeed), Mathf.Clamp(rb.velocity.y, -dynamicMaxSpeed, dynamicMaxSpeed));
    }

    void OnCollisionEnter(Collision collisionInfo)
    {
        // print("Detected collision between " + gameObject.name + " and " + collisionInfo.collider.name);
        // print("There are " + collisionInfo.contacts.Length + " point(s) of contacts");
        // print("Their relative velocity is " + collisionInfo.relativeVelocity);
        // print("gameObject.name: " + gameObject.name);
        print("collisionInfo.collider.name: " + collisionInfo.collider.name);

        if (collisionInfo.collider.name == "Bullet(Clone)")
        {
            Destroy();
            gameplay.GetComponent<Gameplay>().numAsteroids -= 1;
            gameplay.GetComponent<Gameplay>().score += 100;
        }

        if (collisionInfo.collider.name == "Rocket")
        {
            gameplay.GetComponent<Gameplay>().RocketFail();
            gameplay.GetComponent<Gameplay>().DecreaseHP(incDamage);
        }
    }

    // TODO: Add particle effect when asteroids is destroyed
    
    private void CheckPosition()
    {

        float rockOffset = 1;

        float sceneWidth = mainCam.orthographicSize * 2 * mainCam.aspect;
        float sceneHeight = mainCam.orthographicSize * 2;

        float sceneRightEdge = sceneWidth / 2;
        float sceneLeftEdge = sceneRightEdge * -1;
        float sceneTopEdge = sceneHeight / 2;
        float sceneBottomEdge = sceneTopEdge * -1;

        if (rock.transform.position.x > sceneRightEdge + rockOffset)
        {
            rock.transform.position = new Vector2(sceneLeftEdge - rockOffset, rock.transform.position.y);
        }

        if (rock.transform.position.x < sceneLeftEdge - rockOffset)
        {
            rock.transform.position = new Vector2(sceneRightEdge + rockOffset, rock.transform.position.y);
        }

        if (rock.transform.position.y > sceneTopEdge + rockOffset)
        {
            rock.transform.position = new Vector2(rock.transform.position.x, sceneBottomEdge - rockOffset);
        }

        if (rock.transform.position.y < sceneBottomEdge - rockOffset)
        {
            rock.transform.position = new Vector2(rock.transform.position.x, sceneTopEdge + rockOffset);
        }

    }

    public void Destroy()
    {
        Destroy(gameObject, 0.01f);
    }

}
