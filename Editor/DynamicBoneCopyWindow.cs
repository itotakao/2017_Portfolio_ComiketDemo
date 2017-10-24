using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using MOE.Spring;
using System.Linq;

namespace DynamicBoneEditor
{
	/// <summary>
	/// 指定オブジェクト間でSpring系のコピーを行う
	/// </summary>
	public class DynamicBoneCopyWindow : DataEditorWindowBase {

		private const int PRIORITY = 4;		// ツールバーのプライオリティ

		private GameObject original;		// コピーの元となるオブジェクト

		private GameObject copy;			// コピー先となるオブジェクト
        
		private AnimBool deleteFadeStatus = new AnimBool(false);	// 削除ボタン表示用


		[MenuItem("ItouScript/DynamicBoneCopy", priority = PRIORITY)]
		private static void Open()
		{
			// ProjectBrowserの横にWindowを配置する
			System.Type projectWindow =	typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ProjectBrowser");
			var window = GetWindow<DynamicBoneCopyWindow>(projectWindow);

			GUIContent content = new GUIContent("必ずHierarchy上に配置したPrefabを指定して下さい。");
			window.ShowNotification(content);
		}

		private void OnEnable() {

			deleteFadeStatus = new AnimBool(true);
			deleteFadeStatus.valueChanged.AddListener(Repaint);

		}

		// GUI生成、配置
		private void OnGUI() {

			EditorGUILayout.BeginVertical( GUI.skin.box, GUILayout.Width ( 300 ) );

			GUILayout.Space(5);
			GUILayout.Label( "DynamicBone Copy Editor", GetSectionStyle( 20 ) );
			GUILayout.Space(5);

			EditorGUILayout.BeginVertical( GUI.skin.box );
			GUILayout.Space(5);
			GUILayout.Label( "オリジナル(コピー元)", GetSectionStyle( 14 ) );
			GUILayout.Space(5);
			original = EditorGUILayout.ObjectField( "コピー元", original, typeof(GameObject), true ) as GameObject;
			EditorGUILayout.EndVertical();

			EditorGUILayout.BeginVertical( GUI.skin.box );
			GUILayout.Space(5);
			GUILayout.Label( "コピー先", GetSectionStyle( 14 ) );
			GUILayout.Space(5);
			copy = EditorGUILayout.ObjectField( "コピー先", copy, typeof(GameObject), true ) as GameObject;
			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			GUILayout.Label( "オリジナルからコピー先へ値をコピーします", GetSectionStyle( 14 ) );

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.Space();
			if ( GUILayout.Button("実行", GUILayout.Width(120), GUILayout.Height(30) ) ) {

                // オリジナルをコピー先へコピーする
                DynamicBoneCopy.Copy( original, copy );
			}
			EditorGUILayout.Space();
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
            GUILayout.Space(5);
			EditorGUILayout.BeginVertical( GUI.skin.box );
			// FadeGroupのターゲット指定
			deleteFadeStatus.target = EditorGUILayout.ToggleLeft( "削除ボタンを表示する", deleteFadeStatus.target );
			if ( EditorGUILayout.BeginFadeGroup(deleteFadeStatus.faded) ) {

				GUILayout.Label( "コピー先の揺れ物系コンポーネントを削除する", GetSectionStyle( 10 ) );
				// 削除ボタン
				if ( GUILayout.Button("削除", GUILayout.Width(60), GUILayout.Height(20) ) ) {

                    // コピー先のSpring系コンポーネントを削除する
                    DynamicBoneCopy.OnClickDeleteButton( copy );

				}
				GUILayout.Space(5);

			}
			EditorGUILayout.EndFadeGroup();
			EditorGUILayout.EndVertical();






			EditorGUILayout.EndVertical();
		}

	}
}