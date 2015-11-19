using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using Microsoft.Win32;
using NewYear2011.Helper;

namespace NewYear2011.ViewModel
{
    public class TreeGadgetOpener : ViewModelBase
    {
        public ObservableCollection<GadgetItem> AvailableItems { get; set; }

        public event InsertGadgetHandler onInsertGadget;

        public TreeGadgetOpener()
        {
            AvailableItems = new ObservableCollection<GadgetItem>();
            DirectoryInfo di = new DirectoryInfo(MainHelper.GetApplicationDir() + "\\Images\\");
            foreach (var item in di.GetFiles())
            {
                if (File.Exists(item.FullName))
                {
                    AvailableItems.Add(new GadgetItem() { ImagePath = item.FullName, owner = this });
                }
            }
        }

        public void InsertGadget(int size, string path)
        {
            if (onInsertGadget != null)
                onInsertGadget(size, path);
        }

        ICommand _btnAddPic;
        public ICommand btnAddPic
        {
            get
            {
                if (_btnAddPic == null)
                {
                    _btnAddPic = new RelayCommand(param =>
                    {
                        OpenFileDialog od = new OpenFileDialog();
                        od.Filter = "Portable Network Graphic (PNG)|*.png|JPG|*.jpg";
                        od.Multiselect = true;
                        if (od.ShowDialog().HasValue)
                        {
                            foreach (var item in od.FileNames)
                            {
                                if (File.Exists(item))
                                {
                                    try
                                    {
                                        string fname = System.IO.Path.GetFileName(item);
                                        if (System.IO.Path.GetDirectoryName(item) != MainHelper.GetApplicationDir())
                                        {
                                            if (File.Exists(MainHelper.GetApplicationDir() + "\\" + fname))
                                            {
                                                fname = string.Format("{0}{1:ddmmss}.{2}", System.IO.Path.GetFileNameWithoutExtension(fname), DateTime.Now, System.IO.Path.GetExtension(fname));                                                
                                            }
                                            File.Copy(item, MainHelper.GetApplicationDir() + "\\Images\\" + fname, true);
                                        }
                                        //TreeGadget gadget = new TreeGadget() { Source = fname, IsMoveable = true, Scale = 0.5 };
                                        //treeGadgets.Add(gadget);
                                        //gridTree.Children.Add(gadget);                                        
                                        fname = MainHelper.GetApplicationDir() + "\\Images\\" + fname;
                                        if (File.Exists(fname))
                                        {
                                            AvailableItems.Add(new GadgetItem() { ImagePath = fname, owner = this });
                                            OnPropertyChanged("AvailableItems");
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        continue;
                                    }
                                }
                            }
                        }
                    });
                }
                return _btnAddPic;
            }
        }
    }

    public class GadgetItem
    {
        public string ImagePath { get; set; }
        public TreeGadgetOpener owner { get; set; }

        ICommand _btnBig;
        public ICommand btnBig
        {
            get
            {
                if (_btnBig == null)
                {
                    _btnBig = new RelayCommand(param =>
                    {
                        owner.InsertGadget(3, (param as GadgetItem).ImagePath);
                    });
                }
                return _btnBig;
            }
        }

        ICommand _btnMiddle;
        public ICommand btnMiddle
        {
            get
            {
                if (_btnMiddle == null)
                {
                    _btnMiddle = new RelayCommand(param =>
                    {
                        owner.InsertGadget(2, (param as GadgetItem).ImagePath);
                    });
                }
                return _btnMiddle;
            }
        }

        ICommand _btnSmall;
        public ICommand btnSmall
        {
            get
            {
                if (_btnSmall == null)
                {
                    _btnSmall = new RelayCommand(param =>
                    {
                        owner.InsertGadget(1, (param as GadgetItem).ImagePath);
                    });
                }
                return _btnSmall;
            }
        }
    }

    public delegate void InsertGadgetHandler(int size, string path);
}
