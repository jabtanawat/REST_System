using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace REST.Service
{
    public class Comp
    {
        public class FormMode
        {
            public const string ADD = "ADD";
            public const string EDIT = "EDIT";
            public const string SEARCH = "SEARCH";
            public const string DELETE = "DELETE";
        }
    }
}
