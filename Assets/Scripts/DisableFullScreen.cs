﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableFullScreen : MonoBehaviour {

    private void Awake() {
        Screen.fullScreen = false;
    }
}
