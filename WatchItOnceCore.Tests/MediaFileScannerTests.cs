namespace WatchItOnce.Core.Tests
{
    class FileListProviderMock : IFileListProvider
    {
        public List<string> Directories = new ();
        public string[] GetDirectories(string path)
        {
            return Directories.ToArray();
        }

        public Dictionary<string, List<string>> Files = new();
        public string[] GetFiles(string path, string extension)
        {
            List<string> files = new ();
            foreach (var file in Files[path])
            {
                if (Path.GetExtension(file) == extension)
                {
                    files.Add(file);
                }
            }
            return files.ToArray();
        }
    }

    [TestClass]
    public class MediaFileScannerTests
    {
        [TestMethod]
        public void FilterDuplicateExtensions()
        {
            var filesProvider = new FileListProviderMock();
            var testFiles = new List<string>();
            testFiles.Add("1.mp4");
            filesProvider.Files["test"] = testFiles;
            var scanner = new MediaFileScanner(null, new string[] { ".mp4", ".mp4" }, filesProvider);
            scanner.ScannFolder("test");
            Assert.AreEqual(1, scanner.Files.Count);
            Assert.AreEqual("1.mp4", scanner.Files[0].Path);
        }
    }
}