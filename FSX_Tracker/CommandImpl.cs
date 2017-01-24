using System;
using System.Windows.Input;

namespace FSX_Tracker
{
    public class CommandImpl : ICommand
    {


        public event EventHandler CanExecuteChanged;
        protected Action _callback;

        public CommandImpl(Action callback)
        {
            _callback = callback;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _callback?.Invoke();
        }
    }

    public class CommandImpl<T> : ICommand
    {


        public event EventHandler CanExecuteChanged;
        protected Action<T> _callback;

        public CommandImpl(Action<T> callback)
        {
            _callback = callback;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            T cast = default(T);
            try
            {
                cast = (T)parameter;
            }
            catch (Exception)
            {
            }
            _callback?.Invoke(cast);
        }
    }
}
