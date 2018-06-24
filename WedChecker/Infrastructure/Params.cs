using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Business.Models.Enums;
using WedChecker.Common;

namespace WedChecker.Infrastructure
{
    public class Params
    {
        public MainPage CurrentPage { get; set; }
        public TaskCategories Category { get; set; }
    }
}
