using UnityEngine;
using UnityEngine.UI;


public class UIPlayerStatus : MonoBehaviour
{
	[SerializeField]
	Image _Color = null;
	[SerializeField]
	Text _Name = null;
	[SerializeField]
	Slider _Health = null;
	[SerializeField]
	Slider _Energy = null;
	[SerializeField]
	Text _Ranking = null;

	/// プレイヤーカラー
	public Color Color { set => _Color.color = value; }

	/// プレイヤー名
	public string Name { get => _Name.text;  set => _Name.text = value; }
	/// 耐久値
	public float Health { set => _Health.value = value; }
	/// エネルギー
	public float Energy { set => _Energy.value = value; }
	/// 順位
	public string Ranking { set => _Ranking.text = value; }

	/// 生存状態にする
	public void SetAlive() => SetStat(true);
	/// 死亡状態にする
	public void SetDead() => SetStat(false);


	void SetStat(bool isActive)
	{
		_Health.gameObject.SetActive(isActive);
		_Energy.gameObject.SetActive(isActive);

		_Ranking.gameObject.SetActive(!isActive);
	}

}
