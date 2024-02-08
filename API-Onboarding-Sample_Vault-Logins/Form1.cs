using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using ACW = Autodesk.Connectivity.WebServices;
using ACWTools = Autodesk.Connectivity.WebServicesTools;
using Framework = Autodesk.DataManagement.Client.Framework;
using Vault = Autodesk.DataManagement.Client.Framework.Vault;
using Forms = Autodesk.DataManagement.Client.Framework.Vault.Forms;

namespace API_Onboarding_Sample_Vault_Logins
{
    public partial class Form1 : Form
    {

        private ACW.ServerIdentities mServerId = new ACW.ServerIdentities();
        private string mVaultName = null;
        private string mUserName = null;
        private string mPassword = null;

        private Vault.Currency.Connections.Connection conn = null;

        public Form1()
        {
            InitializeComponent();

            //Initialize the Vault Forms library
            Forms.Library.Initialize();
        }

        /// <summary>
        /// WebServiceManager, Vault Account Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            mServerId.DataServer = this.txtServer.Text;
            mServerId.FileServer = this.txtServer.Text;
            mUserName = this.txtUserName.Text;
            mPassword = this.txtPassword.Text;
            mVaultName = this.txtVaultName.Text;
            ACW.LicensingAgent mLicAgent = ACW.LicensingAgent.Client;
            ACWTools.WebServiceManager mVault = null;
            ACWTools.UserPasswordCredentials mCred = null;

            mCred = new ACWTools.UserPasswordCredentials(mServerId, mVaultName, mUserName, mPassword, mLicAgent);
            try
            {
                mVault = new ACWTools.WebServiceManager(mCred);
            }
            catch (Exception)
            {
                mUserFeedback(null);
                return;
            }          

            // get connection
            conn = new Vault.Currency.Connections.Connection(mVault, mVaultName, mVault.SecurityService.Session.User.Id,
                mServerId.DataServer, Vault.Currency.Connections.AuthenticationFlags.Standard);

            // feed back the login state 
            mUserFeedback(conn);

            mVault.Dispose();
            Vault.Library.ConnectionManager.CloseAllConnections();
        }


        /// <summary>
        /// WebServiceManager, Autodesk ID Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            mServerId.DataServer = this.txtServer.Text;
            mServerId.FileServer = this.txtServer.Text;
            mUserName = this.txtUserName.Text;
            mPassword = this.txtPassword.Text;
            mVaultName = this.txtVaultName.Text;

            ACWTools.WebServiceManager mVault = null;

            ACW.IAutodeskAccount autodeskAccount = ACWTools.AutodeskAccount.Login(hwnd: IntPtr.Zero);
            String token = autodeskAccount.GetAccessToken();
            ACWTools.AutodeskAuthCredentials autodeskAuthCredentials = new ACWTools.AutodeskAuthCredentials(mServerId, mVaultName, autodeskAccount, token);

            try
            {
                mVault = new ACWTools.WebServiceManager(autodeskAuthCredentials);
            }
            catch (Exception)
            {
                mUserFeedback(null);
                return;
            }

            // get connection
            conn = new Vault.Currency.Connections.Connection(mVault, mVaultName, mVault.SecurityService.Session.User.Id,
                mServerId.DataServer, Vault.Currency.Connections.AuthenticationFlags.Standard);

            // feed back the login state 
            mUserFeedback(conn);

            mVault.Dispose();
            Vault.Library.ConnectionManager.CloseAllConnections();
        }


        /// <summary>
        /// WebServiceManager, Windows Account Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            mServerId.DataServer = this.txtServer.Text;
            mServerId.FileServer = this.txtServer.Text;
            mUserName = this.txtUserName.Text;
            mPassword = this.txtPassword.Text;
            mVaultName = this.txtVaultName.Text;
            ACW.LicensingAgent mLicAgent = ACW.LicensingAgent.Client;
            ACWTools.WebServiceManager mVault = null;
            ACWTools.UserPasswordCredentials mCred = null;

            ACWTools.WinAuthCredentials winAuthCredentials = new ACWTools.WinAuthCredentials(mServerId, mVaultName, null, mLicAgent);

            try
            {
                mVault = new ACWTools.WebServiceManager(winAuthCredentials);
            }
            catch (Exception)
            {
                mUserFeedback(null);
                return;
            }

            // get connection
            conn = new Vault.Currency.Connections.Connection(mVault, mVaultName, mVault.SecurityService.Session.User.Id,
                mServerId.DataServer, Vault.Currency.Connections.AuthenticationFlags.Standard);

            // feed back the login state 
            mUserFeedback(conn);

            mVault.Dispose();
            Vault.Library.ConnectionManager.CloseAllConnections();
        }


        /// <summary>
        /// VDF Login
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            this.conn = Vault.Forms.Library.Login(null);
            mUserFeedback(conn);

            Vault.Library.ConnectionManager.CloseAllConnections();
        }

        private void mUserFeedback(Vault.Currency.Connections.Connection connection)
        {
            if (connection == null)
            {
                Framework.Forms.Library.ShowError("The login failed. Check your connection inputs.", "Vault-API-Sample");
            }
            else
            {
                string mUserName = connection.WebServiceManager.AuthService.Session.User.Name;
                Framework.Forms.Library.ShowMessage("Great! You successfully logged in to " + connection.Vault + " with user name: " + mUserName, "Vault-API-Sample", Framework.Forms.Currency.ButtonConfiguration.None);
            }
        }

    }
}
