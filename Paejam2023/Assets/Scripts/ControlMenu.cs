using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlMenu : MonoBehaviour
{
    public void Nivel(){
        SceneManager.LoadScene("Ciudad");
    }

    public void Salir(){
        Application.Quit();
    }

}
