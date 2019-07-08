<<<<<<< HEAD
﻿namespace BangazonWorkforce.Models
{
    public class Department
    {
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models
{
    public class Department
    {
        public string Name { get; set; }
        public int Budget { get; set; }
        public List<Employee> employees { get; set; }
    }
}
>>>>>>> master
