using Hangout.Shared;
using UnityEngine;

namespace Hangout.Client
{

public class JSReceipt : IReceipt
{
	private GameObject mCBHandler;
	private bool mHasExited = false;
	
	public JSReceipt(GameObject cbh)
	{
		mCBHandler = cbh;
	}
	
	public GameObject GetGO()
	{
		return mCBHandler;
	}
	
	public void UpdateGO(GameObject cbh)
	{
		GameObject.Destroy(mCBHandler);
		mCBHandler = cbh;
	}

	public void Exit()
	{
		GameObject.Destroy(mCBHandler);
		mCBHandler = null;
		mHasExited = true;
	}

	public bool hasExited()
	{
		return mHasExited;
	}
}
}
