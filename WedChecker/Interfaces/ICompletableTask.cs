using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WedChecker.Interfaces
{
    public interface ICompletableTask
    {
        int GetCompletedItems();
        int GetUncompletedItems();
    }
}
