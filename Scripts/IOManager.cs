using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class IOManager : MonoBehaviour
{
    #region File Names
    [Tooltip("玩家用户名信息存储地址")]
    [SerializeField]
    string userNameFileName = "/UserID.txt";
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WriteUsername(string username)
    {
        Debug.Log("Write Username: " + username);
        string filePath = Application.dataPath + userNameFileName;

        StreamWriter sw;

        FileInfo fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists)
        {
            sw = fileInfo.CreateText();
        }
        else
        {
            File.Delete(filePath);
            sw = fileInfo.CreateText();
        }
        sw.Write(username);
        sw.Close();
        sw.Dispose();

        //缓存玩家名字
        Debug.Log("save username: " + username);
        PlayerPrefs.SetString("username", username);
    }

    public string ReadUsername()
    {
        string filePath = Application.dataPath + userNameFileName;

        FileInfo fileInfo = new FileInfo(filePath);

        if (!fileInfo.Exists)
        {
            Debug.Log("Username file does not exist.");
            return null;
        }
        StreamReader sr = File.OpenText(filePath);
        string username = sr.ReadLine();
        sr.Close();
        sr.Dispose();
        return username;
    }

    public bool UsernameExist()
    {
        string filePath = Application.dataPath + userNameFileName;

        return File.Exists(filePath);
    }
}
