//---------------------------------------
// Person.cs (c) 2006 by Charles Petzold
//---------------------------------------
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Petzold.SingleRecordDataEntry
{
    public class Person: INotifyPropertyChanged
    {
        // PropertyChanged определение события.
        public event PropertyChangedEventHandler PropertyChanged;

        // Личные поля.
        string strFirstName = "<first name>";
        string strMiddleName = "" ;
        string strLastName = "<last name>";
        DateTime? dtBirthDate = new DateTime(1800, 1, 1);
        DateTime? dtDeathDate = new DateTime(1900, 12, 31);

        // Открытые свойства.
        public string FirstName
        {
            set 
            { 
                strFirstName = value; 
                OnPropertyChanged("FirstName");
            }
            get { return strFirstName; }
        }
        public string MiddleName
        {
            set 
            { 
                strMiddleName = value; 
                OnPropertyChanged("MiddleName");
            }
            get { return strMiddleName; }
        }
        public string LastName
        {
            set 
            { 
                strLastName = value; 
                OnPropertyChanged("LastName");
            }
            get { return strLastName; }
        }

        [XmlElement(DataType="date")]
        public DateTime? BirthDate
        {
            set 
            { 
                dtBirthDate = value; 
                OnPropertyChanged("BirthDate");
            }
            get { return dtBirthDate; }
        }

        [XmlElement(DataType = "date")]
        public DateTime? DeathDate
        {
            set 
            { 
                dtDeathDate = value; 
                OnPropertyChanged("DeathDate");
            }
            get { return dtDeathDate; }
        }

        // Запускаем событие PropertyChanged.
        protected virtual void OnPropertyChanged(string strPropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, 
                    new PropertyChangedEventArgs(strPropertyName));
        }
    }
}
