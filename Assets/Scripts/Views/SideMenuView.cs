using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SideMenuView : MonoBehaviour {

    [SerializeField]
    Animation animation;
    public string onAnimation, offAnimation;

    [SerializeField]
    InputField nameInput;
    [SerializeField]
    RectTransform classList;

    public bool userInterfaceActive {
        get {
            return GetComponent<GraphicRaycaster>().enabled;
        }
        set {
            GetComponent<GraphicRaycaster>().enabled = value;
        }
    }

    private void Awake() {
        if (gameObject.activeSelf) { gameObject.SetActive(false); }
    }

    // Use this for initialization
    void Start () {
		
	}

    private void OnEnable() {
        nameInput.text = UserSetting.userName;
    }

    public void SetEnable(bool isOn) {
        if (isOn) {
            gameObject.SetActive(true);
            userInterfaceActive = true;
            animation.Play(onAnimation);
        } else {
            userInterfaceActive = false;
            animation.Play(offAnimation);
        }
    }

    public void AnimationEvent_Hide() {
        gameObject.SetActive(false);
    }

    public void OnUserNameEndEdit(string value) {
        if (string.IsNullOrWhiteSpace(value)) {
            nameInput.text = UserSetting.userName;
            return;
        }

        UserSetting.userName = nameInput.text;
    }
}

public static class UserSetting {
    const string UserNameKey = "userName";

    public static string userName {
        get {
            return PlayerPrefs.GetString(UserNameKey, "Wix Litariz");
        }
        set {
            PlayerPrefs.SetString(UserNameKey, value);
        }
    }
}