using CoreAppUWP.Common;
using CoreAppUWP.Helpers;
using CoreAppUWP.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System;
using Windows.ApplicationModel.Core;
using Windows.System.Profile;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CoreAppUWP
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            UnhandledException += Application_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            FocusVisualKind = AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Xbox" ? FocusVisualKind.Reveal : FocusVisualKind.HighVisibility;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            EnsureWindow(args);
        }

        private void EnsureWindow(LaunchActivatedEventArgs e)
        {
            if (!isLoaded)
            {
                RegisterExceptionHandlingSynchronizationContext();
                isLoaded = true;
            }

            Window window = Window.Current;

            // ��Ҫ�ڴ����Ѱ�������ʱ�ظ�Ӧ�ó����ʼ����
            // ֻ��ȷ�����ڴ��ڻ״̬
            if (window.Content is not Frame rootFrame)
            {
                if (SettingsHelper.Get<bool>(SettingsHelper.IsExtendsTitleBar))
                {
                    CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
                }

                // ����Ҫ�䵱���������ĵĿ�ܣ�����������һҳ
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.UWPLaunchActivatedEventArgs.PreviousExecutionState == Windows.ApplicationModel.Activation.ApplicationExecutionState.Terminated)
                {
                    //TODO: ��֮ǰ�����Ӧ�ó������״̬
                }

                // ����ܷ��ڵ�ǰ������
                window.Content = rootFrame;

                WindowHelper.TrackWindow(window);
                ThemeHelper.Initialize();
            }

            if (e is LaunchActivatedEventArgs args)
            {
                if (!args.UWPLaunchActivatedEventArgs.PrelaunchActivated)
                {
                    CoreApplication.EnablePrelaunch(true);
                }
                else { return; }
            }

            if (rootFrame.Content == null)
            {
                // ��������ջ��δ��ԭʱ����������һҳ��
                // ��ͨ����������Ϣ��Ϊ������������������
                // ����
                rootFrame.Navigate(typeof(MainPage), e);
            }

            // ȷ����ǰ���ڴ��ڻ״̬
            window.Activate();
        }

        /// <summary>
        /// �������ض�ҳʧ��ʱ����
        /// </summary>
        ///<param name="sender">����ʧ�ܵĿ��</param>
        ///<param name="e">�йص���ʧ�ܵ���ϸ��Ϣ</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void Application_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            SettingsHelper.LogManager?.GetLogger("Unhandled Exception - Application").Error(e.Exception.ExceptionToMessage(), e.Exception);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception Exception)
            {
                SettingsHelper.LogManager?.GetLogger("Unhandled Exception - CurrentDomain").Error(Exception.ExceptionToMessage(), Exception);
            }
        }

        /// <summary>
        /// Should be called from OnActivated and OnLaunched
        /// </summary>
        private void RegisterExceptionHandlingSynchronizationContext()
        {
            ExceptionHandlingSynchronizationContext
                .Register()
                .UnhandledException += SynchronizationContext_UnhandledException;
        }

        private void SynchronizationContext_UnhandledException(object sender, Common.UnhandledExceptionEventArgs e)
        {
            SettingsHelper.LogManager?.GetLogger("Unhandled Exception - SynchronizationContext").Error(e.Exception.ExceptionToMessage(), e.Exception);
            e.Handled = true;
        }

        private bool isLoaded;
    }
}
