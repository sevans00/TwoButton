/*
* Mad Level Manager by Mad Pixel Machine
* http://www.madpixelmachine.com
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MadLevelManager {

public class MadHashCode {

    // ===========================================================
    // Constants
    // ===========================================================

    public const int FirstPrime = 37;
    public const int SecondPrime = 13;

    // ===========================================================
    // Fields
    // ===========================================================
    
    int currentHash;
    
    // ===========================================================
    // Constructors
    // ===========================================================
    
    public MadHashCode() {
        currentHash = FirstPrime;
    }

    // ===========================================================
    // Methods
    // ===========================================================

    public void Add(object obj) {
        currentHash = Add(currentHash, obj);
    }

    public void AddArray(object[] arr) {
        currentHash = AddArray(currentHash, arr);
    }

    public void AddList<T>(List<T> list) {
        currentHash = AddList(currentHash, list);
    }
    
    public void AddEnumerable(IEnumerable enumerable) {
        if (enumerable == null) {
            Add(null);
            return;
        }
        
        foreach (var obj in enumerable) {
            Add(obj);
        }
    }
    
    public override int GetHashCode() {
        return currentHash;
    }

    // ===========================================================
    // Static Methods
    // ===========================================================

    public static int Add(int currentHash, bool a) {
        return currentHash * SecondPrime + a.GetHashCode();
    }

    public static int Add(int currentHash, int a) {
        return currentHash * SecondPrime + a.GetHashCode();
    }

    public static int Add(int currentHash, float a) {
        return currentHash * SecondPrime + a.GetHashCode();
    }

    public static int Add(int currentHash, double a) {
        return currentHash * SecondPrime + a.GetHashCode();
    }

    public static int Add(int currentHash, object obj) {
        return currentHash * SecondPrime + (obj != null ? obj.GetHashCode() : 0);
    }

    public static int AddArray(int currentHash, object[] arr) {
        if (arr == null) {
            return Add(currentHash, null);
        }

        int c = arr.Length;
        for (int i = 0; i < c; ++i) {
            currentHash = Add(currentHash, arr[i]);
        }

        return currentHash;
    }

    public static int AddList<T>(int currentHash, List<T> list) {
        if (list == null) {
            return Add(currentHash, null);
        }

        int c = list.Count;
        for (int i = 0; i < c; ++i) {
            currentHash = Add(currentHash, list[i]);
        }

        return currentHash;
    }

    // ===========================================================
    // Inner and Anonymous Classes
    // ===========================================================

}

} // namespace