using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlMenu : MonoBehaviour
{
    public void Nivel(){
        SceneManager.LoadScene("Ciudad");
    }

    public void Menu(){
        SceneManager.LoadScene("Menu");
    }

    public void Controles(){
        SceneManager.LoadScene("Controles");
    }

    public void Creditos(){
        SceneManager.LoadScene("Creditos");
    }


    public void Salir(){
        Application.Quit();
    }

}
