using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Facebook;

namespace PhoneApp1
{
    public partial class FacebookLogin : PhoneApplicationPage
    {
        public FacebookLogin()
        {
            InitializeComponent();
        }
        private const string ExtendedPermissions = "user_about_me,publish_stream";
        private readonly FacebookClient _fb = new FacebookClient();
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {

            base.OnBackKeyPress(e);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = FacebookSettings.AppID;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "page";
            parameters["scope"] = ExtendedPermissions;
            FBLogin.Navigate(_fb.GetLoginUrl(parameters));
        }

        private void Browser_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;
            if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                return;
            }

            if (oauthResult.IsSuccess)
            {
                var accessToken = oauthResult.AccessToken;
                LoginSucceded(accessToken);
            }
            else
            {
                MessageBox.Show(oauthResult.ErrorDescription);
            }
        }

        private void LoginSucceded(string accessToken)
        {
            var fb = new FacebookClient(accessToken);

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                var id = (string)result["id"];

                SerializeHelper.SaveSetting<FacebookAccess>("FacebookAccess", new FacebookAccess
                {
                    AccessToken = accessToken,
                    UserId = id
                });
            };

            fb.GetAsync("me?fields=id");
            fb.GetCompleted += (o, args) =>
            {
               
                UIThread.Invoke(() => NavigationService.GoBack());
            };
        }
    }
}