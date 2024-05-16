/*******
 * DUCK HUNT
 * Version : 1.0 (part 2 of clip)
 * Hoang Minh Quan
 * http://khoahocvui.vn
 * *****/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using UnityEngine.UI;
using System;

public class ComController : MonoBehaviour
{
    public static SerialPort spCom;
    public Dropdown dropBoxPort;
    public Text lbMsg;
    public int readTimeOut = 500; // wait time out : 500ms
   
    // Start is called before the first frame update
    void Start()
    {
        lbMsg.text = "";

        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            dropBoxPort.options.Add(new Dropdown.OptionData(port));
        }
    }

    private void OnDestroy()
    {
        spCom.Close();
    }
    // Create A Port
    public void CreatePortWithCallback(Action act)
    {
        string value = dropBoxPort.options[dropBoxPort.value].text;
        spCom = new SerialPort(value, 9600 , Parity.None, 8, StopBits.One);
        bool isOK = false;
        if (spCom != null)
        {
            Debug.Log("COM NOW : " + spCom.IsOpen);
            if (!spCom.IsOpen)
            {
               
                try
                {
                    spCom.Open();
                    spCom.ReadTimeout = readTimeOut; // wait time out : 500ms
                    if (spCom.ReadByte() > 0)
                    {
                        isOK = true;
                        act.Invoke();
                        Debug.Log(value + " OPENED!");
                    }
                }
                catch
                {
                    lbMsg.text = "ERROR PORT!";
                }
            }
        }

        if (!isOK)
            lbMsg.text = "PORT IS NOT READY!";
    }

}
