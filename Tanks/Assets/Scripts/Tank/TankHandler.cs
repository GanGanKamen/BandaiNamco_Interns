using UnityEngine;


/// 戦車ハンドラーコンポーネント
/// 
/// 戦車の各種処理を毎フレーム呼び出すだけのコンポーネントです。@n
/// @ingroup	System
public class TankHandler : MonoBehaviour
{
	/// 戦車アップデーター(マネージャー)
	ITankUpdater Updater { get; set; }


	/// セットアップ
	/// @param[in]	manager		戦車マネージャー
	public void Setup(TankManager manager)
	{
		Updater = manager;
	}


	void Update()
    {
		Updater.Process();
	}

	void OnDisable()
	{
		// 最後にもう1度だけ呼ぶ
		// そうすると、正しく死亡処理が走る
		Updater.Process();
	}

}


/// @file
/// 戦車ハンドラー
