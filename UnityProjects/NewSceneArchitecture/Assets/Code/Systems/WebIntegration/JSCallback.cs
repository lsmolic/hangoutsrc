using System;
using UnityEngine;

public class JSCallback : MonoBehaviour
{
	private Action<string> mHandler;
	
	public void Register(Action<string> handler)
	{
		mHandler = handler;
	}
	
	public void Callback(string arg)
	{
        try
        {
            if (mHandler != null)
            {
                mHandler(arg);
            }
            GameObject.Destroy(this.gameObject);
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("Caught unhandled exception in a callback from Javascript: " + ex);
        }

	}
}
