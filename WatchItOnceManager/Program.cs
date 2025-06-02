using CommandLine;
using System;
using System.Linq;
using WatchItOnce.Core;

namespace WatchItOnce.Manager
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .MapResult(
                    (Options opts) => Execute(opts),
                    errs => 1);
        }

        private static int Execute(Options opts)
        {
            switch (opts.Mode)
            {
                case PlayMode.Default:
                    PlayDefault(opts);
                    break;
                case PlayMode.Quad:
                    PlayQuadFiles(opts);
                    break;
            }
            return 0;
        }

        private static void PlayDefault(Options opts)
        {
            StartWithArgs(GetDefaultArguments(opts));
        }

        private static void PlayQuadFiles(Options opts)
        {
            var files = MediaFileProvider.GetFromFolder(System.IO.Directory.GetCurrentDirectory(), new NoFilter(), opts.Extensions.Split(new char[] { ';' }))
                .Take(4)
                .ToArray();
            var commonArgs = GetDefaultArguments(opts);
            for (int i = 0; i < files.Length; ++i)
            {
                var file = files[i];
                StartWithArgs(commonArgs + " --file \"" + file.Path + "\" --screen-position " + (ScreenPosition)(i + 1));
            }
        }

        private static void StartWithArgs(string args)
        {
            Console.WriteLine("Starting with " + args);
            System.Diagnostics.Process.Start("wio.exe", args);
        }

        private static string GetDefaultArguments(Options opts)
        {
            var args = "--volume " + opts.Volume
                + " --speed " + opts.Speed
                + " --extensions " + opts.Extensions;
            if (opts.DeleteAfterWatch)
            {
                args += " --delete";
            }
            return args;
        }
    }
}
