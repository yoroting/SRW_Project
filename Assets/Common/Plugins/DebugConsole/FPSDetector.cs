using UnityEngine;
using System.Collections;

public sealed class FPSDetector
{
	private float timeLeft;
	
	private int fps;
	
	private int frameCount;

	public int FPS
	{
		get {return fps;}
	}

	public int TargetFPS
	{
		get {return Application.targetFrameRate;}
	}

	public void Clean()
	{
		timeLeft = 0;
		fps = 0;
		frameCount = 0;
	}

	public void EnterFrame()
	{
		this.frameCount ++;
		
		timeLeft += RealTime.deltaTime;
		
		if (timeLeft >= 1f)
		{
			this.fps = frameCount;
			
			frameCount = 0;
			timeLeft = 0;
		}
	}
}
