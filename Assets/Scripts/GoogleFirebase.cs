using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Unity.Editor;
using UnityEngine.UI;

public enum Scene
{
    Login,
    Profile
}

public class GoogleFirebase : Framework.Singleton<GoogleFirebase> {

    //Public Field
    [Header("Login")]
    private FirebaseUser user;
    private bool login;

    //Variable
    private Scene scene;
    private string error;

    //Check dependancy before launching other firebase services
    private void Init()
    {
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //   app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.

                InitAuth();
                InitDatabase();
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });
    }

	// Use this for initialization
	void Start () {
        Init();

        //Login
        scene = Scene.Login;
        LevelManager.Instance.InitLogin();
        login = false;
        error = "";
    }

    // Update is called once per frame
    void FixedUpdate () {
        switch(scene)
        {
            case Scene.Login:
               LevelManager.Instance.GetErrorText().text = error;

                if (login)
                {
                    scene = Scene.Profile;

                    FirebaseData.Instance.SetUser(user);
                    LevelManager.Instance.LoadProfile();
                }
                break;
            case Scene.Profile:
                LevelManager.Instance.GetErrorText().text = error;
                break;
        }
        
	}

    //Scene
    public Scene GetScene()
    {
        return scene;
    }

    //Function
    //Auth
    private void InitAuth()
    {
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    private void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        if (auth.CurrentUser != FirebaseData.Instance.GetUser())
        {
            bool signedIn = FirebaseData.Instance.GetUser() != auth.CurrentUser && auth.CurrentUser != null;
            if (!signedIn && FirebaseData.Instance.GetUser() != null)
            {
                Debug.Log("Signed out " + FirebaseData.Instance.GetUser().UserId);
            }
            FirebaseData.Instance.SetUser(auth.CurrentUser);
            if (signedIn)
            {
                Debug.Log("Signed in " + FirebaseData.Instance.GetUser().UserId);
                //displayName = user.DisplayName ?? "";
                //emailAddress = user.Email ?? "";
                //photoUrl = user.PhotoUrl ?? "";
            }
        }
    }

    public void Register()
    {
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Dictionary<string, InputField> result = LevelManager.Instance.GetLoginUI();
        string email = result["email"].text;
        string password = result["password"].text;

        Debug.Log(email + " " + password);
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                return;
            }
            if (task.IsFaulted)
            {
                GetErrorMessage(task);
                //Debug.Log(task.Exception);
                return;
            }

            // Firebase user has been created.
            Firebase.Auth.FirebaseUser newUser = task.Result;
            error = "";
        });
    }

    public void Login()
    {
        var auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        Dictionary<string, InputField> result = LevelManager.Instance.GetLoginUI();
        string email = result["email"].text;
        string password = result["password"].text;

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                return;
            }
            if (task.IsFaulted)
            {
                GetErrorMessage(task);
                return;
            }

            if(task.IsCompleted)
            {
                Firebase.Auth.FirebaseUser newUser = task.Result;
                error = "";

                user = newUser;
                login = true;
            }
        });
    }

    //Database
    private void InitDatabase()
    {
        // Set up the Editor before calling into the realtime database.
        Firebase.FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://unity-firebase-c505b.firebaseio.com/");

        // Get the root reference location of the database.
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public class User
    {
        public string uid, username, email, phone, level, position;

        public User()
        {
        }

        public User(string email)
        {
            this.email = email;
        }

        public User(string email, string name, string phone, string level, string position)
        {
            this.email = email;
            this.username = name;
            this.phone = phone;
            this.level = level;
            this.position = position;
        }

        public Dictionary<string, System.Object> ToDictionary()
        {
            Dictionary<string, System.Object> result = new Dictionary<string, System.Object>();
            result["email"] = email;
            result["username"] = username;
            result["phone"] = phone;
            result["level"] = level;
            result["position"] = position;
            return result;
        }
    }

    private void WriteNewUser(string userId, string email)
    {
        User user = new User(email);
        string json = JsonUtility.ToJson(user);

        DatabaseReference referrence = FirebaseDatabase.DefaultInstance.RootReference;
        referrence.Child("users").Child(userId).SetRawJsonValueAsync(json);
    }

    private void UpdateUser(string uid, string email, string name, string phone, string level, string position)
    {
        User user = new User(email, name, phone, level, position);
        DatabaseReference referrence = FirebaseDatabase.DefaultInstance.RootReference;

        Dictionary<string, System.Object> entryValues = user.ToDictionary();

        Dictionary<string, System.Object> childUpdates = new Dictionary<string, System.Object>();
        childUpdates["/users/" + uid] = entryValues;

        referrence.UpdateChildrenAsync(childUpdates).ContinueWith(task =>
        {
            if(task.IsCanceled)
            {

            }

            if(task.IsFaulted)
            {
                GetErrorMessage(task);
                return;
            }

            if(task.IsCompleted)
            {
                Debug.Log("Update Succcessfully");
            }
        });
    }

    public void UpdateUser()
    {
        //WriteNewUser(user.UserId, user.Email);
        Dictionary<string, InputField> result = LevelManager.Instance.GetProfileUI();
        string username = result["name"].text;
        string phone = result["phone"].text;
        string level = result["level"].text;
        string position = result["position"].text;

        UpdateUser(FirebaseData.Instance.GetUser().UserId, FirebaseData.Instance.GetUser().Email, username, phone, level, position);
        ReadUser();
    }

    public void ReadUser()
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").ValueChanged += HandleUserValueChanged;
    }

    void HandleUserValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        // Do something with the data in args.Snapshot
    }

    //Error
    private void GetErrorMessage(System.Threading.Tasks.Task task)
    {
        foreach (System.Exception exception in task.Exception.Flatten().InnerExceptions)
        {
            string authErrorCode = "";
            Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
            if (firebaseEx != null)
            {
                authErrorCode = System.String.Format("AuthError.{0}: ",
                  ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
            }
            //Debug.Log("number- " + authErrorCode + "the exception is- " + exception.ToString());

            string[] code = exception.ToString().Split(':'); //((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString();
            Debug.Log(code[1]);
            error = code[1];
        }

    }
}
