/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

public class GroupLoader : MonoBehaviour {

    void Start() {
        MadLevel.LoadLevelByName(MadLevel.arguments);
    }

}