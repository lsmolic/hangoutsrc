/**  --------------------------------------------------------  *
 *   InitializeGameState.cs  
 *
 *   Author: Mortoc, Hangout Industries
 *   Date: 12/03/2009
 *	 
 *   --------------------------------------------------------  *
 */

using System;
using System.Xml;
using System.Collections.Generic;

using Hangout.Client;
using Hangout.Client.Gui;

using Hangout.Shared;
using Hangout.Shared.FashionGame;

namespace Hangout.Client.FashionGame
{
	public class InitializeGameState : State
	{
		private readonly Hangout.Shared.Action mOnInitialInfoLoaded;
		private readonly IScheduler mScheduler;

		/// <summary>
		/// Sets up the Fashion Game systems that aren't tied to a specific level; FashionGameInput, ClothingMediator, FashionNpcMediator, PlayerProgression and FashionGameGui.
		/// </summary>
		/// <param name="onInitialInfoLoaded">Called after all the initial systems are ready</param>
		public InitializeGameState(Hangout.Shared.Action onInitialInfoLoaded)
		{
			if( onInitialInfoLoaded == null )
			{
				throw new ArgumentNullException("onInitialInfoLoaded");
			}
			mOnInitialInfoLoaded = onInitialInfoLoaded;
			mScheduler = GameFacade.Instance.RetrieveMediator<SchedulerMediator>().Scheduler;
		}

		public override void EnterState()
		{
			FashionGameCommands.GetLoadingInfo(delegate(Message loadingInfoMessage)
			{
				mScheduler.StartCoroutine(ProcessLoadingInfo(loadingInfoMessage));
			});
		}

		private IEnumerator<IYieldInstruction> ProcessLoadingInfo(Message loadingInfoMessage)
		{
			if (loadingInfoMessage.Data.Count < 7)
			{
				throw new Exception("Loading Info Message does not contain all the expected data");
			}

			FashionGameInput input = new FashionGameInput();
			GameFacade.Instance.RegisterMediator(input);

			FashionGameGui gui = new FashionGameGui();
			gui.SetEnergy
			(
				(float)loadingInfoMessage.Data[2],
				(float)loadingInfoMessage.Data[3],
				DateTime.Parse((string)loadingInfoMessage.Data[4])
			);
			gui.SetWave(1, 1);
			gui.SetLevel("Loading...");
			gui.EnableNextWave(false);
			GameFacade.Instance.RegisterMediator(gui);

			PlayerProgression progression = new PlayerProgression((uint)loadingInfoMessage.Data[5], (uint)loadingInfoMessage.Data[6]);
			progression.UpdateProgressGUI(false);
			GameFacade.Instance.RegisterMediator(progression);

			XmlDocument modelClothesXml = new XmlDocument();
			modelClothesXml.LoadXml((string)loadingInfoMessage.Data[0]);

			XmlDocument stationWorkerClothesXml = new XmlDocument();
			stationWorkerClothesXml.LoadXml((string)loadingInfoMessage.Data[1]);

			FashionNpcMediator npcFactory = new FashionNpcMediator();
			GameFacade.Instance.RegisterMediator(npcFactory);

			IEnumerable<Asset> modelClothes = null;
			ClientAssetRepository clientAssetRepo = GameFacade.Instance.RetrieveProxy<ClientAssetRepository>();
			clientAssetRepo.GetAssets<Asset>
			(
				ClientAssetInfo.Parse(modelClothesXml),
				delegate(IEnumerable<Asset> downloadedAssets)
				{
					modelClothes = downloadedAssets;
				}
			);

			IEnumerable<Asset> stationWorkerClothes = null;
			clientAssetRepo.GetAssets<Asset>
			(
                ClientAssetInfo.Parse(stationWorkerClothesXml),
				delegate(IEnumerable<Asset> downloadedAssets)
				{
					stationWorkerClothes = downloadedAssets;
				}
			);

			yield return new YieldWhile(delegate()
			{
				return modelClothes == null || stationWorkerClothes == null;
			});

			npcFactory.SetModelDefaultClothes(modelClothes);
			npcFactory.SetStationWorkerDefaultClothes(stationWorkerClothes);

			GameFacade.Instance.RegisterMediator(new ClothingMediator());
			input.StartListeningForInput();

			mOnInitialInfoLoaded();
		}

		public override void ExitState() {}
	}
}
