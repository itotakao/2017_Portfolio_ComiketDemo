using UnityEngine;
using MOE;

/// <summary>
/// SearchNearPlayer.csで取得したオブジェクトの位置をキャラクターの注目点に反映
/// </summary>
public class AttachHeadPoint : MonoBehaviour {

    [SerializeField]
    private SearchNearPlayer searchNearPlayer;
    [SerializeField]
    private FacialController facialController;
    [SerializeField]
    private HeadLookController headLookController;
    [SerializeField]
    private Animator anime;
    private void Reset()
    {
        if (!searchNearPlayer)
            searchNearPlayer = gameObject.GetComponent<SearchNearPlayer>();
        if (!facialController)
            facialController = GameObject.FindWithTag("Model").GetComponent<FacialController>();
        if (!headLookController)
            headLookController = GameObject.FindWithTag("Model").GetComponent<HeadLookController>();
        if (!anime)
            anime = GameObject.FindWithTag("Model").GetComponent<Animator>();
    }

    /// <summary>
    /// SerchNearPlayerで参照しているオブジェを同期　あるなら代入　ないならnull
    /// </summary>
	void Update () {

        // アニメーションを読み込み
        if (anime)
        {
            AnimatorClipInfo clipInfo = anime.GetCurrentAnimatorClipInfo(0)[0];
            string clipName = clipInfo.clip.name.Substring(clipInfo.clip.name.IndexOf("@") + 1, clipInfo.clip.name.IndexOf("_") - clipInfo.clip.name.IndexOf("@") - 1); // charaName@animeName_hogehogeのように出力されるので@から_だけ出力するように整形

            // 同期先を設定
            if (clipName != "walk")
            {
                if (searchNearPlayer._NearPlayerHead)
                {
                    Transform lookTransform = searchNearPlayer._NearPlayerHead.transform;
                    lookTransform.transform.position = new Vector3(searchNearPlayer._NearPlayerHead.transform.position.x, searchNearPlayer._NearPlayerHead.transform.position.y, 1);
                    facialController.transformForLookAt = lookTransform;
                }   
            }
            else
            {
                if (facialController)
                    if (facialController.transformForLookAt)
                    {
                        facialController.transformForLookAt = null;
                    }
            }

            if (searchNearPlayer._NearPlayerHead)
            {
                headLookController.lookTarget = searchNearPlayer._NearPlayerHead;
            }
            else
            {
                if(headLookController)
                if (headLookController.lookTarget)
                {
                    headLookController.lookTarget = null;
                }
            }
        }

    }
}
