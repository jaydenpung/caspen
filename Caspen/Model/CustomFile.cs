using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caspen.Model
{
    public class CustomFile
    {
        public string filePath;
        public string episode;

        public CustomFile(string filePath, string episode)
        {
            this.filePath = filePath;
            this.episode = episode;
        }
    }
}
