using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleLoad : MonoBehaviour {

    [SerializeField]
    private int loadSceneNumber;

	void Start () {
        SceneManager.LoadScene(loadSceneNumber);
    }
	
}
