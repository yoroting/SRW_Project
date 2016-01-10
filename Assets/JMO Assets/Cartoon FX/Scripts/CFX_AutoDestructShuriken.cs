using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ParticleSystem))]
public class CFX_AutoDestructShuriken : MonoBehaviour
{
    static public int nFXCount = 0;
    public bool OnlyDeactivate;
	
	void OnEnable()
	{
        nFXCount++;
        StartCoroutine("CheckIfAlive");
	}

    void OnDisable()
    {
        if (--nFXCount < 0)
        {
            nFXCount = 0;       // over del by force release lock
        }
    }

    IEnumerator CheckIfAlive ()
	{
		while(true)
		{
			yield return new WaitForSeconds(0.5f);
			if(!GetComponent<ParticleSystem>().IsAlive(true))
			{
               
                if (OnlyDeactivate)
                {
#if UNITY_3_5
						this.gameObject.SetActiveRecursively(false);
#else
                    this.gameObject.SetActive(false);
#endif
                }
                else

                    GameObject.Destroy(this.gameObject);
				break;
			}
		}
	}
}
