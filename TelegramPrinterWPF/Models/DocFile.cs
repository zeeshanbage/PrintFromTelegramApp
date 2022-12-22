﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramPrinterWPF.Models
{
    public class DocFile
    {
        public DocFile(string path)
        {
            Path= path;
            Name= path.Split('/').Last();
            Type= Name.Split('.').Last();
        }
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
    }
}