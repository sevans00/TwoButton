/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

// this script should be placed in group select screen and will unlock group icons when needed
public class GroupUnlocker : MonoBehaviour {

    void Start() {
        string[] groups = MadLevel.GetAllLevelNames(MadLevel.Type.Level, MadLevel.defaultGroupName);

        for (int i = 1; i < groups.Length; ++i) {
            string prevGroup = groups[i - 1];
            string group = groups[i];

            int acquired = StarsUtil.CountAcquiredStars(prevGroup);
            if (acquired >= 6) {
                if (MadLevelProfile.IsLocked(group)) {
                    MadLevelProfile.SetLocked(group, false);
                    MadLevel.ReloadCurrent();
                }
            }
        }
    }

    void Update() {
    }

}