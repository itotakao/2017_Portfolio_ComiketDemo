using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;

public class DebugManager : MonoBehaviour {

    [SerializeField]
    private GUISkin guiSkin;  //ボタンのスタイル設定用 
    [SerializeField]
    private bool OnDebug = true;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private Vector2 rootPosition;
    [SerializeField]
    private GameObject _DebugTarget;

    [SerializeField]
    private SearchNearPlayer searchNearPlayer;
    [SerializeField]
    private HeadLookController headLookController;

    public static bool isKinectAvailable = false;
    private float moveLate = 0.1f;

    private void Start()
    {
        // 初期化
        _DebugTarget.SetActive(false);

        // Kinectが接続されているか確認
        KinectSensor sensor = KinectSensor.GetDefault();
        sensor.Open();
        if (sensor.IsOpen)
        {
            isKinectAvailable = sensor.IsAvailable;
        }

        if (isKinectAvailable)
        {
            gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        if (!isKinectAvailable)
        {

            if (OnDebug)
            {

                Vector3 moveValue = new Vector3(0, 0, 0);

                /*** ターゲット移動 ***/
                GUI.Label(new Rect(rootPosition.x + 30, rootPosition.y, 200, 40), "視点先移動", guiSkin.GetStyle("label"));
                if (GUI.Button(new Rect(rootPosition.x + 130, rootPosition.y + 40, 100, 100), "↑", guiSkin.GetStyle("button")))
                {
                    moveValue.y += moveLate;
                }
                if (GUI.Button(new Rect(rootPosition.x + 130, rootPosition.y + 240, 100, 100), "↓", guiSkin.GetStyle("button")))
                {
                    moveValue.y -= moveLate;
                }
                if (GUI.Button(new Rect(rootPosition.x + 30, rootPosition.y + 140, 100, 100), "←", guiSkin.GetStyle("button")))
                {
                    moveValue.x += moveLate;
                }
                if (GUI.Button(new Rect(rootPosition.x + 230, rootPosition.y + 140, 100, 100), "→", guiSkin.GetStyle("button")))
                {
                    moveValue.x -= moveLate;
                }

                if (moveValue.x != 0 || moveValue.y != 0)
                {
                    _DebugTarget.transform.position += moveValue;
                }

                /*** ターゲットの移動値を設定 ***/
                GUI.Label(new Rect(rootPosition.x + 30, rootPosition.y + 350, 200, 40), "移動力", guiSkin.GetStyle("label"));
                moveLate = GUI.HorizontalSlider(new Rect(rootPosition.x + 30, rootPosition.y + 390, 300, 30), moveLate, 0.01F, 1.0F);

                /*** アニメーション再生 ***/
                GUI.Label(new Rect(rootPosition.x + 30, rootPosition.y + 410, 200, 40), "アニメーション", guiSkin.GetStyle("label"));
                if (GUI.Button(new Rect(rootPosition.x + 30, rootPosition.y + 450, 100, 50), "手を振る"))
                {
                    animator.SetTrigger("onSmile_01");
                }
                if (GUI.Button(new Rect(rootPosition.x + 30, rootPosition.y + 500, 100, 50), "見つめる"))
                {
                    animator.SetTrigger("onShy_01");
                }

                if (GUI.Button(new Rect(rootPosition.x + 30, rootPosition.y + 550, 100, 50), "覗く"))
                {
                    animator.SetTrigger("onTrick_01");
                }
                if (GUI.Button(new Rect(rootPosition.x + 30, rootPosition.y + 600, 100, 50), "胸A"))
                {
                    animator.SetTrigger("onBreast_01");
                }
                if (GUI.Button(new Rect(rootPosition.x + 30, rootPosition.y + 650, 100, 50), "胸B"))
                {
                    animator.SetTrigger("onBreast_02");
                }
                if (GUI.Button(new Rect(rootPosition.x + 30, rootPosition.y + 700, 100, 50), "体A"))
                {
                    animator.SetTrigger("onBody_01");
                }
                if (GUI.Button(new Rect(rootPosition.x + 30, rootPosition.y + 750, 100, 50), "体B"))
                {
                    animator.SetTrigger("onBody_02");
                }
                //ここはいずれ自動読み込みに変更としようと思っていたけどパラメーターきまっているのでこれで
            }
        }
        
    }

	void LateUpdate () {
        if (!isKinectAvailable)
        {
                if (!_DebugTarget.activeSelf)
                    _DebugTarget.SetActive(true);

                searchNearPlayer._NearPlayer = _DebugTarget;
                searchNearPlayer._NearPlayerHead = _DebugTarget;
        }
	}
}
