﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchItOnce
{
    public interface IFileFilter
    {
        bool IsPassing(string file);
    }
}
