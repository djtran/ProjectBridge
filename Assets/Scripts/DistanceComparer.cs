using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceComparer : IComparer {

    public GameObject originator;

    public int Compare(object x, object y)
    {
        GameObject first = (GameObject)x;
        GameObject second = (GameObject)y;
        float distance1 = Vector3.Distance(originator.transform.position, first.transform.position);
        float distance2 = Vector3.Distance(originator.transform.position, second.transform.position);

        if(first.GetComponent<Stats>().dead)
        {
            distance1 += 1000.0f;
        }
        if(second.GetComponent<Stats>().dead)
        {
            distance2 += 1000.0f;
        }

        if(distance1 > distance2)
        {
            return 1;
        }
        else if(distance2 > distance1)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
