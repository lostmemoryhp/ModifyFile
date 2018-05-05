using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModifyFile
{
    public interface IFileModifier
    {
        bool canModify(FileInfo file);
        void modify(FileInfo file);
        void recovery(FileInfo file);
    }
}
