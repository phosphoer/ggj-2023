using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachmentManager : MonoBehaviour
{
    public float EmptyChance = 0.3f;
    public Transform HeadLoc;
    public List<GameObject> Head_Attachments = new List<GameObject>();
    public Transform ChinLoc;
    public List<GameObject> Chin_Attachments = new List<GameObject>();
    public Transform EyeLoc;
    public List<GameObject> Eye_Attachments = new List<GameObject>();

    List<GameObject> attachments = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        RandomizeAttachments();    
    }

    public void RandomizeAttachments()
    {
        //Clear any existing...
        foreach(GameObject g in attachments)
        {
            GameObject.Destroy(g);
        }
        attachments.Clear();

        //Roll for HEAD
        float rollChance = Random.Range(0f,1f);
        if(rollChance>EmptyChance && Head_Attachments.Count>=1)
        {
            GameObject newHeadAttach = GameObject.Instantiate(Head_Attachments[Random.Range(0,Head_Attachments.Count)],Vector3.zero,Quaternion.identity,HeadLoc) as GameObject;
            attachments.Add(newHeadAttach);
            newHeadAttach.transform.localPosition = Vector3.zero;
        }
        

        //Roll for CHIN
        rollChance = Random.Range(0f,1f);
        if(rollChance>EmptyChance && Chin_Attachments.Count>=1)
        {
            GameObject newChinAttach = GameObject.Instantiate(Chin_Attachments[Random.Range(0,Chin_Attachments.Count)],Vector3.zero,Quaternion.identity,ChinLoc) as GameObject;
            attachments.Add(newChinAttach);
            newChinAttach.transform.localPosition = Vector3.zero;
        }
        

        //Roll for EYE
        rollChance = Random.Range(0f,1f);
        if(rollChance>EmptyChance && Eye_Attachments.Count>=1)
        {
            GameObject newEyeAttach = GameObject.Instantiate(Eye_Attachments[Random.Range(0,Eye_Attachments.Count)],Vector3.zero,Quaternion.identity,EyeLoc) as GameObject;
            attachments.Add(newEyeAttach);
            newEyeAttach.transform.localPosition = Vector3.zero;
        }
        
    }

}
