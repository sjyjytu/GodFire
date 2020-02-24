using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldManager : MonoBehaviour
{
    public InputField usernameInputField;
    public IOManager ioManager;

    // Start is called before the first frame update
    void Start()
    {
        if (ioManager.UsernameExist())
        {
            string username = ioManager.ReadUsername();
            usernameInputField.text = username;
            //缓存玩家名字
            Debug.Log("save username: " + username);
            PlayerPrefs.SetString("username", username);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
