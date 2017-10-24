using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor;

public class ModelSetter
{
    [MenuItem("ItouScript/SetModelDate")]
    public static void SetPhotonData()
    {
        // 初期化
        var target = Selection.activeGameObject;
        target.SetActive(true);

        // モデルかどうか判定
        if (!target || !target.GetComponent<Animator>())
        {
            Debug.LogError("モデルを選択してください");
            return;
        }

        // path取得
        //AssetDatabase.GetAssetPath(target);
        // 生成
        var targetInstans = GameObject.Instantiate(target);
        
        // アニメーター取得
        Animator anime;
        anime = targetInstans.GetComponent<Animator>();
        // Rootにアタッチ
        RootSettings(targetInstans,anime);

        // Animater機能のGetBoneを使用してスクリプトを自動でアタッチ
        /*
        foreach (HumanBodyBones humanBodyBone in Enum.GetValues(typeof(HumanBodyBones)))
        {

            Debug.Log(anime.GetBoneTransform(humanBodyBone));

            if (anime.GetBoneTransform(humanBodyBone))
            {
                targetInstans = anime.GetBoneTransform(humanBodyBone).gameObject;
                BoneSettings(targetInstans);
            }

        }
        */

        // Prefab上書き
        targetInstans = targetInstans.transform.root.gameObject;
        var tyep = PrefabUtility.GetPrefabType(target); // Project上のPrefabを選択していたら実行
        if (tyep != PrefabType.None && tyep != PrefabType.PrefabInstance)
        {
            PrefabUtility.ReplacePrefab(targetInstans, target, ReplacePrefabOptions.ConnectToPrefab);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            GameObject.DestroyImmediate(targetInstans);
        }

    }

    static public void BoneSettings(GameObject targetInstance)
    {
        /*
        // ここでAddComponentをする.
        var photon = targetInstance.AddComponent<PhotonView>();
        var transformView = targetInstance.AddComponent<PhotonTransformView>();
        // Photonの設定
        photon.ObservedComponents = new List<Component>() { transformView };
        photon.synchronization = ViewSynchronization.UnreliableOnChange;
        // TransformViewの設定
        transformView.m_RotationModel.SynchronizeEnabled = true;
        transformView.m_PositionModel.SynchronizeEnabled = true;
        */
    }

    static public void RootSettings(GameObject target,Animator anime)
    {
        // レイヤーをモデルに
        var bones = target.GetComponentsInChildren<Transform>(true);
        foreach(var bone in bones)
        {
            bone.gameObject.layer = 9; // 9はモデルレイヤー
        }

        // タグをモデルに
        target.tag = "Model";

        // FacialController
        if (!target.GetComponent<MOE.FacialController>())
        {
            target.AddComponent<MOE.FacialController>();
            Debug.Log(target.name + ":FacialController完了");
        }
        else
        {
            Debug.Log(target.name + "FacialControllerはすでに設定してあります");
        }

        // FacialManager
        if (!target.GetComponent<MOE.FacialManager>())
        {
            target.AddComponent<MOE.FacialManager>();
            Debug.Log(target.name + ":FacialManager完了");
        }
        else
        {
            Debug.Log(target.name + "FacialManagerはすでに設定してあります");
        }

        // HeadLookController
        if (!target.GetComponent<HeadLookController>())
        {
            var headLookController = target.AddComponent<HeadLookController>();
            // スクリプト割り当て
            var scripts = GameObject.Find("Scripts");
            var childrenToKinectManager = scripts.GetComponentsInChildren<KinectManager>(true);
            foreach (var kinectManager in childrenToKinectManager)
            {
                headLookController.kinectManager = kinectManager;
            }
            var childrenToSeatchNearPlayer = scripts.GetComponentsInChildren<SearchNearPlayer>(true);
            foreach (var searchNearPlayer in childrenToSeatchNearPlayer)
            {
                headLookController.searchNearPlayer = searchNearPlayer;
            }
            // RootNode割り当て
            headLookController.rootNode = target.transform;
            
            Debug.Log(target.name + ":HeadLookController完了");
        }
        else
        {
            Debug.Log(target.name + "HeadLookControllerはすでに設定してあります");
        }

        // 胸揺れ
        if (!target.GetComponent<DynamicBone>())
        {
            // 右側
            var dynamicBone = target.AddComponent<DynamicBone>();
            // スクリプト割り当て
            var childrenTarget = target.GetComponentsInChildren<Transform>(true);
            foreach (var child in childrenTarget)
            {
                if(child.gameObject.name == "breast_R")
                {
                    dynamicBone.m_Root = child;
                    dynamicBone.m_Damping = 0.2f;
                    dynamicBone.m_Elasticity = 0.1f;
                    dynamicBone.m_Stiffness = 0;
                    dynamicBone.m_EndOffset = new Vector3(0.1f, -0.1f, 0.3f);
                }
            }
            // 左側
            dynamicBone = target.AddComponent<DynamicBone>();
            // スクリプト割り当て
            childrenTarget = target.GetComponentsInChildren<Transform>(true);
            foreach (var child in childrenTarget)
            {
                if (child.gameObject.name == "breast_L")
                {
                    dynamicBone.m_Root = child;
                    dynamicBone.m_Damping = 0.2f;
                    dynamicBone.m_Elasticity = 0.1f;
                    dynamicBone.m_Stiffness = 0;
                    dynamicBone.m_EndOffset = new Vector3(0.1f, -0.1f, 0.3f);
                }
            }

            Debug.Log(target.name + ":DynamicBone完了");
        }
        else
        {
            Debug.Log(target.name + "DynamicBoneはすでに設定してあります");
        }

        // 胸揺れ
        if (!target.GetComponent<RootMotion.FinalIK.BipedIK>())
        {
            target.AddComponent<RootMotion.FinalIK.BipedIK>();
            Debug.Log(target.name + ":BipedIK完了");
        }
        else
        {
            Debug.Log(target.name + "BipedIKはすでに設定してあります");
        }
        Debug.Log("FacialControllerの表情データ呼び出しボタンを押してください");
        Debug.Log("スカートにClothを追加して設定してください HeadLookControllerのResetを選択して押してください");
    }
}
