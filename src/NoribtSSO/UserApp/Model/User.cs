using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserApp.Model
{
    public partial class User
    {
        public Guid Uuid { get; set; }

        public string Surname { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Patronymic { get; set; }

        public DateOnly Birthday { get; set; }

        public string? Email { get; set; }

        public string Phone { get; set; } = null!;

        public string Login { get; set; } = null!;

        public byte[] Password { get; set; } = null!;

        public bool IsBlocked { get; set; }
    }
}
