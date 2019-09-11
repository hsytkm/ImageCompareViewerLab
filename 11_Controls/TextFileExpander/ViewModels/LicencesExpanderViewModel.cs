using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TextFileExpander.ViewModels
{
    class LicencesExpanderViewModel : BindableBase
    {
        public IReadOnlyCollection<LicenseViewModel> Licenses { get; }

        public LicencesExpanderViewModel()
        {
            var assembly = Assembly.GetExecutingAssembly();

            // Licensesディレクトリ以下の埋め込みリソースを読み込む
            var tasks = assembly.GetManifestResourceNames()
                .Where(x => x.Contains(".Licenses."))
                .Select(x => CreateLicenseViewModelAsync(x));

            var licenses = Task.WhenAll(tasks);
            licenses.Wait();     // ctorなので…
            Licenses = licenses.Result;

            // ViewModel作成Task
            async Task<LicenseViewModel> CreateLicenseViewModelAsync(string resourceFullName)
            {
                using (var sr = new StreamReader(assembly.GetManifestResourceStream(resourceFullName)))
                {
                    // 拡張子を含まないファイル名(namespace.Directory.ResourceName.Extension)
                    var names = resourceFullName.Split('.');
                    var name = names[names.Length - 2];
                    var content = await sr.ReadToEndAsync();
                    return new LicenseViewModel(name, content);
                }
            }
        }
    }

    class LicenseViewModel
    {
        public string Name { get; }

        public string Content { get; }

        public LicenseViewModel(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}
