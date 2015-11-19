using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Expression.Interactivity.Layout;
using NewYear2011.Helper;

namespace NewYear2011.Gadget
{
    /// <summary>
    /// Interaction logic for TreeGadget.xaml
    /// </summary>
    public partial class TreeGadget : UserControl
    {
        public TreeGadget()
        {
            InitializeComponent();
            IsMoveable = false;
        }

        public event EventHandler RemoveMeRequest;

        public string Source
        {
            get {
                if (MainBitmap.Source == null)
                    return "";
                return System.IO.Path.GetFileName((MainBitmap.Source as BitmapImage).UriSource.AbsolutePath); }
            set
            {
                string picPath = MainHelper.GetApplicationDir() + @"\images\" + value;
                if (File.Exists(picPath))
                    if (MainBitmap.Source != null)
                    {
                        (MainBitmap.Source as BitmapImage).UriSource = new Uri(picPath);
                    }
                    else
                        MainBitmap.Source = new BitmapImage(new Uri(picPath));

            }
        }

        public double Scale
        {
            get
            {
                return this.ScaleTrans.ScaleX;
            }
            set
            {
                this.ScaleTrans.ScaleX = value;
                this.ScaleTrans.ScaleY = value;
            }
        }

        Boolean _IsMoveable;
        public Boolean IsMoveable
        {
            get { return _IsMoveable; }
            set
            {
                _IsMoveable = value;
            }
        }

        public MouseDragElementBehavior DragBehavior { get; set; }

        private void UserControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
            {
                if (RemoveMeRequest != null)
                    RemoveMeRequest(this, null);
            }


        }

       
    }
}
