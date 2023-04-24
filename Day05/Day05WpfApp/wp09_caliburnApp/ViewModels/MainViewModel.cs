using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using wp09_caliburnApp.Models;

namespace wp09_caliburnApp.ViewModels
{
    public class MainViewModel : Screen
    {
        // Caliburn version업으로 변경
        private string firstName = "JiHyeon";
        public string FirstName {
            get => firstName; 
            set
            {
                firstName = value;
                NotifyOfPropertyChange(() => FirstName); // 속성값이 변경된걸 시스템에 알려주는 역할
                NotifyOfPropertyChange(nameof(FullName));
                NotifyOfPropertyChange(nameof(CanClearName));
            }
        }

        private string lastName = "Kim";
        public string LastName
        {
            get => lastName;
            set
            {
                lastName = value;
                NotifyOfPropertyChange(() => LastName);
                NotifyOfPropertyChange(() => FullName); // 변화 통보
                NotifyOfPropertyChange(nameof(CanClearName));
            }
        }

        public string FullName
        {
            get => $"{LastName} {FirstName}";
        }

        // 콤보박스에 바인딩 할 속성
        // 멤버변수로 var 사용 불가
        private BindableCollection<Person> managers = new BindableCollection<Person>();

        public BindableCollection<Person> Managers
        {
            get => managers;
            set => managers = value;
        }

        private Person selectedManager;

        public Person SelectedManager
        {
            get => selectedManager;
            set
            {
                selectedManager = value;
                LastName = SelectedManager.LastName;
                FirstName = SelectedManager.FirstName;
                NotifyOfPropertyChange(nameof(SelectedManager));
            }
        }

        public MainViewModel()
        {
            //DB를 사용하면 여기서 DB접속 > 데이터 select까지
            Managers.Add(new Person { FirstName = "John", LastName = "Carmack" });
            Managers.Add(new Person { FirstName = "Steve", LastName = "Jobs" });
            Managers.Add(new Person { FirstName = "Bill", LastName = "Gates" });
            Managers.Add(new Person { FirstName = "Elon", LastName = "Musk" });
        }

        // private, internal void ClearName() 할 경우 초기화 안됨
        public void ClearName()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
        }

        // 메서드와 이름 동일하게 하고 앞에 Can 붙임
        public bool CanClearName
        {
            get => !(string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName));
        }

    }
}
