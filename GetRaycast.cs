/// <summary>
/// Raycastを管理
/// </summary>

using UnityEngine;
using System.Collections;

public class GetRaycast : MonoBehaviour
{

    public bool isHitRaycast;
    public string hitRaycastName;
    public string clickObjecttName;
    public Vector3 raycastPosition;

    #if UNITY_EDITOR
    [SerializeField]
    private bool DebugMode;
    # endif

    void Update()
    {

        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit = new RaycastHit();

        #if UNITY_EDITOR
        if (DebugMode)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward) * 20;
            Debug.DrawRay(transform.position, forward, Color.green);
        }
        #endif

        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            hitRaycastName = hit.transform.name;
            isHitRaycast = true;
            raycastPosition = hit.transform.position;
        }
        else
        {
            isHitRaycast = false;
            hitRaycastName = "NULL";
        }

        if (Input.GetMouseButtonDown(0))
        {

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit))
            {
                GameObject obj = hit.collider.gameObject;
                clickObjecttName = obj.name;
            }
        }
    }

}