using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyNotes.ViewModel.Commands
{
    public class EditCommandNote : ICommand
    {
        public event EventHandler? CanExecuteChanged;
        public NotesVM ViewModel { get; set; }

        public EditCommandNote(NotesVM vm)
        {
            ViewModel = vm;
        }
        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            ViewModel.StartEditingNotes();
        }
    }
}
