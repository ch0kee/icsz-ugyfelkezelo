using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ugyfelkezelo.ViewModel
{
    public class CommandWithEvent : ICommand
    {
        /// <summary>
        /// Fire this event, if command executed
        /// </summary>
        public event EventHandler Event;


        public CommandWithEvent()
        {

        }


        public event EventHandler CanExecuteChanged;
        public Boolean CanExecute(Object parameter)
        {
            return true;
        }

        public void Execute(Object parameter)
        {
            if (Event != null)
                Event(this, EventArgs.Empty);
        }


    }
}
