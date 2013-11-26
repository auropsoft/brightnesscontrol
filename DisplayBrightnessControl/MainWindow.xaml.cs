using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisplayBrightnessControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon ni;
        private bool isForceClose = false;
        private bool balloonShown;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void forceClose()
        {
            isForceClose = true;
            this.Close();
        }

        private void slider_changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double value = ((Slider)sender).Value;
            changeGamme(value);
        }

        private void changeGamme(double value)
        {
            int valint = Convert.ToInt32(value);
            double relval = 44 - value;
            int percentage = Convert.ToInt32(Math.Round(((double)100/(double)41) * relval));
            
            try
            {
                slider_label.Content = String.Format("{0}%", percentage);
            }
            catch { }

            Brightness.SetGamma(valint);
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            slider1.Value = 11;
        }

        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.brightness-control.net");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.ni.Dispose();
            changeGamme(11);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mbr = System.Windows.MessageBox.Show(this, "Would you rather close completely?","Minimizing to tray",MessageBoxButton.YesNoCancel,MessageBoxImage.Question);
            if (mbr.Equals(MessageBoxResult.Cancel))
            {
                e.Cancel = true;
            }
            else if(mbr.Equals(MessageBoxResult.No))
            {
                e.Cancel = true;
                this.MinimizeToTray();
            }
        }

        private void MinimizeToTray()
        {
            this.WindowState = System.Windows.WindowState.Minimized;
            this.ShowInTaskbar = false;
        }

        private void Window_StateChanged(object sender, EventArgs ex)
        {
            if (ni == null)
            {
                // Initialize NotifyIcon instance "on demand"
                this.ni = new NotifyIcon();
                var iconHandle = DisplayBrightnessControl.Properties.Resources.Icon.Handle;
                this.ni.Icon = System.Drawing.Icon.FromHandle(iconHandle);
                this.ni.Visible = false;
                this.ni.Text = this.Title;
                this.ni.BalloonTipTitle = this.Title;
                this.ni.BalloonTipText = this.Title + " is hiding in your tray now!";
                this.ni.MouseClick += new System.Windows.Forms.MouseEventHandler(HandleNotifyIconClicked);
                this.ni.ContextMenu = new System.Windows.Forms.ContextMenu(new System.Windows.Forms.MenuItem[] 
                { 
                    new System.Windows.Forms.MenuItem("Show Window", HandleNotifyIconClicked), 
                    new System.Windows.Forms.MenuItem("Exit", new EventHandler((s, e) => this.forceClose()))
                });
            }
            // Update copy of Window Title in case it has changed
            ni.Text = this.Title;

            // Show/hide Window and NotifyIcon
            var minimized = (this.WindowState == WindowState.Minimized);
            this.ShowInTaskbar = !minimized;
            ni.Visible = minimized;
            if (minimized && !this.balloonShown)
            {
                // If this is the first time minimizing to the tray, show the user what happened
                ni.ShowBalloonTip(1000);
                this.balloonShown = true;
            }

        }
        private void HandleNotifyIconClicked(object sender, EventArgs e)
        {
            // Restore the Window
            this.WindowState = WindowState.Normal;
            this.balloonShown = false;
        }
        
    }
}
