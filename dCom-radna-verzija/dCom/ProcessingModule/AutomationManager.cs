using Common;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProcessingModule
{
    /// <summary>
    /// Class containing logic for automated work.
    /// </summary>
    public class AutomationManager : IAutomationManager, IDisposable
	{
		private Thread automationWorkerDigital; //Thread za citanje digitalnih ulaza/izlaza
		private Thread automationWorkerAnalog; //Thread za citanje analognih ulaza/izlaza
        private AutoResetEvent automationTrigger;
        private IStorage storage;
		private IProcessingManager processingManager;
		private int delayBetweenCommands;
        private IConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationManager"/> class.
        /// </summary>
        /// <param name="storage">The storage.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="automationTrigger">The automation trigger.</param>
        /// <param name="configuration">The configuration.</param>
        public AutomationManager(IStorage storage, IProcessingManager processingManager, AutoResetEvent automationTrigger, IConfiguration configuration)
		{
			this.storage = storage;
			this.processingManager = processingManager;
            this.configuration = configuration;
            this.automationTrigger = automationTrigger;
        }

        /// <summary>
        /// Initializes and starts the threads.
        /// </summary>
		private void InitializeAndStartThreads()
		{
			InitializeAutomationWorkerThread();
			StartAutomationWorkerThread();
		}

        /// <summary>
        /// Initializes the automation worker thread.
        /// </summary>
		private void InitializeAutomationWorkerThread()
		{
			automationWorkerDigital = new Thread(AutomationWorker_Digital);
			automationWorkerDigital.Name = "Automation Digital Thread";
			
			automationWorkerAnalog = new Thread(AutomationWorker_Analog);
			automationWorkerAnalog.Name = "Automation Analog Thread";
		}

        /// <summary>
        /// Starts the automation worker thread.
        /// </summary>
		private void StartAutomationWorkerThread()
		{
			automationWorkerDigital.Start();
			automationWorkerAnalog.Start();
		}


		//Citanje digitalnih izlaza
		private void AutomationWorker_Digital()
		{
			PointIdentifier TM1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4000);
            PointIdentifier TM2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4001);
            PointIdentifier WM1 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4002);
            PointIdentifier WM2 = new PointIdentifier(PointType.DIGITAL_OUTPUT, 4003);
            List<PointIdentifier> list = new List<PointIdentifier> { TM1, TM2, WM1, WM2 };
			while (!disposedValue)
			{
				List<IPoint> points = storage.GetPoints(list);

				Thread.Sleep(2000);
				processingManager.ExecuteReadCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[0].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[1].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[1].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[2].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[3].ConfigItem.StartAddress, 1);
			}
        }

		//Citanje analognih ulaza
		private void AutomationWorker_Analog()
		{
            PointIdentifier SC = new PointIdentifier(PointType.ANALOG_INPUT, 3300);
            PointIdentifier LC = new PointIdentifier(PointType.ANALOG_INPUT, 3301);
            PointIdentifier HRM1 = new PointIdentifier(PointType.ANALOG_INPUT, 3302);
            PointIdentifier HRM2 = new PointIdentifier(PointType.ANALOG_INPUT, 3303);
            List<PointIdentifier> list = new List<PointIdentifier> { SC, LC, HRM1, HRM2 };
            while (!disposedValue)
            {
                List<IPoint> points = storage.GetPoints(list);

                Thread.Sleep(4000);
                processingManager.ExecuteReadCommand(points[0].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[0].ConfigItem.StartAddress, 1);
                processingManager.ExecuteReadCommand(points[1].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[1].ConfigItem.StartAddress, 1);
                processingManager.ExecuteReadCommand(points[2].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[2].ConfigItem.StartAddress, 1);
				processingManager.ExecuteReadCommand(points[3].ConfigItem, configuration.GetTransactionId(), configuration.UnitAddress, points[3].ConfigItem.StartAddress, 1);
            }
        }

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls


        /// <summary>
        /// Disposes the object.
        /// </summary>
        /// <param name="disposing">Indication if managed objects should be disposed.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
				}
				disposedValue = true;
			}
		}


		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// GC.SuppressFinalize(this);
		}

        /// <inheritdoc />
        public void Start(int delayBetweenCommands)
		{
			this.delayBetweenCommands = delayBetweenCommands*1000;
            InitializeAndStartThreads();
		}

        /// <inheritdoc />
        public void Stop()
		{
			Dispose();
		}
		#endregion
	}
}
