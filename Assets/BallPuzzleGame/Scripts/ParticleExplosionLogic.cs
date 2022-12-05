using UnityEngine;
using System.Collections;

/// <summary>
/// Attached to the particle effect who appears when a dot is destroyed. This class lauch the emitter and destroy the particle 5 seconds after
/// </summary>
public class ParticleExplosionLogic : MonoBehaviour
{

	//public ParticleAnimator particleAnimator;

	public void Do()
	{


		//Color[] modifiedColors = particleAnimator.colorAnimation;
		
		
		
		//int count = particleAnimator.colorAnimation.Length;
		
		//for (int i = 0; i < count; i++) {
			
		//	float rand = UnityEngine.Random.Range(0.2f,0.5f);
			
		//	Color cc = Color.black;
			
		//	cc.a = rand;
			
		//	modifiedColors[i] = cc;
			
		//}
		
		
		//particleAnimator.colorAnimation = modifiedColors;


		//GetComponent<ParticleEmitter>().Emit(Random.Range(100,200));

		Destroy(gameObject,5);

	}
}
