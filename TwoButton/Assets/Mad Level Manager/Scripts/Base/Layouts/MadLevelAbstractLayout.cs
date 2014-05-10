/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MadLevelManager;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

public abstract class MadLevelAbstractLayout : MadNode {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    public MadLevelIcon iconTemplate;

    // if true it will look at last played level
    public bool lookAtLastLevel = true;
    // if above cannot be found, will look at the defined level type
    public LookLevelType lookAtLevel = LookLevelType.FirstLevel;
    
    //
    // Icon activation
    //
    
    // how icons should be activated
    public TwoStepActivationType twoStepActivationType = TwoStepActivationType.Disabled;
    private MadLevelIcon activeIcon;

    // make a sound option
    public bool onIconActivatePlayAudio;
    public AudioClip onIconActivatePlayAudioClip;
    public float onIconActivatePlayAudioVolume = 1;
    
    public bool onIconDeactivatePlayAudio;
    public AudioClip onIconDeactivatePlayAudioClip;
    public float onIconDeactivatePlayAudioVolume = 1;
    
    private AudioListener cachedAudioListener;
            
    // send message option
    public bool onIconActivateMessage;
    public GameObject onIconActivateMessageReceiver;
    public string onIconActivateMessageMethodName = "OnIconActivate";
    
    public bool onIconDeactivateMessage;
    public GameObject onIconDeactivateMessageReceiver;
    public string onIconDeactivateMessageMethodName = "OnIconDeactivate";
    
    // mobile "back" button behaviour
    public bool handleMobileBackButton = true;
    public OnMobileBack handleMobileBackButtonAction = OnMobileBack.LoadPreviousLevel;
    public string handleMobileBackButtonLevelName;
    
    [HideInInspector]
    public MadLevelConfiguration configuration;
    public int configurationGroup = 0;
    
    // ===========================================================
    // Events
    // ===========================================================
    
    public delegate void IconActivationEvent(MadLevelIcon icon, string levelName);
    
    public event IconActivationEvent onIconActivate;
    public event IconActivationEvent onIconDeactivate;
    
    // ===========================================================
    // Methods
    // ===========================================================
    
    protected virtual void Update() {
        UpdateHandleMobileBackButton();
    }
    
    void UpdateHandleMobileBackButton() {
        if (SystemInfo.deviceType == DeviceType.Handheld) {
            if (handleMobileBackButton && Input.GetKey(KeyCode.Escape)) {
                switch (handleMobileBackButtonAction) {
                    case OnMobileBack.LoadPreviousLevel:
                        MadLevel.LoadPrevious();
                        break;
                    case OnMobileBack.LoadSpecifiedLevel:
                        MadLevel.LoadLevelByName(handleMobileBackButtonLevelName);
                        break;
                    default:
                        Debug.LogError("Unknown action: " + handleMobileBackButtonAction);
                        break;
                }
            }
        }
    }
    
    #region Public API
    
    /// <summary>
    /// Gets the icon representation for given level name.
    /// </summary>
    /// <returns>The level icon or <code>null</code> if not found.</returns>
    /// <param name="levelName">Level name.</param>
    public abstract MadLevelIcon GetIcon(string levelName);
    
    /// <summary>
    /// Gets the icon representation for the first level (in order).
    /// </summary>
    /// <returns>The first level icon.</returns>
    public MadLevelIcon GetFirstIcon() {
        string groupName = MadLevel.activeConfiguration.FindGroupById(configurationGroup).name;
        string firstLevelName = MadLevel.FindFirstLevelName(MadLevel.Type.Level, groupName);
        return GetIcon(firstLevelName);
    }
    
    /// <summary>
    /// Gets the icon representation for the last level (in order).
    /// </summary>
    /// <returns>The last level icon.</returns>
    public MadLevelIcon GetLastIcon() {
        string groupName = MadLevel.activeConfiguration.FindGroupById(configurationGroup).name;
        string lastLevelName = MadLevel.FindLastLevelName(MadLevel.Type.Level, groupName);
        return GetIcon(lastLevelName);
    }
    
    /// <summary>
    /// Gets the currently active icon or <code>null</code> if no icon is active.
    /// </summary>
    /// <returns>The currently active icon.</returns>
    public MadLevelIcon GetActiveIcon() {
        return activeIcon;
    }
    
    /// <summary>
    /// Finds the closest level icon to given position or <code>null</code> if no icons found.
    /// </summary>
    /// <returns>The closest level icon.</returns>
    /// <param name="position">The position.</param>
    public abstract MadLevelIcon FindClosestIcon(Vector3 position);
    
    public MadLevelIcon GetNextIcon(MadLevelIcon icon) {
        var nextLevel = configuration.FindNextLevel(icon.level.name);
        if (nextLevel == null) {
            return null;
        }
        
        return GetIcon(nextLevel.name);
    }
    
    public MadLevelIcon GetPreviousIcon(MadLevelIcon icon) {
        var nextLevel = configuration.FindPreviousLevel(icon.level.name);
        if (nextLevel == null) {
            return null;
        }
        
        return GetIcon(nextLevel.name);
    }
    
    /// <summary>
    /// Looks at given icon;
    /// </summary>
    /// <param name="icon">Icon.</param>
    public abstract void LookAtIcon(MadLevelIcon icon);

    /// <summary>
    /// Looks at level.
    /// </summary>
    /// <param name="levelName">Level name.</param>
    public bool LookAtLevel(string levelName) {
        var level = MadLevel.activeConfiguration.FindLevelByName(levelName);
        
        if (level.type == MadLevel.Type.Other) {
            Debug.LogWarning("Level " + levelName + " is of wrong type. Won't look at it.");
            return false;
        } else if (level.type == MadLevel.Type.Extra) {
            level = configuration.FindPreviousLevel(MadLevel.lastPlayedLevelName, MadLevel.Type.Level);
            if (level == null) {
                Debug.Log("Cannot find previous level icon.");
                return false; ; // cannot find previous level of type level
            }
        }
        
        var icon = GetIcon(levelName);
        if (icon != null) {
            LookAtIcon(icon);
            return true;
        } else {
            Debug.Log("Cannot find icon for level: " + levelName);
            return false;
        }
    }
    
    /// <summary>
    /// Looks at last played level icon.
    /// </summary>
    /// <returns>true if last level were found</returns>
    public bool LookAtLastPlayedLevel() {
        if (!Application.isPlaying) {
            return false; // do not do anything if is not in play mode
        }
    
        string lastPlayedLevelName = MadLevel.lastPlayedLevelName;
        if (!string.IsNullOrEmpty(lastPlayedLevelName)) {
            return LookAtLevel(lastPlayedLevelName);
        } else {
            return false;
        }
    }
    
    #endregion
    
    protected virtual void OnEnable() {
        if (configuration == null) {
            // not bound to any configuration. Bound it to active
            if (MadLevel.hasActiveConfiguration) {
                configuration = MadLevel.activeConfiguration;
            } else {
                Debug.LogWarning("There's no active level configuration. Please prepare one and activate it.");
            }
        } else if (configuration != MadLevel.activeConfiguration) {
            if (Application.isPlaying) {
                Debug.LogWarning("This layout was prepared for different level configuration than the active one. "
                    + "http://goo.gl/AxZqW2", this);
            }
        }
        
        var panel = MadTransform.FindParent<MadPanel>(transform);
        panel.onFocusChanged += (MadSprite sprite) => {
            if (activeIcon != null && sprite != activeIcon) {
                DeactivateActiveIcon();
            }
        };
        
        onIconActivate += (icon, levelName) => {
            if (onIconActivateMessage && onIconActivateMessageReceiver != null) {
                onIconActivateMessageReceiver.SendMessage(onIconActivateMessageMethodName, icon);
            }
            
            if (onIconActivatePlayAudio && onIconActivatePlayAudioClip != null && cachedAudioListener != null) {
                AudioSource.PlayClipAtPoint(
                    onIconActivatePlayAudioClip,
                    cachedAudioListener.transform.position,
                    onIconActivatePlayAudioVolume);
            }
        };
        
        onIconDeactivate += (icon, levelName) => {
            if (onIconDeactivateMessage && onIconDeactivateMessageReceiver != null) {
                onIconDeactivateMessageReceiver.SendMessage(onIconDeactivateMessageMethodName, icon);
            }
            
            if (onIconDeactivatePlayAudio && onIconDeactivatePlayAudioClip != null && cachedAudioListener != null) {
                AudioSource.PlayClipAtPoint(
                    onIconDeactivatePlayAudioClip,
                    cachedAudioListener.transform.position,
                    onIconDeactivatePlayAudioVolume);
            }
        };
            
#if UNITY_EDITOR
        EditorApplication.playmodeStateChanged = () => {
            if (configuration != null
                && EditorApplication.isPlayingOrWillChangePlaymode
                && !EditorApplication.isPlaying) {
                
                if (!configuration.IsValid()) {
                    if (!EditorUtility.DisplayDialog(
                        "Invalid Configuration",
                        "Your level configuration has errors. Do you want to continue anyway?",
                        "Yes", "No")) {
                            EditorApplication.isPlaying = false;
                            Selection.activeObject = configuration;
                            return;
                        }
                }
                
                if (configuration != MadLevel.activeConfiguration
                    || !configuration.CheckBuildSynchronized()
                    || !configuration.active) {
                    if (EditorUtility.DisplayDialog(
                        "Not Synchronized",
                        "Build configuration of choice is not activate/synchronized with this level select layout "
                        + "(errors will occur). Do it now?",
                        "Yes", "No")) {
                            var active = MadLevelConfiguration.GetActive();
                            if (active != null) {
                                active.active = false; // workaround
                            }
                            configuration.active = true;
                            configuration.SynchronizeBuild();
                    }
                }    
            }
        };
#endif
        
    }
    
    protected virtual void Start() {
        if (onIconActivatePlayAudio) {
            cachedAudioListener = FindObjectOfType(typeof(AudioListener)) as AudioListener;
            if (cachedAudioListener == null) {
                Debug.LogError("Cannot find an audio listener for this scene. Audio will not be played");
            }
        }

        if (Application.isPlaying) {
            bool looked = false;
            if (lookAtLastLevel) {
                looked = LookAtLastPlayedLevel();
            }

            if (!looked) {
                // need to look at other type of level
                LookAtLevel();
            }
        }
    }

    private void LookAtLevel() {
        switch (lookAtLevel) {
            case LookLevelType.FirstLevel:
                LookAtFirstLevel();
                break;
            case LookLevelType.LastUnlocked:
                LookAtLastUnlockedLevel();
                break;
            case LookLevelType.LastCompleted:
                LookAtLastCompletedLevel();
                break;
            default:
                Debug.LogError("Unknown level type: " + lookAtLevel);
                break;
        }
    }

    private void LookAtLastCompletedLevel() {
        var lastCompleted =
            from l in MadLevel.activeConfiguration.levels
            where l.groupId == configurationGroup
                && l.type == MadLevel.Type.Level
                && MadLevelProfile.IsCompleted(l.name)
            orderby l.order descending
            select l;
        var lastCompletedLevel = lastCompleted.FirstOrDefault();
        if (lastCompletedLevel != null) {
            var lastCompletedIcon = MadLevelLayout.current.GetIcon(lastCompletedLevel.name);
            LookAtIcon(lastCompletedIcon);
        } else {
            LookAtFirstLevel();
        }
    }

    private void LookAtFirstLevel() {
        var firstIcon = MadLevelLayout.current.GetFirstIcon();
        LookAtIcon(firstIcon);
    }

    private void LookAtLastUnlockedLevel() {
        var firstUnlocked =
            from l in MadLevel.activeConfiguration.levels
            where l.groupId == configurationGroup
                && l.type == MadLevel.Type.Level
                && MadLevelProfile.IsLockedSet(l.name)
                && MadLevelProfile.IsLocked(l.name) == false
            orderby l.order descending
            select l;
        var firstUnlockedLevel = firstUnlocked.FirstOrDefault();
        if (firstUnlockedLevel != null) {
            var firstUnlockedIcon = MadLevelLayout.current.GetIcon(firstUnlockedLevel.name);
            LookAtIcon(firstUnlockedIcon);
        } else {
            LookAtFirstLevel();
        }
    }
    
    public void Activate(MadLevelIcon icon) {
        if (activeIcon != null && icon != activeIcon) {
            DeactivateActiveIcon();
        }
        
        activeIcon = icon;
        
        switch (twoStepActivationType) {
            case TwoStepActivationType.Disabled:
                if (!icon.locked) {
                    icon.LoadLevel();
                }
                break;
                
            case TwoStepActivationType.OnlyOnMobiles:
                if (SystemInfo.deviceType == DeviceType.Handheld) {
                    if (!icon.hasFocus) {
                        icon.hasFocus = true;
                        if (onIconActivate != null) {
                            onIconActivate(icon, icon.level.name);
                        }
                    } else if (!icon.locked) {
                        icon.LoadLevel();
                    }
                } else if (!icon.locked) {
                    icon.LoadLevel();
                }
                break;
                
            case TwoStepActivationType.Always:
                if (!icon.hasFocus) {
                    icon.hasFocus = true;
                    if (onIconActivate != null) {
                        onIconActivate(icon, icon.level.name);
                    }
                } else if (!icon.locked) {
                    icon.LoadLevel();
                }
                break;
                
            default:
                Debug.LogError("Uknown option: " + twoStepActivationType);
                break;
        }
    }
    
    void DeactivateActiveIcon() {
        var icon = activeIcon;
        activeIcon = null;
        
        if (onIconDeactivate != null) {
            onIconDeactivate(icon, icon.level.name);
        }
    }
    
    // ===========================================================
    // Static Methods
    // ===========================================================

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    public enum TwoStepActivationType {
        Disabled,
        OnlyOnMobiles,
        Always,
    }
    
    public enum OnMobileBack {
        LoadPreviousLevel,
        LoadSpecifiedLevel,
    }

    public enum LookLevelType {
        FirstLevel,
        LastUnlocked,
        LastCompleted,
    }
}

#if !UNITY_3_5
} // namespace
#endif