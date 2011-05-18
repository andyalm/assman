using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Assman.BuildSupport.MSBuild
{
	/// <summary>
	/// Consolidates javascript and css resources into fewer files according to the
	/// resource management configuration in the web.config.
	/// </summary>
	public class PreCompileResources : Task
	{
		/// <summary>
		/// Gets or sets the physical path the the root of the website.
		/// </summary>
		[Required]
		public ITaskItem WebRoot { get; set; }

		/// <summary>
		/// Indicates whether to consolidate the scripts in Debug or Release mode.
		/// </summary>
		public ResourceMode Mode { get; set; }

		/// <summary>
		/// Optionally applies the given version to be used in all script/style includes.  If left null,
		/// the version will be generated based on the Date/Time the scripts and styles were consolidated.
		/// </summary>
		public string Version { get; set; }

		/// <summary>
		/// Forces the task to try to launch a debugger when executed.  Default is <c>false</c>.
		/// </summary>
		public bool DebugTask { get; set; }

		public PreCompileResources()
		{
			Mode = ResourceMode.Release;
		}

		/// <summary>
		/// Executes the task.
		/// </summary>
		/// <returns></returns>
		public override bool Execute()
		{
			if (DebugTask)
				Debugger.Launch();

			PreCompileCommand cmd = PreCompileCommand.GetInstance(FullPathToWebsiteDirectory);
			cmd.Mode = this.Mode;
			cmd.Version = this.Version;
			cmd.Logger = new MSBuildLogger(this.Log);
			cmd.Execute();

			return true;
		}

	   
		private string FullPathToWebsiteDirectory
		{
			get
			{
				return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(BuildEngine.ProjectFileOfTaskNode), WebRoot.ItemSpec));
			}
		}

		private class MSBuildLogger : ILogger
		{
			private readonly TaskLoggingHelper _logHelper;

			public MSBuildLogger(TaskLoggingHelper logHelper)
			{
				_logHelper = logHelper;
			}

			public void LogMessage(string message)
			{
				_logHelper.LogMessage(MessageImportance.Normal, message);
			}
		}
	}
}
