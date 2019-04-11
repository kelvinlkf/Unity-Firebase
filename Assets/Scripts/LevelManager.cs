using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : Framework.Singleton<LevelManager> {

    //Asyn Load Level
    AsyncOperation asyncLoadLevel;

    //Public Field
    [Header("Login")]
    public InputField field_email;
    public InputField field_password;
    public Text txt_error;
    private bool login;

    [Space]
    [Header("Profile")]
    public InputField field_name;
    public InputField field_phone;
    public InputField field_level;
    public InputField field_position;

    private void Start()
    {
        
    }

    //Error
    public Text GetErrorText()
    {
        return txt_error;
    }

    //Login
    public void InitLogin()
    {
        field_email = GameObject.Find("UI_Login_Email").GetComponent<InputField>();
        field_password = GameObject.Find("UI_Login_Password").GetComponent<InputField>();
        txt_error = GameObject.Find("UI_Txt_Error").GetComponent<Text>();

        field_email.text = "k3lvinlkf@gmail.com";
        field_password.text = "k3lvinwind";

        txt_error.text = "";

        //Button
        Button btn_register = GameObject.Find("UI_BTN_Register").GetComponent<Button>();
        Button btn_login = GameObject.Find("UI_BTN_Login").GetComponent<Button>();

        btn_register.onClick.RemoveAllListeners();
        btn_login.onClick.RemoveAllListeners();

        btn_register.onClick.AddListener(() => GoogleFirebase.Instance.Register());
        btn_login.onClick.AddListener(() => GoogleFirebase.Instance.Login());
    }

    public Dictionary<string, InputField> GetLoginUI()
    {
        Dictionary<string, InputField> result = new Dictionary<string, InputField>();
        result["email"] = field_email;
        result["password"] = field_password;

        return result;
    }

    //Profile
    public void InitProfile()
    {
        field_name = GameObject.Find("UI_Profile_Name").GetComponent<InputField>();
        field_phone = GameObject.Find("UI_Profile_Phone").GetComponent<InputField>();
        field_level = GameObject.Find("UI_Profile_Level").GetComponent<InputField>();
        field_position = GameObject.Find("UI_Profile_Position").GetComponent<InputField>();
        txt_error = GameObject.Find("UI_Txt_Error").GetComponent<Text>();

        //Button
        Button btn_update = GameObject.Find("UI_BTN_Update").GetComponent<Button>();

        btn_update.onClick.RemoveAllListeners();

        btn_update.onClick.AddListener(() => GoogleFirebase.Instance.UpdateUser());
    }

    public Dictionary<string, InputField> GetProfileUI()
    {
        Dictionary<string, InputField> result = new Dictionary<string, InputField>();
        result["name"] = field_name;
        result["phone"] = field_phone;
        result["level"] = field_level;
        result["position"] = field_position;

        return result;
    }

    public void LoadProfile()
    {
        StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
        string level = "";

        switch(GoogleFirebase.Instance.GetScene())
        {
            case Scene.Login:
                level = "Login";
                break;
            case Scene.Profile:
                level = "UserProfile";
                break;
        }

        asyncLoadLevel = SceneManager.LoadSceneAsync(level, LoadSceneMode.Single);
        while (!asyncLoadLevel.isDone)
        {
            print("Loading the Scene");
            yield return null;
        }

        switch (GoogleFirebase.Instance.GetScene())
        {
            case Scene.Login:
                InitLogin();
                break;
            case Scene.Profile:
                InitProfile();
                break;
        }
    }

}
