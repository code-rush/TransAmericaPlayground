using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class music : MonoBehaviour {

    void Awake() {
        GameObject obj = GameObject.FindGameObjectWithTag("music");

        DontDestroyOnLoad(this.gameObject);
    }
}
