using Caliburn.Micro;
using EDRouteOptimizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wpfEDRO.Models;

namespace wpfEDRO.ViewModels
{
    public class ShellViewModel : Screen
    {

        private string _firstName = "testing";
        private string _lastName;

        public ShellViewModel()
        {
            People.Add(new PersonModel { FirstName = "Harrison", LastName = "Brown" });
            People.Add(new PersonModel { FirstName = "John", LastName = "Smith" });
            People.Add(new PersonModel { FirstName = "David", LastName = "Braben" });
        }

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                NotifyOfPropertyChange(() => FirstName);
                NotifyOfPropertyChange(() => FullName);

            }
        }

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => FullName);
            }
        }

        public string FullName
        {
            get { return $"{FirstName} {LastName}"; }
        }

        private BindableCollection<PersonModel> _people = new BindableCollection<PersonModel>();

        public BindableCollection<PersonModel> People
        {
            get { return _people; }
            set { _people = value; }
        }

        private PersonModel _selectedPerson;

        public PersonModel SelectedPerson
        {
            get { return _selectedPerson; }
            set
            {
                _selectedPerson = value;
                NotifyOfPropertyChange(() => SelectedPerson);
            }
        }

        public bool CanClearText(string firstName, string lastName)
        {
            return 
                !string.IsNullOrWhiteSpace(firstName) || 
                !string.IsNullOrWhiteSpace(lastName);
        }

        public void ClearText(string firstName, string lastName)
        {
            FirstName = "";
            LastName = "";
        }
    }
}
