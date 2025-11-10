using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    Camera c;
    public float cameraTrackSpeed = 3;
    float cameraCatchSpeed = 12;
    public float cameraMaxDistance = 8;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        c = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //set the camera move speed
        float currentCamSpeed = cameraTrackSpeed;
        //check if camera is too far away
        // print(Vector2.Distance(transform.position, c.transform.position));
        if (Vector2.Distance(transform.position, c.transform.position) >= cameraMaxDistance)
        {
            //increase move speed if too far
            currentCamSpeed = cameraCatchSpeed;
        }
        //move camera
        c.transform.position = Vector3.Lerp(c.transform.position, transform.position, Time.deltaTime * currentCamSpeed);
        //reset camera depth
        c.transform.position = new Vector3(c.transform.position.x, c.transform.position.y, -10);
    }
}
