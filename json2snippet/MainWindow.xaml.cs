using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using Path = System.IO.Path;
using System.Text.RegularExpressions;
using MessageBox = System.Windows.MessageBox;

namespace json2snippet
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnClickBrowseSrcJson(object sender, RoutedEventArgs e)
        {
            var filePath = string.Empty;

            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "c:\\";
                openFileDialog.Filter = "json files (*.json)|*.json";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    //Get the path of specified file
                    filePath = openFileDialog.FileName;
                    tb_src.Text = filePath;
                }
            }
        }

        private void OnClickBrowseTargetDir(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Set the output dir";
                if( !string.IsNullOrEmpty(tb_src.Text) )
                {
                    dlg.SelectedPath = System.IO.Path.GetDirectoryName(tb_src.Text);
                }
                if( dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK )
                {
                    var path = dlg.SelectedPath;
                    tb_target.Text = path;
                }
            }
        }

        private void OnClickConvert(object sender, RoutedEventArgs e)
        {
            try
            {
                //Read the contents of the file into a stream
                var fileStream = File.OpenRead(tb_src.Text);

                using (StreamReader reader = new StreamReader(fileStream))
                {
                    var fileContent = reader.ReadToEnd();

                    JsonSerializerSettings settings = new JsonSerializerSettings();
                    settings.MetadataPropertyHandling = MetadataPropertyHandling.Ignore;
                    var allSnippets = JsonConvert.DeserializeObject<Dictionary<string, VSCodeSnippet>>(fileContent, settings);

                    //Console.WriteLine($"found snippets = {allSnippets.Count}");

                    foreach (var pr in allSnippets)
                    {
                        var name = pr.Key;
                        var filepath = Path.ChangeExtension(Path.Combine(tb_target.Text, name), ".snippet");
                        var aSnippet = pr.Value;
                        var tpl = new VisualStudioSnippet();
                        tpl._name = name;
                        tpl._description = aSnippet.description ?? "No Description";
                        tpl._shortcut = _ProcessShortcut(aSnippet.prefix);
                        tpl._vars = _ParseVars(aSnippet.body);
                        tpl._lines = _ParseLines(aSnippet.body);
                        var outstring = tpl.TransformText();

                        File.WriteAllText(filepath, outstring);
                    }

                    MessageBox.Show($"Generate {allSnippets.Count} snippet files");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
        }

        private string _ProcessShortcut(string prefix)
        {
            if( prefix.StartsWith("$") )
            {
                prefix = "x"+prefix.Substring(1);
            }
            return prefix;
        }

        private const string RE_NUM = @"\$([0-9]+)";
        private const string RE_NUM1 = @"\$\{([0-9]+)\}";
        private const string RE_NUM2 = @"\$\{([0-9]+):[_a-zA-Z][_a-zA-Z0-9]*\}";
        private const string RE_VAR = @"\$([_a-zA-Z][_a-zA-Z0-9]*)";
        private const string RE_VAR1 = @"\$\{([_a-zA-Z][_a-zA-Z0-9]*)\}";
        private const string RE_VAR2 = @"\$\{([_a-zA-Z][_a-zA-Z0-9]*):[_a-zA-Z][_a-zA-Z0-9]*\}";
        private static readonly string RE_ALL = string.Join("|", RE_NUM, RE_NUM1, RE_NUM2, RE_VAR, RE_VAR1, RE_VAR2);
        private static readonly Regex _re = new Regex(RE_ALL, RegexOptions.Compiled);

        private List<string> _ParseLines(List<string> body)
        {
            var newLines = new List<string>();

            foreach(var aline in body)
            {
                newLines.Add(_re.Replace(aline, _ReplaceStringVar) + "\n");
            }

            return newLines;
        }

        private string _ReplaceStringVar(Match match)
        {
            for(int i=1; i<match.Groups.Count; ++i)
            {
                var grp = match.Groups[i];
                var varName = _ProcessVarName(grp.Value);
                if( grp.Success )
                {
                    return $"${varName}$";
                }
            }
            return "__ERROR__";
        }

        private List<Var> _ParseVars(List<string> body)
        {
            var varDict = new Dictionary<string, Var>();

            foreach(var aline in body)
            {
                foreach (Match match in _re.Matches(aline))
                {
                    for(int i=1; i<match.Groups.Count; ++i) //skip the first group(whole)
                    {
                        var grp = match.Groups[i];
                        if (grp.Success)
                        {
                            string varName = _ProcessVarName(grp.Value);
                            varDict[varName] = Var.Create(varName);
                        }
                    }
                }
            }

            var varLst = varDict.Values.ToList();
            return varLst;
        }

        /// <summary>
        /// rename a pure number to a valid varname
        /// </summary>
        private string _ProcessVarName(string varName)
        {
            int v = 0;
            if( int.TryParse(varName, out v) )
            {
                varName = "_var"+varName+"_";
            }
            return varName;
        }

        private void OnSrcTextChanged(object sender, TextChangedEventArgs e)
        {
            _UpdateButtonState();
        }

        private void OnTargetSrcChanged(object sender, TextChangedEventArgs e)
        {
            _UpdateButtonState();
        }

        /// <summary>
        /// check if the src file and target dir are valid, update buttonConvert.enabled accordingly
        /// </summary>
        private void _UpdateButtonState()
        {
            var srcPath = tb_src.Text;
            var targetDir = tb_target.Text;

            buttonConvert.IsEnabled = File.Exists(srcPath) && Directory.Exists(targetDir);
        }
    }
}


