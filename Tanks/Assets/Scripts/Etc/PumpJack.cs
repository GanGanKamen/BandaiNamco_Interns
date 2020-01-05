using UnityEngine;

public class PumpJack : MonoBehaviour
{
	[SerializeField]
	Animator animator;


    void Start()
    {
		animator?.Play("Take 001");
    }

}
