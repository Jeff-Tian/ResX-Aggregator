using System;
using System.IO;
using System.Management;
using System.Threading;

namespace ZiZhuJY.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class WMICmdHelper
    {
        internal const string DEFAULT_USERNAME = "Administrator";
        internal const string DEFAULT_PASSWORD = "#Bugsfor$";

        private string machine;
        private string command;
        private string userName;
        private string password;

        private uint processId;
        public int ExitCode { get; set; }
        public bool ProcessExited { get; set; }
        public string WorkingDirectory { get; set; }
        public string Domain { get; set; }
        public bool EventArrived { get; set; }
        public ManualResetEvent ManualResetEventForWatchingEventArrived = new ManualResetEvent(false);
        public bool ExitCodeCaptured { get; set; }

        public WMICmdHelper(string machine, string command, string userName = DEFAULT_USERNAME, string password = DEFAULT_PASSWORD, string domain = ".")
        {
            this.machine = machine;
            this.command = command;
            this.userName = userName;
            this.password = password;
            this.Domain = domain;

            this.processId = 0;
            this.ExitCode = -1;
            this.EventArrived = false;

            try
            {
                this.WorkingDirectory = Path.GetDirectoryName(command.Split(' ')[0]);
            }
            catch
            {
                this.WorkingDirectory = "C:\\";
            }
        }

        private string localSystemDrive;
        public string LocalSystemDrive
        {
            get
            {
                if (localSystemDrive == null)
                {
                    ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        localSystemDrive = Convert.ToString(obj["SystemDrive"]);
                        break;
                    }
                }

                return localSystemDrive;
            }
        }

        private ManagementScope ManagementScope
        {
            get
            {
                //Log.Info("Current this machine name: {0}; remote machine name: {1}".Format2(System.Environment.MachineName, this.machine));
                if (System.Environment.MachineName.Equals(this.machine))
                {
                    return new ManagementScope()
                    {
                        Path = new ManagementPath(@"\\" + machine + @"\root\cimv2")
                    };
                }
                else
                {
                    return new ManagementScope()
                    {
                        Options = new ConnectionOptions()
                        {
                            Username = userName,
                            Password = password,
                            EnablePrivileges = true,
                            Impersonation = ImpersonationLevel.Impersonate,
                            Authority = "NTLMDOMAIN:" + this.Domain
                            ////Authentication = AuthenticationLevel.PacketPrivacy,
                            ////Authority = "NTLMDOMAIN:FAREAST"
                        },
                        Path = new ManagementPath(@"\\" + machine + @"\root\cimv2")
                    };
                }
            }
        }

        public UInt32 RunCommand()
        {
            return RunCommand(this.command);
        }

        public UInt32 RunCommand(string command)
        {
            try
            {
                this.ManagementScope.Connect();
            }
            catch (Exception ex)
            {
                throw new Exception("Management connect to remote machine {0} as user {1} failed with the following error {2}".FormatWith(this.machine, this.userName, ex.Message), ex);
            }

            ObjectGetOptions objectGetOptions = new ObjectGetOptions();
            ManagementPath managementPath = new ManagementPath("Win32_Process");

            this.ExitCode = 0;
            this.ProcessExited = false;
            Log.Info("Starting command: {0} on remote machine {1}", command, this.machine);

            using (ManagementClass managementClass = new ManagementClass(this.ManagementScope, managementPath, objectGetOptions))
            {
                using (ManagementBaseObject input = managementClass.GetMethodParameters("Create"))
                {
                    input.SetPropertyValue("CommandLine", command);
                    input.SetPropertyValue("CurrentDirectory", this.WorkingDirectory);
                    Log.Info("Used working directory: {0}".FormatWith(input.GetPropertyValue("CurrentDirectory").ToString()));

                    using (ManagementBaseObject output = managementClass.InvokeMethod("Create", input, null))
                    {
                        try
                        {
                            if ((uint)output["returnValue"] != 0)
                            {
                                throw new Exception("Error while starting process " + command + ".\r\nCreation returned an exit code of " + output["returnValue"] + ". It was launched as " + this.userName + " on " + this.machine);
                            }
                            else
                            {
                                this.processId = Convert.ToUInt32(output.GetPropertyValue("ProcessID").ToString());

                                Log.Info("Started command: {0} on remote machine {1}\r\nProcessId: {2}", command, this.machine, this.processId);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Failed to start process with command '{0}' on machine {1}.".FormatWith(command, this.machine));
                            ExceptionHelper.CentralProcess(ex);
                        }
                    }
                }
            }

            return this.processId;
        }


        /// <summary>
        /// Runs the command but doesn't change any class member variables
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public UInt32 RunCommandInline(string command)
        {
            UInt32 processId = 0;
            try
            {
                this.ManagementScope.Connect();
            }
            catch (Exception ex)
            {
                throw new Exception("Management connect to remote machine {0} as user {1} failed with the following error {2}".FormatWith(this.machine, this.userName, ex.Message), ex);
            }

            ObjectGetOptions objectGetOptions = new ObjectGetOptions();
            ManagementPath managementPath = new ManagementPath("Win32_Process");

            Log.Info("Starting command: {0} on remote machine {1}", command, this.machine);

            using (ManagementClass managementClass = new ManagementClass(this.ManagementScope, managementPath, objectGetOptions))
            {
                using (ManagementBaseObject input = managementClass.GetMethodParameters("Create"))
                {
                    input.SetPropertyValue("CommandLine", command);
                    input.SetPropertyValue("CurrentDirectory", this.WorkingDirectory);
                    Log.Info("Used working directory: {0}".FormatWith(input.GetPropertyValue("CurrentDirectory").ToString()));

                    using (ManagementBaseObject output = managementClass.InvokeMethod("Create", input, null))
                    {
                        try
                        {
                            if ((uint)output["returnValue"] != 0)
                            {
                                throw new Exception("Error while starting process " + command + ".\r\nCreation returned an exit code of " + output["returnValue"] + ". It was launched as " + this.userName + " on " + this.machine);
                            }
                            else
                            {
                                processId = Convert.ToUInt32(output.GetPropertyValue("ProcessID").ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Error("Failed to start process with command '{0}' on machine {1}.".FormatWith(command, this.machine));
                            ExceptionHelper.CentralProcess(ex);
                        }
                    }
                }
            }

            return processId;
        }

        public void WaitForProcess(TimeSpan waitTimeout)
        {
            SelectQuery checkProcess = new SelectQuery("Select * from Win32_Process Where ProcessId = " + this.processId);
            using (ManagementObjectSearcher processSearcher = new ManagementObjectSearcher(this.ManagementScope, checkProcess))
            {
                using (ManagementObjectCollection moc = processSearcher.Get())
                {
                    if (moc.Count == 0)
                    {
                        Log.Error("ERROR AS WARNING: Process " + this.command + " terminated before it could be tracked on " + this.machine);
                    }
                }
            }

            // Try to start necessary services to make ManagementEventWatcher work
            try
            {
                this.RunCommandInline("net start RpcLocator");
                this.RunCommandInline("net start RemoteRegistry");
                this.RunCommandInline("net start RasAuto");
                this.RunCommandInline("net start RasMan");
            }
            catch (Exception ex)
            {
                ExceptionHelper.CentralProcess(ex);
            }
            finally
            {
            }

            try
            {
                #region Better waiting methodology
                WqlEventQuery wqlEventQuery = new WqlEventQuery("Win32_ProccessStopTrace");
                using (ManagementEventWatcher watcher = new ManagementEventWatcher(this.ManagementScope, wqlEventQuery))
                {
                    watcher.EventArrived += new EventArrivedEventHandler(this.ProcessStopEventArrived);
                    watcher.Start();
                    if (!this.ManualResetEventForWatchingEventArrived.WaitOne(waitTimeout, false))
                    {
                        watcher.Stop();
                        this.EventArrived = false;
                    }
                    else
                    {
                        watcher.Stop();
                    }
                }

                if (!this.EventArrived)
                {
                    SelectQuery sq = new SelectQuery("Select * from Win32_Process Where ProcessId = " + this.processId);
                    using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(this.ManagementScope, sq))
                    {
                        foreach (ManagementObject obj in searcher.Get())
                        {
                            obj.InvokeMethod("Terminate", null);
                            obj.Dispose();
                            throw new Exception("Process " + this.command + " timed out and was killed on " + this.machine);
                        }
                    }
                }
                else
                {
                    if (this.ExitCode != 0)
                    {
                        throw new Exception("Process " + this.command + " exited with exit code " + this.ExitCode + " on " + this.machine + " run as " + this.userName);
                    }
                    else
                    {
                        Log.Info("Process {0} exited with Exit Code 0", this.processId);
                    }
                }
                #endregion Better waiting methodology
            }
            catch (Exception ex)
            {
                ExceptionHelper.CentralProcess(ex);

                #region Safer waiting methodology
                try
                {
                    Log.Info("Waiting for process {0}, command: {1} to finish on remote machine {2}", this.processId, this.command, this.machine);
                    ThreadHelper.PollCondition pollCondition = new ThreadHelper.PollCondition(delegate()
                    {
                        if (this.ProcessExists())
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    });

                    bool timedOut = !ThreadHelper.PollWait(pollCondition, TimeSpan.FromSeconds(30), waitTimeout);
                    if (timedOut)
                        Log.Info("Timed out after {2} second(s) in waiting process id {0} on remote machine {1}, exit waiting.", processId, machine, waitTimeout.TotalSeconds);
                    else
                        Log.Info("Process id: {0} exited on remote machine {1}.".FormatWith(processId, machine));
                }
                catch (Exception exception)
                {
                    // Make sure this exception has been logged.
                    ExceptionHelper.CentralProcess(exception);
                    throw ex;
                }
                #endregion Safer waiting methodology
            }
        }

        public void ProcessStopEventArrived(object sender, EventArrivedEventArgs e)
        {
            if ((uint)e.NewEvent.Properties["ProcessId"].Value == this.processId)
            {
                Log.Info("Process: {0}, stopped with code: {1}", (int)(uint)e.NewEvent.Properties["ProcessId"].Value, (int)(uint)e.NewEvent.Properties["ExitStatus"].Value);
                this.ExitCode = (int)(uint)e.NewEvent.Properties["ExitStatus"].Value;
                this.ExitCodeCaptured = true;
                this.EventArrived = true;
                this.ManualResetEventForWatchingEventArrived.Set();
            }
        }

        public bool ProcessExists()
        {
            string query = "SELECT * FROM Win32_Process WHERE ProcessID={0}".FormatWith(this.processId);
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(ManagementScope, new SelectQuery(query));
            ManagementObjectCollection processes = searcher.Get();
            return (processes.Count > 0);
        }

        public void WaitForProcess()
        {
            WaitForProcess(TimeSpan.MaxValue);
        }

        public string RunCommandReturnOutput()
        {
            //string outputFileName = @"WMICmdOutput.{0}.txt".Format2(DateTime.Now.ToString("yyyy-MM-ddThh-mm-ssZ")) ;
            //string outputFileName = "text.txt";
            //this.command = "cmd /C " + this.command + " >C:\\" + outputFileName;
            this.command = "cmd /C " + this.command;

            uint pid = RunCommand();
            if (pid > 0)
            {
                WaitForProcess();

                /*
                string outputContent = "";
                try
                {
                    ////string fileFullName = @"\\{0}\C$\{1}".Format2(this.machine, outputFileName);
                    ////outputContent = File.ReadAllText(fileFullName);
                    ////File.Delete(fileFullName);
                }
                catch (Exception ex)
                {
                    outputContent += "\r\n" + ExceptionHelper.CentralProcess(ex);
                }

                Log.Info(outputContent);
                return outputContent;*/

                return "";
            }
            else
            {
                return "Failed to start this command:\r\n{0}\r\n".FormatWith(this.command);
            }
        }

        public void Shutdown()
        {
            try
            {
                this.ManagementScope.Connect();
            }
            catch (Exception ex)
            {
                throw new Exception("Management connect to remote machine {0} as user {1} failed with the following error {2}".FormatWith(this.machine, this.userName, ex.Message), ex);
            }

            ObjectQuery objQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(this.ManagementScope, objQuery);
            foreach (ManagementObject operatingSystem in objSearcher.Get())
            {
                Log.Info("Shuting down {{Caption = {0}, Version = {1}}}...".FormatWith(
                    operatingSystem.GetPropertyValue("Caption"),
                    operatingSystem.GetPropertyValue("Version")
                ));
                ManagementBaseObject outParams = operatingSystem.InvokeMethod("Shutdown", null, null);

                Log.Info("Shuting down {{Caption = {0}, Version = {1}}} done.".FormatWith(
                    operatingSystem.GetPropertyValue("Caption"),
                    operatingSystem.GetPropertyValue("Version")
                ));
            }
        }

        public void Reboot()
        {
            try
            {
                this.ManagementScope.Connect();
            }
            catch (Exception ex)
            {
                throw new Exception("Management connect to remote machine {0} as user {1} failed with the following error {2}".FormatWith(this.machine, this.userName, ex.Message), ex);
            }

            ObjectQuery objQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher objSearcher = new ManagementObjectSearcher(this.ManagementScope, objQuery);
            foreach (ManagementObject operatingSystem in objSearcher.Get())
            {
                Log.Info("Rebooting {{Caption = {0}, Version = {1}}}...".FormatWith(
                    operatingSystem.GetPropertyValue("Caption"),
                    operatingSystem.GetPropertyValue("Version")
                ));
                object result = operatingSystem.InvokeMethod("Reboot", new string[] { "" });

                Log.Info("Rebooting {{Caption = {0}, Version = {1}}} done.".FormatWith(
                    operatingSystem.GetPropertyValue("Caption"),
                    operatingSystem.GetPropertyValue("Version")
                ));
            }
        }

        #region Helpers
        /// <summary>
        /// Convert a remote path to what looks like from user on that remote machine. For example, "\\target\C$\test.txt" --> "C:\test.txt"
        /// </summary>
        /// <param name="path">The remote path.</param>
        /// <returns>The local path</returns>
        public static string PathOnTargetMachine(string path)
        {
            string localPath = path.Remove(0, path.IndexOf('\\', 2) + 1).Replace("$\\", ":\\");
            return localPath;
        }
        #endregion Helpers
    }
}
