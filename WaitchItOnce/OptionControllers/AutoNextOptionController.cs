using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchItOnce
{
    public partial class Options
    {
        /// <summary>
        /// Option controller with switches play to the next video after certan period of time
        /// </summary>
        class AutoNextOptionController : IOptionController
        {
            public AutoNextOptionController(Options options)
            {
                mOptions = options;
            }
            Options mOptions;

            public void ResetToDefault()
            {
                mOptions.AutoNext = null;
            }

            public bool Parse(string optionName, StringIterator strings)
            {
                if (optionName != "--autonext")
                    return false;

                string val = strings.GetNext();
                if (val == null)
                    throw new ArgumentException("--autonext should be followed by the interval in seconds");

                if (!int.TryParse(val, out int interval))
                    throw new ArgumentException("Can't part number of seconds for the --autonext parameter");

                mOptions.AutoNext = interval;

                return true;
            }
        }
    }
}
