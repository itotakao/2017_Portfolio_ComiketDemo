using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MOE.Spring;
using NwgLibrary;
using System.Linq;
using MOE.PhysicsPlaneSkirt;


namespace DynamicBoneEditor
{

	public class DynamicBoneCopy : MonoBehaviour {

		/// <summary>
		/// 削除ボタンのコールバックメソッド
		/// </summary>
		/// <param name="obj">Object.</param>
		public static void OnClickDeleteButton( GameObject obj ) {

			RemoveSpringComponents( obj );

		}

        /// <summary>
        /// Spring系コンポーネントを削除する
        /// </summary>
        private static void RemoveSpringComponents(GameObject obj) {

            // 指定オブジェクトのルート取得
            var root = obj.GetRoot();

            // root下の全オブジェクトを取得
            GameObject[] objs = GameObjectExtensions.GetChildren(root);

            // DynamicBoneを削除
            objs.Select(a => a.GetComponent<DynamicBone>()).Where(a => a != null).ToList().ForEach(Undo.DestroyObjectImmediate);
            // DynamicBoneColliderを削除

            // RootObjの子をすべて探索し格納
            List<GameObject> rootObjList = GetAllChildren.GetAll(root);
            // コピー先を探索しDynamicBoneColliderがあったら参照
            foreach (GameObject child in rootObjList)
            {
                if (child.GetComponent<DynamicBoneCollider>())
                {
                    Undo.DestroyObjectImmediate(child);
                }
            } 
            
		}

		/// <summary>
		/// オリジナルからコピー先へコピーする
		/// </summary>
		public static void Copy( GameObject original, GameObject copy) {
			
			// 念のためオブジェクトのrootを取得
			original = GameObjectExtensions.GetRoot(original);
			copy = GameObjectExtensions.GetRoot(copy);

			// originalのSpring系のコンポーネントがアタッチされている全オブジェクトを取得する

			// originalの全オブジェクトを取得
			var originalChildrens = GameObjectExtensions.GetChildren(original);

			// copyの全オブジェクトを取得
			var copyChildrens = GameObjectExtensions.GetChildren(copy);
            
			//************************
			// DynamicBoneColliderのコピー
			//************************

			// originalからDynamicBoneColliderがアタッチされているオブジェクトを取得
			var colliders = originalChildrens.Where( a => a.GetComponent<DynamicBoneCollider>() ).ToList();

			// colliderListの親を取得
			var colliderParents = colliders.Select( a => a.GetParent() ).ToList();
            
			// Boneの配列に受け渡す用
			List<GameObject> colliderObjs = new List<GameObject>();

			foreach( var child in copyChildrens ) {

				foreach( var parent in colliderParents ) {

					foreach( var collider in colliders ) {
						if ( child.name.Equals(parent.name) && parent.name.Equals(collider.GetParent().name) ) {

                            // 同じ階層に複数Colliderがあると重複分複製してしまうので判定
                            bool isDuplication = false;

                            foreach (var obj in colliderObjs)
                            {
                                if (collider.name.Contains(obj.name))
                                {
                                    isDuplication = true;
                                    break;
                                }
                            }

                            if (!isDuplication)
                            {
                                colliderObjs.Add(CreateCopyObject(collider, child));
                            }
							
						}
					}
				}
			}

			// Colliderのオブジェクトを追加したので更新
			originalChildrens = GameObjectExtensions.GetChildren(original);
			copyChildrens = GameObjectExtensions.GetChildren(copy);

			//************************
			// DynamicBoneのコピー
			//************************

			// originalからDynamicBoneがアタッチされているオブジェクトを取得
			var bones = originalChildrens.Where( a => a.GetComponent<DynamicBone>() ).ToList();

			List<DynamicBone> boneObjs = new List<DynamicBone>();

			foreach( var child in copyChildrens ) {

				foreach ( var bone in bones ) {

					if ( child.name.Equals( bone.name ) ) {

						DynamicBone boneCompo = null;


						boneCompo = AddBoneComponent( bone, copyChildrens, child );

                        if (boneCompo != null)
                        {
                            boneObjs.Add(boneCompo);

                        }}
				}
			}



			// Colliderのオブジェクトを追加したので更新
			originalChildrens = GameObjectExtensions.GetChildren(original);
			copyChildrens = GameObjectExtensions.GetChildren(copy);

			//************************
			// SpringManagerのコピー
			//************************

			// SpringMangerをもつオブジェクトを抜き出す
			var managerlist = originalChildrens.Where( a => a.GetComponent<SpringManager>() ).ToList();

			managerlist.ForEach(print);

			foreach( var item in managerlist ) {

				var obj = GameObjectExtensions.SearchGameObject( copy, item.name );
                
				if (obj != null) {
					AddMangerComponent(obj, copyChildrens, item);
				}

			}

			print( original.name + "から" + copy.name + "へ揺れ物系をコピーしました。");

		}


		// PhysicsPlaneSkirtをコピー先に追加する
		private static void AddPhysicsPlaneComponent( GameObject[] copys, GameObject[] origins ) {

			// オリジナルのPhysicsObjectPlaneSkirtコンポーネントを取得
			var orgPhysicsComponents = origins.Select( a => a.GetComponent<PhysicsObjectPlaneSkirt>() ).Where( a => a != null).ToList();

			// コピー先のPhysicsObjectPlaneSkirtが必要な名前リスト
			var copyPhysicsNames = copys.Select( a => a.name ).Intersect( orgPhysicsComponents.Select( a => a.name )).ToList();

			foreach( var obj in copys ) {

				foreach ( var name in copyPhysicsNames ) {

					if ( obj.name.Equals(name) ) {

						var compo = obj.AddComponent<PhysicsObjectPlaneSkirt>();

						foreach( var org in orgPhysicsComponents ) {

							if ( org.name.Equals(obj.name) ) {

								compo.m_CheckKneeL = org.m_CheckKneeL;
								compo.m_CheckKneeR = org.m_CheckKneeR;
								compo.m_DrawGizmos = org.m_DrawGizmos;
								compo.m_Inside = org.m_Inside;
								compo.m_KneeRadius = org.m_KneeRadius;
								compo.m_Length = org.m_Length;
								compo.m_MoveAngle = org.m_MoveAngle;
								compo.m_OffsetY = org.m_OffsetY;
								compo.m_Outside = org.m_Outside;

							}

						}

					}

				}

			}

			// オリジナルでSkirtControllerがついているオブジェクト名と同名のオブジェクトをコピー先で取得する
			var orgSkirt = origins.Where( a => a.GetComponent<SkirtController>() ).ToList();
			var copySkirt = copys.Where( a => a.name.Equals(orgSkirt[0].name) ).ToList();

			var orgController = orgSkirt[0].GetComponent<SkirtController>();
			var copyController = copySkirt[0].AddComponent<SkirtController>();

			copyController._isAxisX = orgController._isAxisX;
			copyController._LegAngleCoefficient = orgController._LegAngleCoefficient;
			copyController.m_KneeRadius = orgController.m_KneeRadius;
			copyController.m_DrawGizmos = orgController.m_DrawGizmos;

			// コピーのRoot
			var copyRoot = copys[0].GetRoot();

			copyController.m_ThighBoneL = GameObjectExtensions.SearchGameObject( copyRoot, orgController.m_ThighBoneL.name);
			copyController.m_ThighBoneR = GameObjectExtensions.SearchGameObject( copyRoot, orgController.m_ThighBoneR.name);
			copyController.m_KneeBoneL = GameObjectExtensions.SearchGameObject( copyRoot, orgController.m_KneeBoneL.name);
			copyController.m_KneeBoneR = GameObjectExtensions.SearchGameObject( copyRoot, orgController.m_KneeBoneR.name);

		}


		// オリジナルからコピー先にSpringMangerコンポーネントをコピーする
		private static void AddMangerComponent( GameObject copyObj, GameObject[] copyObjs, GameObject originalObj ) {

			// DynamicBoneでは不必要なので撤去

		}



		// objのコンポーネント情報をtargetに追加する
		private static DynamicBone AddBoneComponent( GameObject obj, GameObject[] objs, GameObject target ) {

			// コピー先に子がいない場合は抜ける
			if ( !target.HasChild() ) {
				return null;
			}

			// コピー元
			var fromBone = obj.GetComponent<DynamicBone>();

			// コピー先
			var toBone = target.AddComponent<DynamicBone>();

			// ルートObject
			var rootObj = target.GetRoot();
            
            /*** DynamicBone Parameters ***/
            
            // Rootを設定
            toBone.m_Root = target.gameObject.transform;
            // UpdateRateを設定
            toBone.m_UpdateRate = fromBone.m_UpdateRate;
            // UpdateModeを設定
            toBone.m_UpdateMode = fromBone.m_UpdateMode;
            // Dampingを設定
            toBone.m_Damping = fromBone.m_Damping;
            toBone.m_DampingDistrib = fromBone.m_DampingDistrib;
            // Elasticityを設定
            toBone.m_Elasticity = fromBone.m_Elasticity;
            toBone.m_ElasticityDistrib = fromBone.m_ElasticityDistrib;
            // Stiffnessを設定
            toBone.m_Stiffness = fromBone.m_Stiffness;
            toBone.m_StiffnessDistrib = fromBone.m_StiffnessDistrib;
            // RadiusDistribを設定
            toBone.m_Radius = fromBone.m_Radius;
            toBone.m_RadiusDistrib = fromBone.m_RadiusDistrib;
            // EndLenghを設定
            toBone.m_EndLength = fromBone.m_EndLength;
            // EndOffset,Gravity,Forceを設定
            toBone.m_EndOffset = fromBone.m_EndOffset;
            toBone.m_Gravity = fromBone.m_Gravity;
            toBone.m_Force = fromBone.m_Force;
            // Collidersを設定
            toBone.m_Colliders = new List<DynamicBoneColliderBase>();

            // RootObjの子をすべて探索し格納
            List<GameObject> rootObjList = GetAllChildren.GetAll(rootObj);
            // コピー先を探索しDynamicBoneColliderがあったら参照
            foreach (var collider in fromBone.m_Colliders)
            {
                //var dynamicBoneCollider = rootObj.transform.Find(collider.gameObject.name).GetComponent<DynamicBoneCollider>();
                foreach (GameObject child in rootObjList)
                {
                    if (child.name == collider.gameObject.name)
                    {
                        toBone.m_Colliders.Add(child.GetComponent<DynamicBoneCollider>());
                    }
                }

            }
            // Exclusionsを設定
            toBone.m_Exclusions = new List<Transform>();
            foreach (var exclusion in fromBone.m_Exclusions)
            {
                toBone.m_Exclusions.Add(exclusion);
            }
            // FreezeAxisを設定
            toBone.m_FreezeAxis = fromBone.m_FreezeAxis;
            // DistantDisableを設定
            toBone.m_DistantDisable = fromBone.m_DistantDisable;
            // ReferenceObjectを設定
            toBone.m_ReferenceObject = fromBone.m_ReferenceObject;
            // DistanceToObjectを設定
            toBone.m_DistanceToObject = fromBone.m_DistanceToObject;
            
            return toBone;
		}

       

        /// <summary>
        /// 第1引数のオブジェクトをコピーし、第2引数の子供にし返す
        /// </summary>
        /// <returns>The copy object.</returns>
        /// <param name="obj">Object.</param>
        /// <param name="child">Child.</param>
        private static GameObject CreateCopyObject( GameObject obj, GameObject child ) {

			// Copy先に Colliderを配置するオブジェクト名があるので追加する
			var clone = Instantiate( obj, child.transform );
			clone.name = obj.name;
			clone.transform.localPosition = obj.transform.localPosition;
			clone.transform.localRotation = obj.transform.localRotation;
			clone.transform.localScale = obj.transform.localScale;

			return clone;
		}




	}



	/// <summary>
	/// スプリングボーンがついているゲームオブジェクト名の比較クラス.
	/// </summary>
	public class NameComparer : IEqualityComparer<GameObject>
	{
		#region IEqualityComparer implementation
		public bool Equals (GameObject x, GameObject y)
		{
			if( x == null || y == null )
			{
				return false;
			}
			return x.name == y.name;
		}
		public int GetHashCode (GameObject obj)
		{
			return obj.name.GetHashCode();
		}
		#endregion
	}
}