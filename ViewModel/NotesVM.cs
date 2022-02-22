using MyNotes.Model;
using MyNotes.ViewModel.Commands;
using MyNotes.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static MyNotes.Model.HasId;

using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;


namespace MyNotes.ViewModel
{
    public class NotesVM : INotifyPropertyChanged
    {
        public ObservableCollection<Notebook> Notebooks { get; set; }
        public ObservableCollection<Note> Notes { get; set; }

        private Notebook selectedNotebook;

        public Notebook SelectedNotebook
        {
            get { return selectedNotebook; }
            set
            {
                selectedNotebook = value;
                OnpPropertyChanged("SelectedNotebook");
                GetNotes();
            }
        }

        private Note selectedNote;

        public Note SelectedNote
        {
            get { return selectedNote; }
            set 
            { 
                selectedNote = value;
                OnpPropertyChanged("SelectedNote");
                SelectedNoteChanged.Invoke(this, new EventArgs());
            }
        }

        private Visibility isVisible;
        public Visibility IsVisible
        {
            get
            {
                return isVisible;
            }
            set
            {
                isVisible = value;
                OnpPropertyChanged("IsVisible");
            }
        }
        private Visibility isVisiblee;
        public Visibility IsVisiblee
        {
            get
            {
                return isVisiblee;
            }
            set
            {
                isVisiblee = value;
                OnpPropertyChanged("IsVisiblee");
            }
        }
        public NewNotebookCommand NewNotebookCommand { get; set; }
        public EditCommand EditCommand { get; set; }
        public EndEditingCommand EndEditingCommand { get; set; }

        public NewNoteCommand NewNoteCommand { get; set; }
        public EndEditingCommandNote EndEditingCommandNote { get; set; }
        public EditCommandNote EditCommandNote { get; set; }
        public EndDeleteCommand EndDeleteCommand { get; set; }
        public EndDeleteCommandNote EndDeleteCommandNote { get; set; }


        public event PropertyChangedEventHandler? PropertyChanged;

        public event EventHandler SelectedNoteChanged;

        public NotesVM()
        {
            NewNotebookCommand = new NewNotebookCommand(this);
            NewNoteCommand = new NewNoteCommand(this);

            EditCommand = new EditCommand(this);
            EndEditingCommand = new EndEditingCommand(this);
            EndDeleteCommand = new EndDeleteCommand(this);
            IsVisible = Visibility.Collapsed;

            EditCommandNote = new EditCommandNote(this);
            EndEditingCommandNote = new EndEditingCommandNote(this);
            EndDeleteCommandNote = new EndDeleteCommandNote(this);
            IsVisiblee = Visibility.Collapsed;
            

            Notebooks = new ObservableCollection<Notebook>();
            Notes = new ObservableCollection<Note>();

            GetNotebooks();
        }

        public async void CreateNotebook()
        {
            Notebook newNotebook = new Notebook()
            {
                Name = "Notebook",
                UserId = App.UserId
            };  
            await DatabaseHelper.Insert(newNotebook);
            GetNotebooks();

        }

        public async void CreateNote(string notebookId)
        {
            Note newNote = new Note
            {
                NotebookId = notebookId,
                CreatedTime = DateTime.Now,
                UpdatedTime = DateTime.Now,
                Title = $"Note created at {DateTime.Now.ToString()}",
            };
            await DatabaseHelper.Insert(newNote);
            GetNotes();
        }
       public async void GetNotebooks()
        {
            try
            {
                var notebooks = (await DatabaseHelper.Read<Notebook>()).Where(n => n.UserId == App.UserId).ToList();

                Notebooks.Clear();
                foreach (var notebook in notebooks)
                {
                    Notebooks.Add(notebook);
                }
            }
            catch (Exception ex)
            { }
        }

        private async void GetNotes()
        {
            if (SelectedNotebook != null)
            {
                try
                {
                    var notes = (await DatabaseHelper.Read<Note>()).Where(n => n.NotebookId == SelectedNotebook.Id).ToList();

                    Notes.Clear();
                    foreach (var note in notes)
                    {
                        Notes.Add(note);
                    }
                }
                catch (Exception ex)
                { }
            }
        }

        private void OnpPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void StartEditing()
        {
            IsVisible = Visibility.Visible;
        }

        public void StopEditing(Notebook notebook)
        {
            IsVisible = Visibility.Collapsed;
            DatabaseHelper.Update(notebook);
            GetNotebooks();
        }
        public void DeleteNotebooks(Notebook notebooks)
        {
            try
            {
                DatabaseHelper.Delete(notebooks);
                DatabaseHelper.Update(notebooks);
                GetNotebooks();
            }

            catch (Exception ex)
            { }
        }
        public void StartEditingNotes()
        {
            IsVisiblee = Visibility.Visible;
        }

        public void StopEditingNotes(Note notes)
        {
            IsVisiblee = Visibility.Collapsed;
            DatabaseHelper.Update(notes);
            GetNotes();
        }

        public void DeleteNotes(Note notes)
        {
            try
            {
                DatabaseHelper.Delete(notes);
                DatabaseHelper.Update(notes);
                GetNotes();
            }

            catch (Exception ex)
            { }
        }
    }
}
