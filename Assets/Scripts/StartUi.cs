using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartUi : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Sound.Instance.PlayBG("1");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScen(int value) {

        SceneManager.LoadScene(value);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
