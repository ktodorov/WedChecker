using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Business.Models.Enums;
using WedChecker.Common;
using WedChecker.UserControls.Tasks;

namespace WedChecker.Interfaces
{
    public interface IUpdateableTasks
    {
        TaskCategories TasksCategory { get; }

        void UpdateTasks(List<BaseTaskControl> controls);
    }
}
