using UnityEngine;

public class PlayAnimationWhenTrigger : MonoBehaviour
{
    [SerializeField] private Animator _animatorController;
    [SerializeField] private string _triggerName = "death";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trigger"))
        {
            _animatorController.SetTrigger(_triggerName);
        }
    }
}
