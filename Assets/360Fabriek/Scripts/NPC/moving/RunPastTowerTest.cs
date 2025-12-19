using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class NPCRunPastTowerTest : MonoBehaviour
{
    [SerializeField] private Transform tower;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float startDistance = 12f;
    [SerializeField] private float endDistance = 12f;
    [SerializeField] private bool loop = true;

    private Rigidbody rb;
    private Vector3 startPos;
    private Vector3 endPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true; // good for trigger events + scripted motion
        }
    }

    private void Start()
    {
        if (!tower)
        {
            Debug.LogError("[NPCRunPastTowerTest] Tower not assigned.");
            enabled = false;
            return;
        }

        Vector3 dir = (tower.position - transform.position);
        if (dir.sqrMagnitude < 0.001f) dir = Vector3.forward;
        dir.y = 0;
        dir.Normalize();

        startPos = tower.position - dir * startDistance;
        endPos = tower.position + dir * endDistance;

        transform.position = startPos;

        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        while (true)
        {
            yield return MoveTo(endPos);
            if (!loop) yield break;

            // Reset back behind the tower and run again
            transform.position = startPos;
            yield return null;
        }
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while ((transform.position - target).sqrMagnitude > 0.01f)
        {
            Vector3 next = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (rb) rb.MovePosition(next);
            else transform.position = next;

            yield return null;
        }
    }
}
