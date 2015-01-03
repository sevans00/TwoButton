/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using MadLevelManager;

// this script should be placed in group select screen and will unlock group icons when needed
public class WorldUnlocker : MonoBehaviour {

    void Start() {
        string[] groups = MadLevel.GetAllLevelNames(MadLevel.Type.Level, MadLevel.defaultGroupName);

		//Setting world stars & world times:
		string groupName;
		for (int ii = 0; ii < groups.Length; ++ii) {
			groupName = groups[ii];
			float totalTime = StarsUtil.CountTotalTime(groupName);
			if ( totalTime > 0f ) {
				MadLevelProfile.SetLevelString(groupName, "time", totalTime.ToString() );
			}
			int acquired = StarsUtil.CountAcquiredStars(groupName);
			int allStars = StarsUtil.CountTotalStars(groupName);
			int starId = 1;
			MadLevelProfile.SetLevelBoolean(groupName, "star_1", false);
			MadLevelProfile.SetLevelBoolean(groupName, "star_2", false);
			MadLevelProfile.SetLevelBoolean(groupName, "star_3", false);
			if ( acquired > 1 ) {
				MadLevelProfile.SetLevelBoolean(groupName, "star_"+starId, true);
			}
			if ( acquired > allStars/2 ) {
				starId = 2;
				MadLevelProfile.SetLevelBoolean(groupName, "star_"+starId, true);
			}
			if ( allStars >= 3 ) { //
				starId = 3;
				if ( groupName == "World0" && acquired >= allStars - 3 ) { //-3 accounts for comic level
					MadLevelProfile.SetLevelBoolean(groupName, "star_"+starId, true);
				}
				if ( acquired >= allStars ) {
					MadLevelProfile.SetLevelBoolean(groupName, "star_"+starId, true);
				}
			}
		}

		//Unlocking:
        for (int i = 1; i < groups.Length; ++i) {
            string prevGroup = groups[i - 1];
            string group = groups[i];

			if (MadLevel.FindLastLevelName(MadLevel.Type.Level, prevGroup).Equals(MadLevel.FindLastUnlockedLevelName(prevGroup)) ) {
				MadLevelProfile.SetCompleted(prevGroup, true);
				if (MadLevelProfile.IsLocked(group)) {
					MadLevelProfile.SetLocked(group, false);
					MadLevel.ReloadCurrent();
				}
			}
            /*int acquired = StarsUtil.CountAcquiredStars(prevGroup);
			int allStars = MadLevel.GetAllLevelNames(prevGroup).Length;
            if (acquired >= allStars - 3 ) {
                if (MadLevelProfile.IsLocked(group)) {
                    MadLevelProfile.SetLocked(group, false);
                    MadLevel.ReloadCurrent();
                }
            }*/
        }
    }

    void Update() {
    }

}