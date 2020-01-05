using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// 戦車AI 01：実装者：（名前）AI名：（イカす名前を付けてね！）
/// 
/// （こんな思惑で作りました的なアピールをしてください。
///	@n＠ｎを入れると改行できます。）
///	
/// @ingroup	TankAI
public partial class TankAI_00 : TankAIBase
{
	int target = 0;
	float time = Time.realtimeSinceStartup;

	public override void Process(TankController controller)
	{
		// 敵をランダムに選ぶ
		while (target == 0 || target == Services.PlayerNumber)
		{
			target = Random.Range(0, GameManager.Instance.NumberOfPlayers) + 1;
		}

		// 敵が存命なら、敵に近づくように移動する
		var enemy = controller.Services.GetTank(target);
		if (enemy != null && enemy.Services.Health > 0.0f)
		{
			// 一定距離内だったら前進しない
			var distance = Services.DistanceTo(enemy);
			bool isForward = distance > 20.0f;

			// 敵の居る方向を向く(一定範囲内の時は方向転換しない)
			var angle = Services.AngleTo(enemy);
			bool isLeft = false;
			bool isRight = false;
			if (Mathf.Abs(angle) > 5.0f)
			{
				isLeft = angle > 0.0f;
				isRight = !isLeft;
			}
			// 前進か左右転回するようなら移動処理を行う
			if (isForward || isLeft || isRight)
			{
				controller.Move(isForward, isLeft, isRight);
			}
			else if (Time.realtimeSinceStartup - time > 0.25f)
			{
				// 移動しない時は、一定間隔で砲弾発射！
				controller.Aim(0.0f, 5.0f);
				controller.Fire(1);
				time = Time.realtimeSinceStartup;
			}
		}
		else
		{
			// 敵が設定されてなかったり死んでたら、敵を設定しなおす
			target = 0;
		}
	}
}


