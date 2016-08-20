using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WedChecker.Interfaces
{
    public interface IStorableTask
    {
        void DisplayValues();

        void EditValues();

        void Serialize(BinaryWriter writer);

        Task Deserialize(BinaryReader reader);

        string GetDataAsText();
    }
}
