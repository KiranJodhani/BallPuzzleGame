using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Attached to all game object we want with a bright color
/// </summary>
public class ColorBrightLogic : ColorBase 
{

	public override void OnColorChanged (AAColor c)
	{
		SetColor(c.colorBright);
	}

	public override void SetColor(Color c)
	{
		if(Time.realtimeSinceStartup < 0.1f)
			SetColor(c, 0);
		else
			SetColor(c, timeAnim);
	}

	public void SetColor(Color c, float time)
	{
		if(cam != null)
			cam.DOColor(c,time);

		if(sr != null)
			sr.DOColor(c,time);

		if(im != null)
			im.DOColor(c,time);

		if(txt != null)
			txt.DOColor(c,time);
	}
}
