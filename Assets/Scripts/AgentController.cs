using Unity.AI.Navigation;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _surface;
    private void Update()
    {
        if (_target == null || _surface.GetComponent<NavMeshSurface>().navMeshData == null)
        {
            return;
        }
        GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(_target.position);
    }
}
