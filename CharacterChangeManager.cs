using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterChangeManager : MonoBehaviour {

    [SerializeField]
    private FadeManager fadeManager;
    [SerializeField]
    private GameObject _Target;

    [SerializeField]
    private float waitTime = 60;

    private void Reset()
    {
        if (!fadeManager)
            fadeManager = GameObject.Find("FadeManager").GetComponent<FadeManager>();
        if (!_Target)
            _Target = GameObject.FindWithTag("Model");
    }

    IEnumerator Start () {

        while (true)
        {

            yield return new WaitForSeconds(waitTime);
            if (_Target)
            {
                _Target.GetComponent<Animator>().SetTrigger("onWalk_01");
                yield return new WaitForSeconds(2.0f);
            }

            fadeManager.enableFade = true;
            fadeManager.enableFadeOut = true;
            
            // ModelにDontDestroy属性があるので強制的に削除
            if (_Target)
            {
                Destroy(_Target, 1f);
                yield return new WaitForSeconds(5);
            }
            
            // 現在のシーンナンバー+1を読み込む　上限だったら1に
            if (SceneManager.GetActiveScene().buildIndex +1 >= SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(1);
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }

        }
    }
}
