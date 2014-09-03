/*
* Copyright (c) Mad Pixel Machine
* http://www.madpixelmachine.com/
*/

using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class TrialInfo {

    public static void TrialStarted() {
        EditorUtility.DisplayDialog(
            "Mad Level Manager Evaluation",
            "This is a full-featured evaluation version of Mad Level Manager. Feel free to use it for 14 days.\n\n"
            + "Please be aware that all content created by this evaluation version IS NOT USABLE with the full version of Mad Level Manager (You will need to configure all things again).",
            "OK");
    }

    public static void TrialEnded() {
        if (EditorUtility.DisplayDialog(
            "Mad Level Manager Evalutation Expired.",
            "Mad Level Manager evaluation period has expired. We hope you have had a good time while evaluating Mad Level Manager. \n\n"
            + "Please purchase our product for continuous use or request another evaluation key from the Tools/Mad Level Manager/Request Evaluation Key menu.",
                "Purchase", "Cancel")) {
                    Application.OpenURL("_TRIAL_PURCHASE_URL_");
        }
    }

}

#if !UNITY_3_5
} // namespace
#endif