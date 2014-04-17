/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

public class StarsLabel : MonoBehaviour {


    void Start() {
        MadLevelIcon icon = MadTransform.FindParent<MadLevelIcon>(transform);
        MadText text = GetComponent<MadText>();

        int available = StarsUtil.CountAvailableStars(icon.level.name); // level name is a group name
        int acquired = StarsUtil.CountAcquiredStars(icon.level.name);

        text.text = "Stars: " + acquired + "/" + available;
    }

}