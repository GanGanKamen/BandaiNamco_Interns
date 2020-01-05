using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


/// 戦車AI 01：実装者：（名前）AI名：（イカす名前を付けてね！）
/// 
/// （こんな思惑で作りました的なアピールをしてください。
///	@n＠ｎを入れると改行できます。）
///	
/// @ingroup	TankAI
public partial class TankAI_08 : TankAIBase
{
    public enum Status
    {
        Start,
        Attack,
        Defult,
        Next,
        Break
    }
    public Status status = Status.Start;
    int target = 0;
    float time = Time.realtimeSinceStartup;
    public TankController targetTank;

    public override string MyName { get; } = "MEVIUS";

    public override Color MyColor { get; } = new Color(236f, 0, 140f);

    private TankController tankCtrl;

    private float startCount0 = 0;

    public override void Process(TankController controller)
    {
        tankCtrl = controller;
        Debug.Log(status);
        // 敵をランダムに選ぶ
        /*
		while (target == 0 || target == Services.PlayerNumber)
		{
			target = Random.Range(0, GameManager.Instance.NumberOfPlayers) + 1;
		}
        
        while (target == 0 || target == Services.PlayerNumber)
        {
            //target = Random.Range(0, GameManager.Instance.NumberOfPlayers) + 1;

            
            for (int i = 0; i < GameManager.Instance.NumberOfPlayers + 1; i++)
            {
                Debug.Log(Services.DistanceTo(controller.Services.GetTank(i)));
            }
        }
        */
        switch (status)
        {
            case Status.Start:
                StartRun();
                break;
            case Status.Attack:
                Attack();
                break;
            case Status.Defult:
                DefuAttack();
                break;
            case Status.Next:
                if(targetTank == null || targetTank.Services.Health <= 0)
                {
                    targetTank = TargetEnemy();
                }
                else
                {
                    NextAttack();
                }
                break;
            case Status.Break:
                FullRun();
                break;
        }
        //Debug.Log(startCount0);
    }

    private void Attack()// 敵が存命なら、敵に近づくように移動する
    {
        var enemy = tankCtrl.Services.GetNearestTank();
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
                tankCtrl.Move(isForward, isLeft, isRight);
            }
            else if (Time.realtimeSinceStartup - time > 0.2f)
            {
                // 移動しない時は、一定間隔で砲弾発射！
                tankCtrl.Aim(Random.Range(-4f,4f), 5.0f);
                if(tankCtrl.Services.Energy >= 6000)
                {
                    tankCtrl.Fire(10);
                }
                else if(tankCtrl.Services.Energy < 6000&&tankCtrl.Services.Energy >= 2000)
                {
                    tankCtrl.Fire(5);
                }
                else
                {
                    tankCtrl.Fire(1);
                }
                time = Time.realtimeSinceStartup;
            }
        }
        else
        {
           
        }
    }

    private void StartRun()
    {
        if (startCount0 < 20)
        {
            var allTanks = new List<TankController>();
            for (int i = 0; i < GameManager.Instance.NumberOfPlayers; i++)
            {
                var target = tankCtrl.Services.GetTank(i + 1);
                if (target.Services.Health != 0 && target != tankCtrl)
                {
                    allTanks.Add(target);
                }
            }
            allTanks.Sort((a, b) => (int)tankCtrl.Services.DistanceTo(a) - (int)tankCtrl.Services.DistanceTo(b));
            var alertEnemy = allTanks[0];
            if (Mathf.Abs(tankCtrl.Services.AngleTo(alertEnemy)) <= 170f)
            {
                tankCtrl.Move(true, true, false);
            }
            else
            {
                tankCtrl.Move(true, false, false);
            }
            startCount0 += Time.deltaTime;
        }
        else
        {
            startCount0 = 0;
            status = Status.Next;
        }
    }

    private void FullRun()
    {
        if(tankCtrl.Services.Energy <= 5000)
        {
            var allTanks = new List<TankController>();
            for (int i = 0; i < GameManager.Instance.NumberOfPlayers; i++)
            {
                var target = tankCtrl.Services.GetTank(i + 1);
                if (target.Services.Health != 0 && target != tankCtrl)
                {
                    allTanks.Add(target);
                }
            }
            allTanks.Sort((a, b) => (int)tankCtrl.Services.DistanceTo(a) - (int)tankCtrl.Services.DistanceTo(b));
            var alertEnemy = allTanks[0];
            var disFast = tankCtrl.Services.DistanceTo(alertEnemy);
            if (disFast <= 10f)
            {
                if (Mathf.Abs(tankCtrl.Services.AngleTo(alertEnemy)) <= 170f)
                {
                    tankCtrl.Move(true, true, false);
                }
                else
                {
                    tankCtrl.Move(true, false, false);
                }
            }
            else
            {
                tankCtrl.Move(false, false, false);
            }
        }

        else
        {
            status = Status.Next;
        }
    }


    private void NextAttack()
    {
        if (targetTank != null && targetTank.Services.Health > 0.0f)
        {
            // 一定距離内だったら前進しない
            var distance = Services.DistanceTo(targetTank);
            bool isForward = distance > 20.0f;

            // 敵の居る方向を向く(一定範囲内の時は方向転換しない)
            var angle = Services.AngleTo(targetTank);
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
                tankCtrl.Move(isForward, isLeft, isRight);
            }
            else if (Time.realtimeSinceStartup - time > 0.2f)
            {
                // 移動しない時は、一定間隔で砲弾発射！
                tankCtrl.Aim(Random.Range(-4f, 4f), 5.0f);
                if (tankCtrl.Services.Energy >= 6000)
                {
                    tankCtrl.Fire(10);
                }
                else if (tankCtrl.Services.Energy < 6000 && tankCtrl.Services.Energy >= 2000)
                {
                    tankCtrl.Fire(5);
                }
                else
                {
                    tankCtrl.Fire(1);
                }
                time = Time.realtimeSinceStartup;
            }
        }
        if(tankCtrl.Services.Energy < 1000)
        {
            status = Status.Break;
        }
    }

    private TankController TargetEnemy()
    {
        var allTanks = new List<TankController>();
        for (int i = 0; i < GameManager.Instance.NumberOfPlayers; i++)
        {
            var target = tankCtrl.Services.GetTank(i + 1);
            if (target.Services.Health != 0 && target!=tankCtrl)
            {
                allTanks.Add(target);
            }
        }
        allTanks.Sort((a, b) => (int)tankCtrl.Services.DistanceTo(a) - (int)tankCtrl.Services.DistanceTo(b));
        return allTanks[0];
    } 

    private void DefuAttack()
    {
        while (target == 0 || target == Services.PlayerNumber)
        {
            target = Random.Range(0, GameManager.Instance.NumberOfPlayers) + 1;
        }

        // 敵が存命なら、敵に近づくように移動する
        var enemy = tankCtrl.Services.GetTank(target);
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
                tankCtrl.Move(isForward, isLeft, isRight);
            }
            else if (Time.realtimeSinceStartup - time > 0.2f)
            {
                // 移動しない時は、一定間隔で砲弾発射！
                tankCtrl.Aim(Random.Range(-4f, 4f), 5.0f);
                if (tankCtrl.Services.Energy >= 6000)
                {
                    tankCtrl.Fire(10);
                }
                else if (tankCtrl.Services.Energy < 6000 && tankCtrl.Services.Energy >= 2000)
                {
                    tankCtrl.Fire(5);
                }
                else
                {
                    tankCtrl.Fire(1);
                }
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


