using UnityEngine;
using UnityEngine.UI;


// @brief		色の線形補間アニメーション
// 
// UI.Graphic の Color を線形補間でループアニメーションさせるコンポーネントです。
[RequireComponent(typeof(Graphic))]
public class ColorLerp : MonoBehaviour
{
	[SerializeField, Header("開始色")]
	Color _From = Color.black;
	[SerializeField, Header("終了色")]
	Color _To = Color.white;

	[SerializeField, Header("時間(秒)")]
	float _Time = 2.0f;
	[SerializeField, Header("往復か片道か")]
	bool _Reversal = false;

	Graphic Graphic { get; set; }
	float ElapsedTime { get; set; }


	void Awake()
	{
		Graphic = GetComponent<Graphic>();
		Debug.Assert(Graphic != null);
		enabled = Graphic != null;
	}

	void Update()
	{
		var max = _Reversal ? _Time * 0.5f : _Time;
		var time = _Reversal && ElapsedTime > max ? _Time - ElapsedTime : ElapsedTime;
		Graphic.color = Color.Lerp(_From, _To, time / max);

		ElapsedTime += Time.deltaTime;
		if (ElapsedTime > _Time) ElapsedTime -= _Time;
	}

}


/// @file
/// @brief	色の線形補間アニメーション
