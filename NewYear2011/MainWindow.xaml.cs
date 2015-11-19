using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Expression.Interactivity.Layout;
using Microsoft.Win32;
using NewYear2011.Dialog;
using NewYear2011.Gadget;
using NewYear2011.Helper;
using NewYear2011.ViewModel;
#if Garlic
using Garlic;
#endif

namespace NewYear2011
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MouseDragElementBehavior dragBehavior = new MouseDragElementBehavior();
        List<TreeGadget> gadgetInstances { get; set; }
        public MainWindow()
        {
            InitializeComponent();
        }

#if Garlic
        AnalyticsSession gal { get; set; } 
#endif
        MediaPlayer player = new MediaPlayer();
        private void MainWindow_Load(object sender, RoutedEventArgs e)
        {
            
            //System.Media.SoundPlayer player = new System.Media.SoundPlayer();
            //player.Play();


            Settings.Load();

            if (Settings.curSettings == null)
                SetFirstSettings();
            else
            {
                TreeTranslateTrans = (gridTree.RenderTransform as TransformGroup).Children[3] as TranslateTransform;
                TreeTranslateTrans.X = Settings.curSettings.TreeX;
                TreeTranslateTrans.Y = Settings.curSettings.TreeY;
                TreeScaleTrans.ScaleX = Settings.curSettings.TreeScale;
                TreeScaleTrans.ScaleY = Settings.curSettings.TreeScale;

                gadgetInstances = new List<TreeGadget>();

                LoadGadgets();
            }
            dragBehavior.Attach(gridTree);

            if (Settings.curSettings.PlaySound)
            {
                player.Open(new Uri(MainHelper.GetApplicationDir() + "\\sound\\elkasound.mp3"));
                player.Play();
            }

#if Garlic
            gal = new AnalyticsSession("newyear.andromeda.mn", "UA-6790599-40");
            gal.SetCustomVariable(2, "ComputerName", Environment.MachineName);
            gal.SetCustomVariable(3, "OSVersion", Environment.OSVersion.ToString());
            gal.SetCustomVariable(4, "UserName", Environment.UserName);
            var mainGal = gal.CreatePageViewRequest("/Tree", "");
            mainGal.Send();  
#endif
        }

        private void LoadGadgets()
        {
            foreach (var item in Settings.curSettings.gadgetInstances)
            {
                TreeGadget g = new TreeGadget() { Source = item.imgPath, Scale = item.Scale, IsMoveable = true };
                g.RemoveMeRequest += new EventHandler(gadget_RemoveMeRequest);
                TranslateTransform tt = (g.RenderTransform as TransformGroup).Children[3] as TranslateTransform;
                tt.X = item.TranslateX;
                tt.Y = item.TranslateY;
                gadgetInstances.Add(g);
            }

            foreach (var item in gadgetInstances)
            {
                gridTree.Children.Add(item);
            }


        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.curSettings.TreeX = gridTree.RenderTransform.Value.OffsetX;
            Settings.curSettings.TreeY = gridTree.RenderTransform.Value.OffsetY;
            Settings.curSettings.TreeScale = TreeScaleTrans.ScaleX;
            Settings.curSettings.TreeScale = TreeScaleTrans.ScaleY;
            Settings.curSettings.gadgetInstances.Clear();
            foreach (var item in gadgetInstances)
            {
                TreeGadgetSettings gs = new TreeGadgetSettings();
                gs.imgPath = item.Source;
                gs.Scale = item.Scale;
                gs.TranslateX = item.RenderTransform.Value.OffsetX;
                gs.TranslateY = item.RenderTransform.Value.OffsetY;
                Settings.curSettings.gadgetInstances.Add(gs);
            }

            Settings.Save();
        }
        Boolean _isMoveMode = false;
        public Boolean isMoveMode
        {
            get { return _isMoveMode; }
            set
            {
                _isMoveMode = value;
                if (value)
                {
                    miFinishMove.Visibility = System.Windows.Visibility.Visible;
                    miMoveGadget.Visibility = System.Windows.Visibility.Collapsed;
                    miDeleteTip.Visibility = System.Windows.Visibility.Visible;
                    miDeleteSep.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    miFinishMove.Visibility = System.Windows.Visibility.Collapsed;
                    miMoveGadget.Visibility = System.Windows.Visibility.Visible;
                    miDeleteTip.Visibility = System.Windows.Visibility.Collapsed;
                    miDeleteSep.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }
        private void cmMoveGadget_Click(object sender, RoutedEventArgs e)
        {
            MoveGadget();
        }

        private void MoveGadget()
        {
            dragBehavior.Detach();
            borTree.Opacity = 0.7;
            foreach (var item in gadgetInstances.Where(t => t.IsMoveable))
            {
                if (item.DragBehavior == null)
                    item.DragBehavior = new MouseDragElementBehavior();
                item.DragBehavior.Attach(item);
                //dragBehavior.Attach(item);                
            }
            isMoveMode = true;

        }

        private void cmFinishMove_Click(object sender, RoutedEventArgs e)
        {
            if (Settings.curSettings.PlaySound)
            {
                player.Open(new Uri(MainHelper.GetApplicationDir() + "\\sound\\elkasound.mp3"));
                player.Play();
            }
            dragBehavior.Attach(gridTree);

            foreach (var item in gadgetInstances)
            {
                if (item.DragBehavior != null)
                    item.DragBehavior.Detach();
            }

            isMoveMode = false;
            borTree.Opacity = 1;
        }

        private void cmSettings_Click(object sender, RoutedEventArgs e)
        {

            winSettings settings = new winSettings();
            if (Settings.curSettings == null)
                Settings.curSettings = new Settings();
            settings.SetDefaultSettings += new EventHandler(settings_SetDefaultSettings);
            settings.chStartupOption.IsChecked = Settings.curSettings.RunOnStartup;
            settings.isPlaySound.IsChecked = Settings.curSettings.PlaySound;
            double prevValue = settings.sliderSize.Value;
            settings.sliderSize.Value = TreeScaleTrans.ScaleX * 100;
            settings.sliderSize.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderSize_ValueChanged);

            //settings.sliderSize.
            if (settings.ShowDialog().HasValue)
            {
                try
                {
                    string keyName = @"Software\Microsoft\Windows\CurrentVersion\Run";
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(keyName, true))
                    {
                        if (settings.chStartupOption.IsChecked.HasValue && settings.chStartupOption.IsChecked.Value)
                        {
                            key.SetValue("NewYear2011", System.Reflection.Assembly.GetExecutingAssembly().Location);
                            Settings.curSettings.RunOnStartup = true;
                        }
                        else
                        {
                            if (key.GetValue("NewYear2011", null) != null)
                            {
                                key.DeleteValue("NewYear2011");
                                Settings.curSettings.RunOnStartup = false;
                            }
                        }
                    }
                    if (settings.isPlaySound.IsChecked.HasValue)
                        Settings.curSettings.PlaySound = settings.isPlaySound.IsChecked.Value;
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }
            }
            else
            {
                TreeScaleTrans.ScaleX = prevValue;
                TreeScaleTrans.ScaleY = prevValue;
            }

        }

        void settings_SetDefaultSettings(object sender, EventArgs e)
        {
            SetFirstSettings();
        }

        private void SetFirstSettings()
        {
            Settings.Load(true);
            if (Settings.curSettings == null)
                Settings.curSettings = new Settings();

            TreeTranslateTrans = (gridTree.RenderTransform as TransformGroup).Children[3] as TranslateTransform;

            TreeTranslateTrans.X = System.Windows.SystemParameters.PrimaryScreenWidth - 310;
            TreeTranslateTrans.Y = System.Windows.SystemParameters.PrimaryScreenHeight - 480;
            Settings.curSettings.TreeScale = 0.8;
            TreeScaleTrans.ScaleX = Settings.curSettings.TreeScale;
            TreeScaleTrans.ScaleY = Settings.curSettings.TreeScale;

            gadgetInstances = new List<TreeGadget>();

            gridTree.Children.RemoveRange(1, gridTree.Children.Count - 1);
            LoadGadgets();
        }

        void sliderSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            ScaleTransform st = (gridTree.RenderTransform as TransformGroup).Children[0] as ScaleTransform;
            st.ScaleX = s.Value / 100;
            st.ScaleY = s.Value / 100;
        }

        private void cmAbout_Click(object sender, RoutedEventArgs e)
        {
            winAbout about = new winAbout();
            about.ShowDialog();
        }

        private void cmExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void miAddGadget_Click(object sender, RoutedEventArgs e)
        {

            winAddGadget addGadget = new winAddGadget();
            TreeGadgetOpener tgo = new TreeGadgetOpener();
            addGadget.DataContext = tgo;
            tgo.onInsertGadget += new InsertGadgetHandler(tgo_onInsertGadget);
            addGadget.ShowDialog();
            if (!isMoveMode)
                MoveGadget();
        }

        void tgo_onInsertGadget(int size, string path)
        {
            double scale = 1;
            switch (size)
            {
                case 3: scale = 0.6; break;
                case 2: scale = 0.5; break;
                case 1: scale = 0.4; break;
            }
            TreeGadget gadget = new TreeGadget() { Source = System.IO.Path.GetFileName(path), IsMoveable = true, Scale = scale };
            gadget.RemoveMeRequest += new EventHandler(gadget_RemoveMeRequest);
            if (isMoveMode)
            {
                if (gadget.DragBehavior == null)
                    gadget.DragBehavior = new MouseDragElementBehavior();
                gadget.DragBehavior.Attach(gadget);
            }
            gadgetInstances.Add(gadget);
            gridTree.Children.Add(gadget);
        }

        void gadget_RemoveMeRequest(object sender, EventArgs e)
        {
            TreeGadget g = sender as TreeGadget;
            if (_isMoveMode)
            {
                gridTree.Children.Remove(g);
                gadgetInstances.Remove(g);
            }
        }



    }
}
