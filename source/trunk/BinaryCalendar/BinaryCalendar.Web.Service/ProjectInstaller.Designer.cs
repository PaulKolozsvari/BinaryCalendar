namespace BinaryCalendar.Web.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BinaryCalendarServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.BinaryCalendarServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // BinaryCalendarServiceProcessInstaller
            // 
            this.BinaryCalendarServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.BinaryCalendarServiceProcessInstaller.Password = null;
            this.BinaryCalendarServiceProcessInstaller.Username = null;
            // 
            // BinaryCalendarServiceInstaller
            // 
            this.BinaryCalendarServiceInstaller.Description = "Web service for querying calendar and public holidays.";
            this.BinaryCalendarServiceInstaller.ServiceName = "Binary Calendar Service";
            this.BinaryCalendarServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.BinaryCalendarServiceProcessInstaller,
            this.BinaryCalendarServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller BinaryCalendarServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller BinaryCalendarServiceInstaller;
    }
}