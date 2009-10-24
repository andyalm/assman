using System;
using System.IO;
using System.Threading;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace AlmWitt.Web.ResourceManagement.BuildSupport.MSBuild
{
    /// <summary>
    /// Consolidates javascript and css resources into fewer files according to the
    /// resource management configuration in the web.config.
    /// </summary>
    public class PreConsolidateResources : Task
    {
        private ITaskItem m_webRoot;
        private bool m_sleep = false;

        /// <summary>
        /// Gets or sets the physical path the the root of the website.
        /// </summary>
        public ITaskItem WebRoot
        {
            get { return m_webRoot; }
            set { m_webRoot = value; }
        }

        /// <summary>
        /// Forces the task to sleep for 15 seconds before executing.  Default is <c>false</c>.
        /// </summary>
        /// <remarks>
        /// This is intended to be used for debugging purposes (to allow you time to attach to the msbuild process).
        /// </remarks>
        public bool Sleep
        {
            get { return m_sleep; }
            set { m_sleep = value; }
        }

        /// <summary>
        /// Executes the task.
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            if (Sleep)
                Thread.Sleep(15000);

            PreConsolidateCommand cmd = PreConsolidateCommand.GetInstance(BasePath);
            cmd.Logger = new MSBuildLogger(this.Log);
            cmd.Execute();

            return true;
        }

       
        private string BasePath
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
