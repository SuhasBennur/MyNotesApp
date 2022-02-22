using MyNotes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyNotes.ViewModel.Commands
{
    public class EndDeleteCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged;

        public NotesVM ViewModel { get; set; }

        public EndDeleteCommand(NotesVM vm)
        {
            ViewModel = vm;
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Notebook notebooks = parameter as Notebook;
            if (notebooks != null)
            {
                ViewModel.DeleteNotebooks(notebooks);
            }
        }
    }
}
