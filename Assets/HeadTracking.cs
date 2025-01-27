using UnityEngine;

public class HeadTracking : MonoBehaviour
{
    #region Serialized Fields
        [SerializeField][Range(0,1)]public float lookAtWeight = 1.0f; // Head tracking intensity (0 to 1)
        [SerializeField] float headHeight = 1.5f; // Y-position of the character's head
        [SerializeField][Range(0,10)] float smoothSpeed = 10f; // Smoothing for head movement
    #endregion

    #region Private Fields
        private Animator animator;
        private Camera mainCamera;
        private Vector3 smoothLookAtPosition;

    #endregion
    
    void Start()
    {
        animator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            // Create a horizontal plane at the character's head height
            Plane horizontalPlane = new Plane(Vector3.up, new Vector3(0, headHeight, 0));
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            // Find where the ray intersects the horizontal plane
            float rayDistance;
            if (horizontalPlane.Raycast(ray, out rayDistance))
            {
                Vector3 targetPosition = ray.GetPoint(rayDistance);
                targetPosition.y = headHeight; // Lock Y to the head height

                // Smoothly interpolate the target position
                smoothLookAtPosition = Vector3.Lerp(
                    smoothLookAtPosition,
                    targetPosition,
                    Time.deltaTime * smoothSpeed
                );

                // Update the look-at target
                animator.SetLookAtWeight(lookAtWeight);
                animator.SetLookAtPosition(smoothLookAtPosition);
            }
        }
    }
}