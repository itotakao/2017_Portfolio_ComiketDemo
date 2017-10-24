using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キネクトで認識されている中で一番近くにいるプレイヤーを見つける
/// </summary>
public class SearchNearPlayer : MonoBehaviour {

    KinectManager kinectManager;
    [SerializeField]
    private GameObject[] _CubemanList = new GameObject[6];
    [SerializeField]
    private GameObject[] _CubemanHeadList = new GameObject[6];
    private int holdPlayerCount;

    [Header("出力結果")]
    public long userID;
    public int userNumber;
    public GameObject _NearPlayer;
    public GameObject _NearPlayerHead;

    private void Start()
    {
        if (!kinectManager)
        {
            kinectManager = GameObject.Find("KinectManager").GetComponent<KinectManager>();
        }
    }

    void Update() {

        // 初期化
        _NearPlayer = null;
        _NearPlayerHead = null;

        // Kinectの認識されているユーザー数を読み込み
        List<long> userList = new List<long>();

        try
        {
            userList = kinectManager.GetAllUserIds();
        }
        catch
        {
            // NiceCatch;
        }
        if (userList.Count != 0)
        {
            // 初期化
            float _NearPlayerPos = 1000; // if文用
            
            
            // 認識されているプレイヤーを探索
            for (int i = 0; i < userList.Count; i++)
            {
                // 正確に認識されているか
                if (KinectInterop.TrackingState.Tracked == kinectManager.GetJointTrackingState(userList[i], (int)KinectInterop.JointType.SpineBase)){
                    // 一番近くいるプレイヤー情報だけ格納
                    if (_CubemanList[i].transform.position.z < _NearPlayerPos)
                    {
                        userID = userList[i];
                        userNumber = i;
                        _NearPlayerPos = _CubemanList[i].transform.position.z;
                        _NearPlayer = _CubemanList[i];
                        _NearPlayerHead = _CubemanHeadList[i];
                    }
                }
            }
        }
        else
        {
            // 初期化
            if (_NearPlayer)
            {
                _NearPlayer = null;
            }
        }
    }
}
