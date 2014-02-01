using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatchItOnce
{
    public partial class Options
    {
        interface IOptionController
        {
            /// <summary>
            /// Resets option to the defailt value
            /// </summary>
            void ResetToDefault();

            /// <summary>
            /// Parses option
            /// </summary>
            /// <param name="strings"></param>
            /// <exception cref="ArgumentException">Thrown in case of bad option parameters.</exception>
            /// <returns>true - the option has been parsed. false - the option is not supported.</returns>
            bool Parse(string optionName, StringIterator strings);
        }
    }
}
