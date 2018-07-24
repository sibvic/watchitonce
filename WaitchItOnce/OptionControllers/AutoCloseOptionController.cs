
namespace WatchItOnce
{
    public partial class Options
    {
        class AutoCloseOptionController : IOptionController
        {
            public AutoCloseOptionController(Options options)
            {
                mOptions = options;
            }
            Options mOptions;

            public void ResetToDefault()
            {
                mOptions.AutoClose = null;
            }

            public bool Parse(string optionName, StringIterator strings)
            {
                if (optionName != "--tomato")
                    return false;

                mOptions.AutoClose = 25 * 60;

                return true;
            }
        }
    }
}
