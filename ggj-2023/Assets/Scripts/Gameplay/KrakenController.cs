using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenController : MonoBehaviour
{
    public List<GameObject> KrakenArms = new List<GameObject>();
    // Start is called before the first frame update

    GameStateManager GSM;
    bool disabled = false;
    float activationInterval = 0f;
    int currentArm = 0;
    int randOffset = 0;

    void Start()
    {
        if(FindObjectOfType(typeof(GameStateManager))) GSM = GameObject.FindObjectOfType<GameStateManager>();

        if(GSM)
        {
            activationInterval = GSM.GameplayDuration / (KrakenArms.Count+1);
            randOffset = Random.Range(0,KrakenArms.Count);
        }
        else disabled = true;
    }

    // Update is called once per frame
    void Update()
    {
           if(!disabled && GSM.TimeInState>=((currentArm+1)*activationInterval)) ActivateArm();
    }

    public void ActivateArm()
    {
        if(randOffset+currentArm>=KrakenArms.Count) randOffset=0;
        KrakenArms[currentArm+randOffset].SetActive(true);
        currentArm++;
    }
}
