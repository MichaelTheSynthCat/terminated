using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WWWScript : MonoBehaviour
{
    public List<GameObject> connected_routers;
    private LineRenderer linerender;

    private void Awake()
    {
        RouterScript routerscript;
        foreach (GameObject router in connected_routers)
        {
            routerscript = router.GetComponent<RouterScript>();
            routerscript.ConnectToInternet(this.gameObject);
        }
    }
    void Start()
    {
        // create lines
        RenderLines();
    }

    // Update is called once per frame
    void Update()
    {
        RenderLines();
    }

    public void RenderLines()
    {
        linerender = GetComponent<LineRenderer>();
        int max_vertex = 0;
        foreach(GameObject router in connected_routers)
        {
            if (router.GetComponent<DeviceScript>().CanPlayerSee(1))
            {
                max_vertex += 2;
            }
        }
        if(max_vertex > 0)
        {
            linerender.positionCount = max_vertex;
            int vertex = 0;

            foreach (GameObject router in connected_routers)
            {
                if (router.GetComponent<DeviceScript>().CanPlayerSee(1))
                {
                    linerender.SetPosition(vertex, gameObject.transform.position);
                    vertex++;
                    linerender.SetPosition(vertex, router.transform.position);
                    vertex++;
                }
            }
        }
        
    }
}
