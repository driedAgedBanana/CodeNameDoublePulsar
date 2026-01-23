using UnityEngine;

public class TutorialFollow : MonoBehaviour
{
    [SerializeField] private Vector3 offSet = new Vector3(0f, 1f, 0f);
    [SerializeField] private float smoothTime = 0.25f;
    private Vector3 velocity = Vector3.zero;

    [SerializeField] private Transform target;

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isMainMenuActive)
        {
            target = GameObject.FindWithTag("Player").transform; // This is such a stupid way to do this but whatever
            if (target != null)
            {
                Vector3 targetPosition = target.position + offSet;
                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            }
        }
    }
}
