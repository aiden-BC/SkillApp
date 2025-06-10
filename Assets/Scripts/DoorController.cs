using UnityEngine;

namespace XRMultiplayer
{
    public class DoorController : MonoBehaviour
    {
        [SerializeField] private Animator animator = null;

        void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void OpenDoor()
        {
            animator.Play("Door-Open");
        }

        public void CloseDoor()
        {
            animator.Play("CloseDoor");
        }
    }
}