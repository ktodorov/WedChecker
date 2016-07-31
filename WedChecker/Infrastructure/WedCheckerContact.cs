using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Contacts;

namespace WedChecker.Infrastructure
{
    public class WedCheckerContact : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Contact storedContact;

        #region Contact Properties

        private string _id;
        public string Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value == _id)
                {
                    return;
                }

                _id = value;
                storedContact.Id = value;
                NotifyPropertyChanged("Id");
            }
        }

        private string _firstName;
        public string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                if (value == _firstName)
                {
                    return;
                }

                _firstName = value;
                storedContact.FirstName = value;
                FullName = $"{FirstName} {LastName}";
                NotifyPropertyChanged("FirstName");
                NotifyPropertyChanged("FullName");
            }
        }

        private string _lastName;
        public string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if (value == _lastName)
                {
                    return;
                }

                _lastName = value;
                storedContact.LastName = value;

                FullName = $"{FirstName} {LastName}";
                NotifyPropertyChanged("LastName");
                NotifyPropertyChanged("FullName");
            }
        }

        private string _fullName;
        public string FullName
        {
            get
            {
                return _fullName;
            }
            set
            {
                if (value == _fullName)
                {
                    return;
                }

                _fullName = value;

                var names = value.Split(' ');
                var lastName = names.LastOrDefault();
                var firstName = string.Empty;

                for (int i = 0; i < names.Length - 1; i++)
                {
                    firstName += names[i];
                }

                FirstName = firstName;
                LastName = lastName;
                storedContact.FirstName = firstName;
                storedContact.LastName = lastName;
                NotifyPropertyChanged("FullName");
            }
        }

        private IList<ContactPhone> _phones;
        public IList<ContactPhone> Phones
        {
            get
            {
                if (_phones == null)
                {
                    _phones = new List<ContactPhone>();
                }
                return _phones;
            }
            set
            {
                _phones = value;
                storedContact.Phones.Clear();

                foreach (var phone in Phones)
                {
                    storedContact.Phones.Add(phone);
                }

                NotifyPropertyChanged("Phones");
            }
        }

        public ContactPhone Phone
        {
            get
            {
                return Phones.FirstOrDefault();
            }
            set
            {
                Phones?.Clear();
                Phones?.Add(value);

                storedContact.Phones.Clear();
                storedContact.Phones.Add(value);
                NotifyPropertyChanged("Phone");
            }
        }

        public string PhoneNumber
        {
            get
            {
                return Phone?.Number;
            }
            set
            {
                if (Phone == null)
                {
                    Phone = new ContactPhone();
                }

                Phone.Number = value;
                NotifyPropertyChanged("PhoneNumber");
            }
        }

        private IList<ContactEmail> _emails;
        public IList<ContactEmail> Emails
        {
            get
            {
                if (_emails == null)
                {
                    _emails = new List<ContactEmail>();
                }
                return _emails;
            }
            set
            {
                _emails = value;
                storedContact.Emails.Clear();

                foreach (var email in Emails)
                {
                    storedContact.Emails.Add(email);
                }

                NotifyPropertyChanged("Emails");
            }
        }

        public ContactEmail Email
        {
            get
            {
                return Emails.FirstOrDefault();
            }
            set
            {
                Emails?.Clear();
                Emails?.Add(value);

                storedContact.Emails.Clear();
                storedContact.Emails.Add(value);
                EmailAddress = Email?.Address;
                NotifyPropertyChanged("Email");
            }
        }

        public string EmailAddress
        {
            get
            {
                return Email?.Address;
            }
            set
            {
                if (Email == null)
                {
                    Email = new ContactEmail();
                }

                Email.Address = value;
                NotifyPropertyChanged("EmailAddress");
            }
        }



        private string _notes;
        public string Notes
        {
            get
            {
                return _notes;
            }
            set
            {
                if (value == _notes)
                {
                    return;
                }

                _notes = value;
                storedContact.Notes = value;
                NotifyPropertyChanged("Notes");
            }
        }

        #endregion

        private void NotifyPropertyChanged(String info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

        public WedCheckerContact()
        {
            storedContact = new Contact();
        }

        public WedCheckerContact(Contact contact)
        {
            storedContact = new Contact();
            SetContact(contact);
        }

        public void SetContact(Contact contact)
        {
            Id = contact.Id;
            FirstName = contact.FirstName;
            LastName = contact.LastName;
            FullName = $"{FirstName} {LastName}";
            Phones = contact.Phones;
            Emails = contact.Emails;
            Notes = contact.Notes;

            storedContact = contact;
        }

        public Contact ToContact()
        {
            return storedContact;
        }
    }
}
