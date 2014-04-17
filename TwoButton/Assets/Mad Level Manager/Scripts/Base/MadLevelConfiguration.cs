/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MadLevelManager;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if !UNITY_3_5
namespace MadLevelManager {
#endif

[ExecuteInEditMode]
public class MadLevelConfiguration : ScriptableObject {

    // ===========================================================
    // Constants
    // ===========================================================

    // ===========================================================
    // Fields
    // ===========================================================
    
    // only one configuration can be active at time
    [SerializeField]
    private bool _active;
    
    public List<Level> levels = new List<Level>();
    public List<Group> groups = new List<Group>();
    public List<MadLevelExtension> extensions = new List<MadLevelExtension>();
    
    [NonSerialized]
    public Callback0 callbackChanged = () => {};
    
    // to prevent activation of everything that is found
    // sadly OnEnable() activation method can activate too much for some reason
    private static bool automaticallyActivatedSomething;
    
    // ===========================================================
    // Properties
    // ===========================================================
    
    public bool active {
        get {
            return _active;
        }
        
        set {
            _active = value;
            if (value) {
                DeactivateOthers();
            }
            SetDirty();
        }
    }
    
    public Group defaultGroup {
        get {
            if (_defaultGroup == null || _defaultGroup.parent == null) {
                _defaultGroup = new Group(this, 0);
                _defaultGroup.name = "(default)";
            }
            
            return _defaultGroup;
        }
    }
    private Group _defaultGroup;
    
    // ===========================================================
    // Methods for/from SuperClass/Interfaces
    // ===========================================================
    
    public override int GetHashCode() {
        int hash = 17;
    
        foreach (var level in levels) {
            hash = hash * 31 + level.GetHashCode();
        }
        
        foreach (var group in groups) {
            hash = hash * 31 + group.GetHashCode();
        }
        
        return hash;
    }

    // ===========================================================
    // Methods
    // ===========================================================
    
    void OnEnable() {
        Upgrade();
    }
    
    void Upgrade() {
        if (levels != null) {
            foreach(var level in levels) {
                level.parent = this;
                level.Upgrade();
            }
        }
    }
    
    public new void SetDirty() {
        Reorder();
        
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif
        callbackChanged();
    }
    
    // maintains order
    void Reorder() {
        int order = 0;
        var ordered = GetLevelsInOrder();
        foreach (var level in ordered) {
            level.order = order;
            order += 10;
        }
    }
    
    public Level CreateLevel() {
        return new Level(this);
    }
    
    public Group CreateGroup() {
        int nextId = 1;
        foreach (var group in groups) {
            nextId = Mathf.Max(nextId, group.id + 1);
        }
        
        return new Group(this, nextId);
    }
    
    public void AddGroup(Group group) {
        groups.Add(group);
        SetDirty();
    }
    
    public void RemoveGroup(Group group) {
        MadDebug.Assert(groups.Contains(group), "There's no such group");
    
        var dGroup = defaultGroup;
        var levels = group.GetLevels();
        foreach (var level in levels) {
            level.groupId = dGroup.id;
        }
        
        groups.Remove(group);
        SetDirty();
    }
    
    public Group FindGroupById(int groupId) {
        if (groupId == defaultGroup.id) {
            return defaultGroup;
        } else {
            var gr = from g in groups where g.id == groupId select g;
            return gr.FirstOrDefault();
        }
    }
    
    public Group FindGroupByName(string groupName) {
        if (defaultGroup.name == groupName) {
            return defaultGroup;
        } else {
            var gr = from g in groups where g.name == groupName select g;
            return gr.FirstOrDefault();
        }
    }
    
    public int LevelCount() {
        return levels.Count;
    }
    
    public int LevelCount(MadLevel.Type type) {
        return LevelCount(type, -1);
    }

    public int LevelCount(MadLevel.Type type, int groupId) {
        if (groupId == -1) {
            var query = from level in levels where level.type == type select level;
            return query.Count();
        } else {
            var query = from level in levels where level.type == type && level.groupId == groupId select level;
            return query.Count();
        }
    }
    
    public Level[] GetLevelsInOrder() {
        var query = from l in levels orderby l.groupId, l.order ascending select l;
        return query.ToArray();
    }

    public Level GetLevel(int index) {
        return GetLevel(-1, index);
    }

    public Level GetLevel(int groupId, int index) {
        Level[] arr;

        if (groupId == -1) {
            var query = from l in levels orderby l.order ascending select l;
            arr = query.ToArray();
        } else {
            var query = from l in levels where l.groupId == groupId orderby l.order ascending select l;
            arr = query.ToArray();
        }

        if (arr.Length > index) {
            return arr[index];
        } else {
            return null;
        }
    }
    
    public Level GetLevel(MadLevel.Type type, int index) {
        return GetLevel(type, -1, index);
    }
    
    public Level GetLevel(MadLevel.Type type, int groupId, int index) {
        Level[] queryResult;
        
        if (groupId == -1) {
            var query = from l in levels where l.type == type orderby l.order ascending select l;
            queryResult = query.ToArray();
        } else {
            var query = from l in levels where l.type == type && l.groupId == groupId orderby l.order ascending select l;
            queryResult = query.ToArray();
        }
    
        int skipped = 0;
        foreach (var level in queryResult) {
            if (skipped == index) {
                return level;
            } else {
                skipped++;
            }
        }
        
        return null;
    }
    
    public int FindLevelIndex(MadLevel.Type type, string levelName) {
        return FindLevelIndex(type, -1, levelName);
    }
    
    public int FindLevelIndex(MadLevel.Type type, int groupId, string levelName) {
        var query = from l in levels where l.type == type orderby l.order ascending select l;
        
        int index = 0;
        foreach (var level in query) {
            if (level.name == levelName) {
                return index;
            }
            
            index++;
        }
        
        return -1;
    }
    
    public Level FindLevelByName(string levelName) {
        var query = from l in levels where l.name == levelName select l;
        var first = query.FirstOrDefault();
        return first;
    }

    //public Level FindLevelByGUID(string guid) {
    //    var query = from l in levels where l.guid == guid select l;
    //    var first = query.FirstOrDefault();
    //    return first;
    //}

    public Level FindNextLevel(string currentLevelName) {
        return FindNextLevel(currentLevelName, false);
    }

    public Level FindNextLevel(string currentLevelName, bool sameGroup) {
        var currentLevel = FindLevelByName(currentLevelName);
        MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);

        if (sameGroup) {
            var nextLevelQuery =
                from l in levels
                where l.groupId == currentLevel.groupId && l.order > currentLevel.order
                orderby l.order ascending
                select l;

            var nextLevel = nextLevelQuery.FirstOrDefault();
            return nextLevel;
        } else {
            var nextLevelQuery =
                from l in levels
                where l.order > currentLevel.order
                orderby l.order ascending
                select l;

            var nextLevel = nextLevelQuery.FirstOrDefault();
            return nextLevel;
        }
    }

    public Level FindNextLevel(string currentLevelName, MadLevel.Type type) {
        return FindNextLevel(currentLevelName, type, false);
    }

    public Level FindNextLevel(string currentLevelName, MadLevel.Type type, bool sameGroup) {
        var currentLevel = FindLevelByName(currentLevelName);
        MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);

        if (sameGroup) {
            var nextLevelQuery =
                from l in levels
                where l.groupId == currentLevel.groupId && l.order > currentLevel.order && l.type == type
                orderby l.order ascending
                select l;

            var nextLevel = nextLevelQuery.FirstOrDefault();
            return nextLevel;
        } else {
            var nextLevelQuery =
                from l in levels
                where l.order > currentLevel.order && l.type == type
                orderby l.order ascending
                select l;

            var nextLevel = nextLevelQuery.FirstOrDefault();
            return nextLevel;
        }
    }

    public Level FindPreviousLevel(string currentLevelName) {
        return FindPreviousLevel(currentLevelName, false);
    }

    public Level FindPreviousLevel(string currentLevelName, bool sameGroup) {
        var currentLevel = FindLevelByName(currentLevelName);
        MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);

        if (sameGroup) {
            var prevLevelQuery =
                from l in levels
                where l.groupId == currentLevel.groupId && l.order < currentLevel.order
                orderby l.order descending
                select l;

            var prevLevel = prevLevelQuery.FirstOrDefault();
            return prevLevel;
        } else {
            var prevLevelQuery =
                from l in levels
                where l.order < currentLevel.order
                orderby l.order descending
                select l;

            var prevLevel = prevLevelQuery.FirstOrDefault();
            return prevLevel;
        }
    }

    public Level FindPreviousLevel(string currentLevelName, MadLevel.Type type) {
        return FindPreviousLevel(currentLevelName, type, false);
    }

    public Level FindPreviousLevel(string currentLevelName, MadLevel.Type type, bool sameGroup) {
        var currentLevel = FindLevelByName(currentLevelName);
        MadDebug.Assert(currentLevel != null, "Cannot find level " + currentLevelName);

        if (sameGroup) {
            var prevLevelQuery =
                from l in levels
                where l.groupId == currentLevel.groupId && l.order < currentLevel.order && l.type == type
                orderby l.order descending
                select l;

            var prevLevel = prevLevelQuery.FirstOrDefault();
            return prevLevel;
        } else {
            var prevLevelQuery =
                from l in levels
                where l.order < currentLevel.order && l.type == type
                orderby l.order descending
                select l;

            var prevLevel = prevLevelQuery.FirstOrDefault();
            return prevLevel;
        }
    }
    
    public Level FindFirstForScene(string levelName) { // TODO: look for index
        var ordered =
            from l in levels
            orderby l.order ascending
            select l;
        
        foreach (var level in ordered) {
            if (level.sceneName == levelName) {
                return level;
            }
        }
        
        return null;
    }

    public MadLevelExtension FindExtensionByGUID(string guid) {
        var query =
            from e in extensions
            where e.guid == guid
            select e;
        return query.FirstOrDefault();
    }

    List<MadLevelScene> ScenesInOrder() {
        List<MadLevelScene> all = new List<MadLevelScene>();
        var levelsQuery = from l in levels orderby l.groupId, l.order ascending select l;
        foreach (var level in levelsQuery) {
            all.Add(level);
        }

        foreach (var extension in extensions) {
            all.AddRange(extension.scenesBefore);
            all.AddRange(extension.scenesAfter);
        }

        return all;
    }
    
#if UNITY_EDITOR
    public bool CheckBuildSynchronized() {
        var scenes = EditorBuildSettings.scenes;
        
        if (levels.Count == 0) {
            // do not synchronize anything if it's nothing there
            return true;
        }
        
        if (scenes.Length == 0 && levels.Count > 0 || scenes.Length > 0 && levels.Count == 0) {
//            Debug.Log("Failed size test");
            return false;
        }
        
        if (scenes.Length == 0 && levels.Count == 0) {
            return true;
        }
        
        var firstLevel = GetLevel(0);
        
        // check if first scene is my first scene
        if (scenes[0].path != firstLevel.scenePath) {
//            Debug.Log("Different start scene");
            return false;
        }
        
        // find all configuration scenes that are not in build
        List<MadLevelScene> allScenes = new List<MadLevelScene>();

        foreach (var level in levels) {
            allScenes.Add(level);
        }

        foreach (var extension in extensions) {
            allScenes.AddRange(extension.scenesBefore);
            allScenes.AddRange(extension.scenesAfter);
        }

        foreach (var level in allScenes) {
            if (!level.IsValid()) {
                continue;
            }
        
            var obj = Array.Find(scenes, (scene) => scene.path == level.scenePath);
            if (obj == null) {  // scene not found in build
//                Debug.Log("Scene not found in build: " + item.level.scene);
                return false;
            }
        }
        
        return true;
    }
    
    public void SynchronizeBuild() {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>();
        foreach (var configuredScene in ScenesInOrder()) {
            if (!configuredScene.IsValid()) {
                continue;
            }
        
            string path = configuredScene.scenePath;
            if (scenes.Find((obj) => obj.path == path) == null) {
                var scene = new EditorBuildSettingsScene(path, true);
                scenes.Add(scene);
            }
        }
        
        EditorBuildSettings.scenes = scenes.ToArray();
    }
#endif
    
    // ===========================================================
    // Static Methods
    // ===========================================================
    
    public static MadLevelConfiguration[] FindAll() {
        List<MadLevelConfiguration> output = new List<MadLevelConfiguration>();
        var configurations = Resources.LoadAll("LevelConfig", typeof(MadLevelConfiguration));
        
        foreach (var conf in configurations) {
            output.Add(conf as MadLevelConfiguration);
        }
        
        return output.ToArray();
    }
    
    public static MadLevelConfiguration GetActive() {
        var active = FindActive();
        if (active == null) {
            Debug.LogWarning("There's no active configuration. Please make at least one!");
        }
        return active;
    }
    
    public static MadLevelConfiguration FindActive() {
        var all = FindAll();
        var active = from conf in all where conf.active == true select conf;
        
        var configuration = active.FirstOrDefault();
        
        if (active.Count() > 1) {
            Debug.Log("There are more than one active configuration. "
                + "This shouldn't happen, but there's nothing to worry about. I will now use " + configuration.name
                + " and deactivate others.", configuration);
            configuration.active = true;
        }
        
        return configuration;
    }
    
    void DeactivateOthers() {
        var all = FindAll();
        foreach (var conf in all) {
            if (conf != this) {
                conf.active = false;
            }
        }
    }
    
    public bool IsValid() {
        foreach (var level in levels) {
            if (!level.IsValid()) {
                return false;
            }
            
            if (level.sceneObject == null) {
                return false;
            }
        }
        
        return true;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================
    
    [System.Serializable]
    public class Group {
    
        public string name = "New Group";
    
        public int id {
            get {
                return _id;
            }
            private set {
                _id = value;
            }
        }
        [SerializeField]
        private int _id;
        
        [SerializeField] internal MadLevelConfiguration parent;
        
        internal Group(MadLevelConfiguration parent, int id) {
            MadDebug.Assert(parent != null, "Parent cannot be null");
            this.parent = parent;
            this.id = id;
        }
        
        public List<Level> GetLevels() {
            var levels = from l in parent.levels where l.groupId == id select l;
            return levels.ToList();
        }
        
        public override int GetHashCode () {
            int hash = 17;
            hash = hash * 31 + (name != null ? name.GetHashCode() : 0);
            hash = hash * 31 + id.GetHashCode();
            return hash;
        }
    }
    
    [System.Serializable]
    public class Level : MadLevelScene {
    
        // fields
        [SerializeField] internal MadLevelConfiguration parent;
        public int order;
        
        public string name = "New Level";
        public MadLevel.Type type;
        public string arguments = "";
        public int groupId;
        //public string guid;

        public string extensionGUID = "";
        
        
        
        public Group group {
            get {
                Group g = parent.FindGroupById(groupId);
                return g;
            } set {
                MadDebug.Assert(value == parent.defaultGroup || parent.groups.Contains(value),
                    "Unknown group: " + value);
                groupId = value.id;
            }
        }

        public bool hasExtension {
            get {
                return extension != null;
            }
        }

        public MadLevelExtension extension {
            get {
                return parent.FindExtensionByGUID(extensionGUID);
            }

            set {
                if (value == null) {
                    extensionGUID = "";
                } else {
                    int index = parent.extensions.FindIndex((e) => e == value);
                    if (index != -1) {
                        extensionGUID = value.guid;
                    } else {
                        Debug.LogError("Trying to add extesion to a level that is not in the configuration");
                    }
                }
            }
        }


        internal Level(MadLevelConfiguration parent) {
            this.parent = parent;
        }

        public override void Load() {
            MadLevel.lastPlayedLevelName = MadLevel.currentLevelName;
            // arguments must be set after reading currentLevelName
            // because reading it may overwrite arguments in result
            // TODO: find a better way to solve this

            MadLevel.arguments = arguments;
            MadLevel.currentLevelName = name;
            Application.LoadLevel(sceneName); // TODO: change it to scene index
        }

        public override AsyncOperation LoadAsync() {
            MadLevel.lastPlayedLevelName = MadLevel.currentLevelName;
            // arguments must be set after reading currentLevelName
            // because reading it may overwrite arguments in result
            // TODO: find a better way to solve this

            MadLevel.arguments = arguments;
            MadLevel.currentLevelName = name;
            return Application.LoadLevelAsync(sceneName); // TODO: change it to scene index
        }
        
        public override bool IsValid() {
            if (!base.IsValid()) {
                return false;
            }

            return !string.IsNullOrEmpty(name) && !HasDuplicatedName();
        }
        
        public bool HasDuplicatedName() {
            foreach (var otherLevel in parent.levels) {
                if (otherLevel == this) {
                    continue;
                }
                
                if (otherLevel.name == name) {
                    return true;
                }
            }
            
            return false;
        }
        
        public override int GetHashCode () {
            int hash = 17;
            hash = hash * 31 + order.GetHashCode();
            hash = hash * 31 + (sceneObject != null ? sceneObject.GetHashCode() : 0);
            hash = hash * 31 + name.GetHashCode();
            hash = hash * 31 + type.GetHashCode();
            hash = hash * 31 + arguments.GetHashCode();
            
            return hash;
        }
    }
    
    public delegate void Callback0();

}

#if !UNITY_3_5
} // namespace
#endif