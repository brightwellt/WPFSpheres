using System;
using System.Diagnostics;
using System.Windows.Input;

namespace WpfSpheres
{
    /// <summary>
    /// Helper from Josh Smith which gives us nice Command syntax.
    /// </summary>
    /// <remarks>
    /// http://msdn.microsoft.com/en-us/magazine/dd419663.aspx#id0090030
    /// </remarks>
    public class RelayCommand : ICommand 
    { 
        #region Fields 
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute; 
        #endregion 
            
        #region Constructors 
        public RelayCommand(Action<object> execute) : this(execute, null) { } 
            
        public RelayCommand(Action<object> execute, Predicate<object> canExecute) 
        { 
            if (execute == null) throw new ArgumentNullException(nameof(execute)); 
            _execute = execute; 
            _canExecute = canExecute; 
        } 
        #endregion 
        // Constructors 
            
        #region ICommand Members 
        [DebuggerStepThrough] 
        public bool CanExecute(object parameter) 
        { 
            return _canExecute?.Invoke(parameter) ?? true; 
        } 
            
        public event EventHandler CanExecuteChanged 
        { 
            add 
            { 
                CommandManager.RequerySuggested += value; 
            } 
            remove 
            { 
                CommandManager.RequerySuggested -= value; 
            } 
        } 
            
        public void Execute(object parameter) 
        { 
            _execute(parameter); 
        } 
        #endregion 
    }
}