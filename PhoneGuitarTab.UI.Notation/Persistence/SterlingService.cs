using System;
using System.Windows;
using Wintellect.Sterling;
using System.ComponentModel;
using System.Diagnostics;

namespace PhoneGuitarTab.UI.Notation.Persistence
{
	public sealed class SterlingService : IApplicationService, IApplicationLifetimeAware, IDisposable
	{
		private SterlingEngine _engine;

		public static SterlingService Current
		{
			get;
			private set;
		}

		public ISterlingDatabaseInstance Database
		{
			get;
			private set;
		}

		private SterlingDefaultLogger _logger;

		public void StartService(ApplicationServiceContext context)
		{
			if (DesignerProperties.IsInDesignTool)
				return;
			_engine = new SterlingEngine();
			Current = this;
		}

		public void StopService()
		{
			return;
		}

		public void Starting()
		{
			if (DesignerProperties.IsInDesignTool)
				return;

			if (Debugger.IsAttached)
			{
				_logger = new SterlingDefaultLogger(SterlingLogLevel.Verbose);
			}


			_engine.Activate();
			Database = _engine.SterlingDatabase.RegisterDatabase<TabDataBaseInstance>();
		}

		public void Started()
		{
            //Tests.DbTest.InitDb();
			return;
		}

		public void Exiting()
		{
			if (DesignerProperties.IsInDesignTool)
				return;

			if (Debugger.IsAttached && _logger != null)
			{
				_logger.Detach();
			}
		}

		public void Exited()
		{
			Dispose();
			_engine = null;
			return;
		}

		public void Dispose()
		{
			if (_engine != null)
			{
				_engine.Dispose();
			}
			GC.SuppressFinalize(this);
		}
	}
}
