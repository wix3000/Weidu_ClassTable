using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationView : MonoBehaviour {

    [SerializeField]
    SideMenuView sideMenuView;

	// Use this for initialization
	void Start () {
        ClassTableFetcher.CreateFetch(UserSetting.userName, System.DateTime.Today, FetchType.Teacher, f =>
        {
            if (f == null) {
                print("Fetch Error");
                return;
            }

            foreach (var info in f.analyzer.result) {
                print(info);
            }
        });
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
