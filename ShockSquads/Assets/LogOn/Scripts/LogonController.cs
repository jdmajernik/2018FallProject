using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LogonController : MonoBehaviour
{

    //Gui Public Variables
    public Text UsernameText;
    public Text PasswordText;
    public Text WrongLogin;
    public Text UsernameTaken;

    //Variables for Scripting
    [SerializeField] private GameObject gameController;
    private DatabaseDebug debugger;
    public string ServerIP = "71.210.130.40";
    public string Port = "8080";

    private string connection;
    private string username;
    private string password;
    WWW serverConnect;

    private bool DebuggerActive = true;
    private GameObject NetworkDebugger;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag("GameController");
        debugger = gameController.GetComponent<DatabaseDebug>();
        WrongLogin.gameObject.SetActive(false); //deactivates the incorrect login if it hasn't been already
        //connection = "http://" + ServerIP + ":" + Port;
        connection = "http://"+ ServerIP +":8080";
        StartCoroutine(startupServerConnect("DBConnected"));
    }
    //All the button Functions
    public void LogInButton()
    {
        WrongLogin.gameObject.SetActive(false); //deactivates the incorrect login if it hasn't been already
        //converts the username and password text inputs into strings
        username = UsernameText.text;
        password = PasswordText.text;
        //executes LogIn function
        StartCoroutine(logIn("loggedIn"));

    }
    public void CreateAccountButton()
    {
        UsernameTaken.gameObject.SetActive(false);
        //converts the username and password text inputs into strings
        username = UsernameText.text;
        password = PasswordText.text;
        //executes LogIn function
        StartCoroutine(createAccount("EXISTS"));
    }
    //All the Server connections
    IEnumerator startupServerConnect(string text)
    {
        using (serverConnect = new WWW(connection + "/unityTest.php"))
        {
            yield return serverConnect;
            Debug.Log(serverConnect.text);
            if (Parse(serverConnect.text, text))
            {
                debugger.ServerConnected();
            }
        }
    }
    IEnumerator logIn(string text)
    {
        WWWForm testForm = new WWWForm();
        testForm.AddField("Username", username);
        testForm.AddField("Password", password);
        //testForm.AddField("Test2", "not really sure what this does");
        using (serverConnect = new WWW(connection + "/testLogin.php", testForm))
        {
            yield return serverConnect;
            Debug.Log(serverConnect.text);
            if (Parse(serverConnect.text, text))
            {
                debugger.LoggedIn();
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
            else if (Parse(serverConnect.text, "NOTFOUND"))
            {
                WrongLogin.gameObject.SetActive(true);
            }
        }
    }
    IEnumerator createAccount(string text)
    {
        WWWForm testForm = new WWWForm();
        testForm.AddField("Username", username);
        testForm.AddField("Password", password);
        //testForm.AddField("Test2", "not really sure what this does");
        using (serverConnect = new WWW(connection + "/createAccount.php", testForm))
        {
            yield return serverConnect;
            Debug.Log(serverConnect.text);
            if (Parse(serverConnect.text, text))
            {
                UsernameTaken.gameObject.SetActive(true);
            }
        }
    }
    public bool Parse(string text, string phrase)
    {
        char terminatorChar = '#';
        string[] text_broken = text.Split(terminatorChar); //the output text broken by breakpoints
        foreach (string texts in text_broken)
        {
            if (phrase.Equals(texts))
            {
                return true;
            }
        }
        return false;
    }
    //override for parse if you want to change the termiantor character for any reason
    public bool Parse(string text, string phrase, char terminatorChar)
    {
        string[] text_broken = text.Split(terminatorChar); //the output text broken by breakpoints
        foreach (string texts in text_broken)
        {
            if (phrase.Equals(texts))
            {
                return true;
            }
        }
        return false;
    }
}
