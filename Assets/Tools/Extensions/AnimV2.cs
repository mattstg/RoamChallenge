using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimV2
{
    public AnimationCurve animCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 1), new Keyframe(1, 1) });
    public Vector2 animBounds = new Vector2(1,1);

    public AnimV2() { }
    public AnimV2(Vector2 animBounds) { this.animBounds = animBounds; animCurve = new AnimationCurve(new Keyframe[] { new Keyframe(0, 0), new Keyframe(1, 1) }); }


    public float GetValue(float x)
    {
		if (animCurve == null || animBounds == null) {
			Debug.Log ("Error in Anim V2: Null input.");
			return 1;
		} else {
			try {
				return Mathf.Lerp (animBounds.x, animBounds.y, animCurve.Evaluate (x));
			} catch {
				Debug.Log ("Error in Anim V2: null result?");
				return 1;
			}
		}
    }

    public float GetValueFromYMappedToXInstead(float y)
    {
        if (animCurve == null || animBounds == null)
        {
            Debug.Log("Error in Anim V2: Null input.");
            return 1;
        }
        else
        {
            try
            {
                float perc = Mathf.Clamp01((y - animBounds.x) / (animBounds.y - animBounds.x));
                return animCurve.Evaluate(perc);
                //return Mathf.Lerp(animBounds.x, animBounds.y, animCurve.Evaluate(x));
            }
            catch
            {
                Debug.Log("Error in Anim V2: null result?");
                return 1;
            }
        }
    }

   
}
