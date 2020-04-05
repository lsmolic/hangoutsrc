/*
 * Pherg built this.
 * 10/06/09
 */ 

using System;
using System.Xml;
using System.Collections.Generic;
using System.Text;

using Hangout.Shared;

using UnityEngine;
using PureMVC.Patterns;

namespace Hangout.Client
{	
	public class SoundProxy : Proxy
	{		
		private string SOUND_LOOKUP_TABLE_XML_PATH = "resources://Sounds/SoundLookupTable";
		
		private int mSoundsLeftToLoad = 0;
		
		private IDictionary<SoundName, AudioClip> mSoundDictionary = new Dictionary<SoundName, AudioClip>();

		private GameObject mMainCamera; //We play the sound at this point cuz it's the listener.
		
		public override void OnRegister()
		{
			Init();
		}

        public void Init()
        {			
			ClientAssetRepository clientAssetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			
			XmlDocument mXmlSoundDocument = XmlUtility.LoadXmlDocument(SOUND_LOOKUP_TABLE_XML_PATH);
			
			// TODO: Remove this loading hack.
			XmlNodeList soundNodes = mXmlSoundDocument.SelectNodes("Sounds/Sound");
			mSoundsLeftToLoad = soundNodes.Count;

			foreach (XmlNode soundNode in soundNodes)
			{
				string name = soundNode.SelectSingleNode("Name").InnerText;
				SoundName soundName = (SoundName)Enum.Parse(typeof(SoundName), name);
				
				string path = soundNode.SelectSingleNode("Path").InnerText;
                //Console.WriteLine("Loading sound: " + name + ", " + path);
                clientAssetRepo.LoadAssetFromPath<SoundAsset>(path, delegate(SoundAsset asset)
				{
					RetrieveSoundAssetFromRepoOnLoad(soundName, asset);
				});
			}
			
			mMainCamera = GameFacade.Instance.RetrieveMediator<CameraManagerMediator>().MainCamera;
		}

		private void RetrieveSoundAssetFromRepoOnLoad(SoundName soundName, Asset asset)
		{
			//Console.WriteLine("Loaded sound: " + soundName.ToString());
			SoundAsset soundAsset = (SoundAsset)asset;
			soundAsset.AudioClip.name = soundName.ToString();
			if (soundAsset == null)
			{
				throw new Exception("asset returned from repo could not be cast as SoundAsset.");
			}
			AddSoundFromLoading(soundName, soundAsset.AudioClip);
		}
		
		private void RetrieveSoundAssetFromRepo(SoundName soundName, Asset asset)
		{
            //Console.WriteLine("Loaded sound: " + soundName.ToString());
			SoundAsset soundAsset = (SoundAsset) asset;
			soundAsset.AudioClip.name = soundName.ToString();
			if (soundAsset == null)
			{
				throw new Exception("asset returned from repo could not be cast as SoundAsset.");
			}
			AddSound(soundName, soundAsset.AudioClip);
		}
		
		//Default to play the sound @ the camera.. 
		public void PlaySound(SoundName soundEnum)
		{
			PlaySoundAtPosition(soundEnum, mMainCamera.transform.position);
		}
	
		public void PlaySoundAtPosition(SoundName soundEnum, Vector3 position)
		{
			AudioClip clipToPlay = new AudioClip();
			if (mSoundDictionary.TryGetValue(soundEnum, out clipToPlay))
			{
				AudioSource.PlayClipAtPoint(clipToPlay, position);
			}
			else
			{
				throw new Exception("Trying to play sound: " + soundEnum + " when it is not added to the SoundProxy.");
			}
		}

		private void AddSoundFromLoading(SoundName name, AudioClip audioClip)
		{
			if (mSoundDictionary.ContainsKey(name))
			{
				throw new Exception("mSoundDictionary already contains SoundName: " + name + ".");
			}
			mSoundDictionary.Add(name, audioClip);
			--mSoundsLeftToLoad;
			if (mSoundsLeftToLoad == 0)
			{
				GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler.StartCoroutine(WaitOneFrameThenFireOffLoadedNotification());
			}
		}
		
		public void AddSound(SoundName name, AudioClip audioClip)
		{
			if (mSoundDictionary.ContainsKey(name))
			{
				throw new Exception("mSoundDictionary already contains SoundName: " + name + ".");
			}
			mSoundDictionary.Add(name, audioClip);
		}
		
		/// <summary>
		/// one frame is needed so the GSM can reigster itself to hear this nofiication if we had to reconnect
		/// to the server.
		/// </summary>
		/// <returns></returns>
		private IEnumerator<IYieldInstruction> WaitOneFrameThenFireOffLoadedNotification()
		{
			yield return new YieldUntilNextFrame();
			GameFacade.Instance.SendNotification(GameFacade.SOUND_PROXY_LOADED);
		}
	}
}
