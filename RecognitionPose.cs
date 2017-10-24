using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecognitionPose : MonoBehaviour {
    
    private KinectManager kinectManager;
    [SerializeField]
    private SearchNearPlayer searchNearPlayer;
    [SerializeField]
    private GetRaycast[] _UserRaycastList = new GetRaycast[6];
    [SerializeField]
    private CubemanController[] cubemanController = new CubemanController[6];
    [SerializeField]
    Animator animator;

    [Header("デバック用")]
    [SerializeField]
    private float riseHandTime;
    [SerializeField]
    private float lookTime;
    [SerializeField]
    private float skirtTime;
    [SerializeField]
    private float nearPositionTime;

    private float waitTime = 0.5f;

    private bool isBodyTouch;
    private bool isHoldBodyTouch;

    AnimatorClipInfo clipInfo;
    string clipName;

    IEnumerator Start()
    {
        // 初期化
        if (!kinectManager)
        {
            kinectManager = GameObject.Find("KinectManager").GetComponent<KinectManager>();
        }

        while (true)
        {
            
            // Kinectでユーザーが認証されていたら
            if (searchNearPlayer._NearPlayer)
            {
                Vector3 rightHandPos = kinectManager.GetJointKinectPosition(searchNearPlayer.userID, (int)KinectInterop.JointType.HandRight);
                Vector3 leftHandPos = kinectManager.GetJointKinectPosition(searchNearPlayer.userID, (int)KinectInterop.JointType.HandLeft);
                Vector3 shoulderPos = kinectManager.GetJointKinectPosition(searchNearPlayer.userID, (int)KinectInterop.JointType.SpineShoulder);

                /* --- 手を振る認識 --- */
                // waitTime以上 手をあげていたらアクション
                if (riseHandTime > waitTime)
                {
                    if (animator)//TODO:これ関数化できるから元気な時にやる
                    {
                        
                        if (cubemanController[searchNearPlayer.userNumber])
                        {
                            if (cubemanController[searchNearPlayer.userNumber].useAnimationName != "smile")
                            {
                                
                                animator.SetTrigger("onSmile_01");
                                yield return new WaitForSeconds(1);
                                SaveAnimationName();
                            }
                        }
                    }
                    yield return new WaitForSeconds(4);
                    Initialized();
                }
                // 右手と左手の手が上がっていたら
                else if (rightHandPos.y >= shoulderPos.y - 0.2f || leftHandPos.y >= shoulderPos.y - 0.2f)// 胸よりちょっと上ぐらい
                {
                    if (waitTime >= riseHandTime)
                        {
                            riseHandTime += Time.deltaTime;
                    }
                }
                
                // それ以外
                else
                {
                    if (riseHandTime != 0)
                        riseHandTime = 0;
                }
                

                /*--- 見つめられる ---*/
                if (_UserRaycastList[searchNearPlayer.userNumber].hitRaycastName == "HeadCollision")
                {
                    if (cubemanController[searchNearPlayer.userNumber].isTrackShy)
                    {
                        
                        if (lookTime > waitTime + 1)
                        {
                            if (animator)//TODO:これ関数化できるから元気な時にやる
                            {
                                if (cubemanController[searchNearPlayer.userNumber])
                                {
                                    if (cubemanController[searchNearPlayer.userNumber].useAnimationName != "shy")
                                    {
                                        
                                        animator.SetTrigger("onShy_01");
                                        yield return new WaitForSeconds(1);
                                        SaveAnimationName();
                                    }
                                }
                            }
                            yield return new WaitForSeconds(4);
                            cubemanController[searchNearPlayer.userNumber].isTrackShy = false;
                            Initialized();
                        }
                        else
                        {
                            lookTime += Time.deltaTime;
                        }
                    }
                }
                else
                {
                    if (lookTime != 0)
                        lookTime = 0;
                }


                /* スカートを覗く */
                Vector3 headPos = kinectManager.GetJointKinectPosition(searchNearPlayer.userID, (int)KinectInterop.JointType.Head);

                if (-0.1f > headPos.y)
                {
                    if (skirtTime > waitTime)
                    {
                        if (animator)//TODO:これ関数化できるから元気な時にやる
                        {
                            if (cubemanController[searchNearPlayer.userNumber])
                            {
                                if (cubemanController[searchNearPlayer.userNumber].useAnimationName != "trick")
                                {
                                    
                                    animator.SetTrigger("onTrick_01");
                                    yield return new WaitForSeconds(1);
                                    //SaveAnimationName();
                                }
                            }
                        }
                        yield return new WaitForSeconds(4);
                        Initialized();
                    }
                    else
                    {
                        skirtTime += Time.deltaTime;
                    }
                }
                else
                {
                    if (skirtTime != 0)
                        skirtTime = 0;
                }

                /* 近づかれてあとずさり */
                if (0.8f > shoulderPos.z)
                {
                    if (nearPositionTime > waitTime)
                    {
                        // 今回の対象で一度も実行されていなかったら
                        if (!cubemanController[searchNearPlayer.userNumber].isTrackBackwards)
                        {
                            cubemanController[searchNearPlayer.userNumber].isTrackBackwards = true;
                            if (animator)//TODO:これ関数化できるから元気な時にやる
                            {
                                if (cubemanController[searchNearPlayer.userNumber])
                                {
                                    if (cubemanController[searchNearPlayer.userNumber].useAnimationName != "bodytap")
                                    {
                                        
                                        animator.SetTrigger("onBody_02");
                                        yield return new WaitForSeconds(1);
                                        SaveAnimationName();
                                    }
                                }
                            }
                            yield return new WaitForSeconds(4);
                            Initialized();
                        }
                    }
                    else
                    {
                        nearPositionTime += Time.deltaTime;
                    }
                }
                else
                {
                    if (nearPositionTime != 0)
                        nearPositionTime = 0;
                }

                /* ボディタッチ */
                if (isBodyTouch)
                {
                    if (!isHoldBodyTouch)
                    {
                        isHoldBodyTouch = true;
                        int rand = GenereteRandNumber();
                        if (rand == 0)
                        {
                            if (animator)//TODO:これ関数化できるから元気な時にやる
                            {
                                if (cubemanController[searchNearPlayer.userNumber])
                                {
                                    if (cubemanController[searchNearPlayer.userNumber].useAnimationName != "breasttap")
                                    {
                                        
                                        animator.SetTrigger("onBreast_01");
                                        yield return new WaitForSeconds(1);
                                        SaveAnimationName();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (animator)//TODO:これ関数化できるから元気な時にやる
                            {
                                if (cubemanController[searchNearPlayer.userNumber])
                                {
                                    if (cubemanController[searchNearPlayer.userNumber].useAnimationName != "breasttap")
                                    {
                                        
                                        animator.SetTrigger("onBreast_02");
                                        yield return new WaitForSeconds(1);
                                        SaveAnimationName();
                                    }
                                }
                            }
                        }
                        yield return new WaitForSeconds(4);
                        Initialized();
                    }
                }

            }
            yield return null;
        }
    }

    private void Initialized()
    {
        try
        {
            if (animator)
            {
                animator.ResetTrigger("onTrick_01");
                animator.ResetTrigger("onBreast_01");
                animator.ResetTrigger("onBreast_02");
                animator.ResetTrigger("onBody_01");
                animator.ResetTrigger("onBody_02");
                animator.ResetTrigger("onShy_01");
                animator.ResetTrigger("onSmile_01");
            }
        }
        catch
        {
            //NiceCatch　初期化前にアニメーターが削除される場合があるので
        }
        riseHandTime = 0;
        lookTime = 0;
        skirtTime = 0;
        nearPositionTime = 0;
    }

    private int GenereteRandNumber()
    {
        int randNumber = Random.Range(0, 2);
        return randNumber;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "07_Hand_Left" || other.name == "11_Hand_Right")
            isBodyTouch = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "07_Hand_Left" || other.name == "11_Hand_Right")
        {
            isHoldBodyTouch = false;
            isBodyTouch = false;
        }
    }

    private void SaveAnimationName()
    {
        if (animator)
        {
            clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
            clipName = clipInfo.clip.name.Substring(clipInfo.clip.name.IndexOf("@") + 1, clipInfo.clip.name.IndexOf("_") - clipInfo.clip.name.IndexOf("@") - 1); // charaName@animeName_hogehogeのように出力されるので@から_だけ出力するように整形
            cubemanController[searchNearPlayer.userNumber].useAnimationName = clipName;
        }
    }
}
