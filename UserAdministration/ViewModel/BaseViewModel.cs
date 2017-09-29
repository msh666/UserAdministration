using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace UserAdministration.ViewModel
{
    /// <summary>
    /// Base ViewModel implements methods for property changin and widows closing
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        internal void OnPropertyChanged(string prop)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool? _closeWindowFlag;
        public bool? CloseWindowFlag
        {
            get { return _closeWindowFlag; }
            set
            {
                _closeWindowFlag = value;
                OnPropertyChanged("CloseWindowFlag");
            }
        }

        /// <summary>
        /// Method that close current window
        /// </summary>
        /// <param name="result"></param>
        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null
                    ? true
                    : !CloseWindowFlag;
            }));
        }
    }
}
