/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using MadLevelManager;

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public class MadLevelAbstractLayoutInspector : Editor {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    protected SerializedProperty iconTemplate;

    protected SerializedProperty lookAtLastLevel;
    protected SerializedProperty lookAtLevel;
    
    protected SerializedProperty twoStepActivationType;

    protected SerializedProperty onIconActivatePlayAudio;
    protected SerializedProperty onIconActivatePlayAudioClip;
    protected SerializedProperty onIconActivatePlayAudioVolume;
    
    protected SerializedProperty onIconDeactivatePlayAudio;
    protected SerializedProperty onIconDeactivatePlayAudioClip;
    protected SerializedProperty onIconDeactivatePlayAudioVolume;

    protected SerializedProperty onIconActivateMessage;
    protected SerializedProperty onIconActivateMessageReceiver;
    protected SerializedProperty onIconActivateMessageMethodName;
    
    protected SerializedProperty onIconDeactivateMessage;
    protected SerializedProperty onIconDeactivateMessageReceiver;
    protected SerializedProperty onIconDeactivateMessageMethodName;
    
    protected SerializedProperty handleMobileBackButton;
    protected SerializedProperty handleMobileBackButtonAction;
    protected SerializedProperty handleMobileBackButtonLevelName;

    protected SerializedProperty configuration;
    protected SerializedProperty configurationGroup;
    
    private MadLevelAbstractLayout s;

    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================

    // ===========================================================
    // Methods
    // ===========================================================
    
    protected virtual void OnEnable() {
        s = target as MadLevelAbstractLayout;
    
        iconTemplate = serializedObject.FindProperty("iconTemplate");

        lookAtLastLevel = serializedObject.FindProperty("lookAtLastLevel");
        lookAtLevel = serializedObject.FindProperty("lookAtLevel");
        
        twoStepActivationType = serializedObject.FindProperty("twoStepActivationType");
        
        onIconActivatePlayAudio = serializedObject.FindProperty("onIconActivatePlayAudio");
        onIconActivatePlayAudioClip = serializedObject.FindProperty("onIconActivatePlayAudioClip");
        onIconActivatePlayAudioVolume = serializedObject.FindProperty("onIconActivatePlayAudioVolume");
        
        onIconDeactivatePlayAudio = serializedObject.FindProperty("onIconDeactivatePlayAudio");
        onIconDeactivatePlayAudioClip = serializedObject.FindProperty("onIconDeactivatePlayAudioClip");
        onIconDeactivatePlayAudioVolume = serializedObject.FindProperty("onIconDeactivatePlayAudioVolume");
        
        onIconActivateMessage = serializedObject.FindProperty("onIconActivateMessage");
        onIconActivateMessageReceiver = serializedObject.FindProperty("onIconActivateMessageReceiver");
        onIconActivateMessageMethodName = serializedObject.FindProperty("onIconActivateMessageMethodName");
        
        onIconDeactivateMessage = serializedObject.FindProperty("onIconDeactivateMessage");
        onIconDeactivateMessageReceiver = serializedObject.FindProperty("onIconDeactivateMessageReceiver");
        onIconDeactivateMessageMethodName = serializedObject.FindProperty("onIconDeactivateMessageMethodName");
        
        handleMobileBackButton = serializedObject.FindProperty("handleMobileBackButton");
        handleMobileBackButtonAction = serializedObject.FindProperty("handleMobileBackButtonAction");
        handleMobileBackButtonLevelName = serializedObject.FindProperty("handleMobileBackButtonLevelName");
        
        configuration = serializedObject.FindProperty("configuration");
        configurationGroup = serializedObject.FindProperty("configurationGroup");
    }

    protected void LookAtLastLevel() {
        MadGUI.PropertyField(lookAtLastLevel, "Look At Previous Level", "When scene is loaded, it will automatically "
                                 + "go to the previously played level (but only if previous scene is of type Level.");

        GUILayout.Label("If above is disabled or cannot be determined, then...");

        MadGUI.PropertyFieldEnumPopup(lookAtLevel, "Look At Level");
    }

    protected void TwoStepActivation() {
        MadGUI.PropertyFieldEnumPopup(twoStepActivationType, "Two Step Activation");
        MadGUI.ConditionallyEnabled(
            twoStepActivationType.enumValueIndex != (int) MadLevelAbstractLayout.TwoStepActivationType.Disabled, () => {
            MadGUI.Indent(() => {
            
                if (MadGUI.Foldout("On Activate", false)) {
                    MadGUI.Indent(() => {
                        ActivateAction(
                            onIconActivatePlayAudio,
                            onIconActivatePlayAudioClip,
                            onIconActivatePlayAudioVolume,
                            onIconActivateMessage,
                            onIconActivateMessageReceiver,
                            onIconActivateMessageMethodName
                        );
                    });
                }
                
                if (MadGUI.Foldout("On Deactivate", false)) {
                    MadGUI.Indent(() => {
                        ActivateAction(
                            onIconDeactivatePlayAudio,
                            onIconDeactivatePlayAudioClip,
                            onIconDeactivatePlayAudioVolume,
                            onIconDeactivateMessage,
                            onIconDeactivateMessageReceiver,
                            onIconDeactivateMessageMethodName
                            );
                    });
                }
            });
        });
    }
    
    void ActivateAction(
        SerializedProperty playAudio,
        SerializedProperty playAudioClip,
        SerializedProperty playAudioVolume,
        SerializedProperty message,
        SerializedProperty messageReceiver,
        SerializedProperty messageMethodName
    ) {
        MadGUI.PropertyField(playAudio, "Play Audio");
        MadGUI.ConditionallyEnabled(playAudio.boolValue, () => {
            MadGUI.Indent(() => {
                if (playAudio.boolValue && !FoundAudioListener()) {
                    if (MadGUI.ErrorFix("There's no AudioListener on the scene. Do you want me to add an "
                                        + "AudioListener to Camera 2D?", "Add")) {
                        var camera = MadTransform.FindParent<Camera>((target as Component).transform);
                        if (camera == null) {
                            camera = FindObjectOfType(typeof(Camera)) as Camera;
                        }
                        if (camera != null) {
                            camera.gameObject.AddComponent<AudioListener>();
                        } else {
                            Debug.LogError("There's no camera on this scene!");
                        }
                    }
                }
                
                MadGUI.PropertyField(playAudioClip, "Audio Clip", MadGUI.ObjectIsSet);
                MadGUI.PropertyFieldSlider(playAudioVolume, 0, 1, "Volume");
            });
        });
        
        MadGUI.PropertyField(message, "Send Message");
        MadGUI.ConditionallyEnabled(message.boolValue, () => {
            MadGUI.Indent(() => {
                MadGUI.PropertyField(messageReceiver, "Receiver", MadGUI.ObjectIsSet);
                MadGUI.PropertyField(messageMethodName, "Method Name", MadGUI.StringNotEmpty);
                
                if (message.boolValue) {
                    MadGUI.Info("This should look like this:\nvoid " + messageMethodName.stringValue + "(MadLevelIcon icon)");
                }
            });
        });
    }
    
    bool FoundAudioListener() {
        var obj = FindObjectOfType(typeof(AudioListener));
        return obj != null;
    }
    
    protected string[] GroupNames(MadLevelConfiguration configuration) {
        var groups = configuration.groups;
        var groupNames = new List<string>();
        groupNames.Add(configuration.defaultGroup.name);
        
        foreach (var g in groups) {
            groupNames.Add(g.name);
        }
        
        return groupNames.ToArray();
    }
    
    protected MadLevelConfiguration.Group IndexToGroup(MadLevelConfiguration configuration, int index) {
        if (index == 0) {
            return configuration.defaultGroup;
        } else {
            return configuration.groups[index - 1];
        }
    }
    
    protected int GroupToIndex(MadLevelConfiguration configuration, MadLevelConfiguration.Group group) {
        if (group == configuration.defaultGroup) {
            return 0;
        } else {
            if (configuration.groups.Contains(group)) {
                return configuration.groups.IndexOf(group) + 1;
            } else {
                Debug.LogError("Group not found: " + group);
                return 0;
            }
        }
    }
    
    protected void HandleMobileBack() {
        MadGUI.PropertyField(handleMobileBackButton, "Handle Mobile 'Back'",
            "Handles mobile 'back' action by loading selected level.");
        MadGUI.Indent(() => {
            MadGUI.PropertyFieldEnumPopup(handleMobileBackButtonAction, "Action");
            
            if (s.handleMobileBackButtonAction == MadLevelAbstractLayout.OnMobileBack.LoadSpecifiedLevel) {
                MadGUI.Indent(() => {
                    MadGUI.PropertyField(handleMobileBackButtonLevelName, "Level Name", MadGUI.StringNotEmpty);
                });
            }
        });
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

#if !UNITY_3_5
} // namespace
#endif