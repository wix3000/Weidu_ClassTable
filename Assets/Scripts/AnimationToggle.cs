using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationToggle: MonoBehaviour {

    [SerializeField]
    Animation animation;
    public string onAnimation, offAnimation;

    public void PlayAnimation(bool isOn) {
        animation.Play((isOn) ? onAnimation : offAnimation);
    }
}
