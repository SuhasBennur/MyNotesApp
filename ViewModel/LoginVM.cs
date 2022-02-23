using MyNotes.Model;
using MyNotes.ViewModel.Commands;
using MyNotes.ViewModel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MyNotes.View;

namespace MyNotes.ViewModel
{
    public class LoginVM : INotifyPropertyChanged 
    {
        private bool isShowingRegister = false;

        private User user;
        public User User
        {
            get { return user; }
            set 
            { 
                user = value;
                OnPropertyChanged("User");
            }
        }
        private string username;

        public string Username
        {
            get { return username; }
            set
            { 
                username = value;
                User = new User
                {
                    Username = username,
                    Password = this.Password,
                    Firstname = this.Firstname,
                    Lastname = this.Lastname,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged("Username");
            }
        }

        private string password;

        public string Password
        {
            get { return password; }
            set
            {
                password = value;
                User = new User
                {
                    Username = this.Username,
                    Password = password,
                    Firstname = this.Firstname,
                    Lastname = this.Lastname,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged("Password");
            }
        }



        private string firstname;
        public string Firstname
        {
            get { return firstname; }
            set
            {
                firstname = value;
                User = new User
                {
                    Username = this.Username,
                    Password = this.Password,
                    Firstname = firstname,
                    Lastname = this.Lastname,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged("Firstname");
            }
        }

        private string lastname;
        public string Lastname
        {
            get { return lastname; }
            set
            {
                lastname = value;
                User = new User
                {
                    Username = this.Username,
                    Password = this.Password,
                    Firstname = this.Firstname,
                    Lastname = lastname,
                    ConfirmPassword = this.ConfirmPassword
                };
                OnPropertyChanged("Lastname");
            }
        }

        private string confirmPassword;
        public string ConfirmPassword
        {
            get { return confirmPassword; }
            set
            {
                confirmPassword = value;
                User = new User
                {
                    Username = this.Username,
                    Password = this.Password,
                    Firstname = this.Firstname,
                    Lastname = this.Lastname,
                    ConfirmPassword = confirmPassword
                };
                OnPropertyChanged("ConfirmPassword");
            }
        }

        private Visibility loginVis;

        public Visibility LoginVis
        {
            get { return loginVis; }
            set 
            { 
                loginVis = value;
                OnPropertyChanged("LoginVis");
            }
        }

        private Visibility registerVis;

        public Visibility RegisterVis
        {
            get { return registerVis; }
            set
            {
                registerVis = value;
                OnPropertyChanged("RegisterVis");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler Authenticated;
        

        public RegisterCommand RegisterCommand { get; set; }
        public LoginCommand LoginCommand { get; set; }
        public ShowRegisterCommand ShowRegisterCommand { get; set; }

        public LoginVM()
        {
            LoginVis = Visibility.Visible;
            RegisterVis = Visibility.Collapsed;

            RegisterCommand = new RegisterCommand(this);
            LoginCommand = new LoginCommand(this);

            ShowRegisterCommand = new ShowRegisterCommand(this);

            User = new User();
        }
        public void SwitchViews()
        {
            isShowingRegister = !isShowingRegister;
            if(isShowingRegister)
            {
                RegisterVis = Visibility.Visible;
                LoginVis = Visibility.Collapsed;
            }
            else
            {
                RegisterVis = Visibility.Collapsed;
                LoginVis = Visibility.Visible;
            }
        }
        public async void Login()
        {
            bool result=await FireBaseAuthHelper.Login(User);
            if(result)
            {
                NotesWindow starts = new NotesWindow();
                Authenticated?.Invoke(this, new EventArgs());
                starts.ShowDialog();
            }
        }
        public async void Register()
        {
           bool result=await FireBaseAuthHelper.Register(User);
            if (result)
            {
                LoginWindow s=new LoginWindow();
                Authenticated?.Invoke(this, new EventArgs());
                MessageBox.Show("Registered Successfully.");
                s.ShowDialog();
            }
        }
        private void OnPropertyChanged(string propertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
