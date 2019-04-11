using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;

public class FirebaseData : Framework.Singleton<FirebaseData> {

    private FirebaseUser user;

    public void SetUser(FirebaseUser user)
    {
        this.user = user;
    }

    public FirebaseUser GetUser()
    {
        return user;
    }
}
