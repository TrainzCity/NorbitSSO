using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApp.Model
{
    public partial class User
    {
        public Guid uuid { get; set; }

        public string surname { get; set; } = null!;

        public string name { get; set; } = null!;

        public string? patronymic { get; set; }

        public DateOnly birthday { get; set; }

        public string? email { get; set; }

        public string phone { get; set; } = null!;

        public string login { get; set; } = null!;

        public byte[] password { get; set; } = null!;

        public bool IsBlocked { get; set; }
        public string FullName => $"{surname} {name} {patronymic}";
        public int Age { get
            {
                DateTime now = DateTime.Today;
                int age = now.Year - birthday.Year;
                if (birthday.ToDateTime(new TimeOnly()) > now.AddYears(-age)) age--;
                return age;
            } }
    }
}
