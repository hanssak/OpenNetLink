using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ViewTest
{
    public partial class Form1 : Form
    {
        readonly List<string> _tempFolders = new List<string>();

        public Form1()
        {
            InitializeComponent();
        }

        private void ViewerForm_Load(object sender, EventArgs e)
        {
            // ReSharper disable LocalizableElement
            //var version = Assembly.GetExecutingAssembly().GetName().Version;
            //Text = "Office Extractor test tool v" + version.Major + "." + version.Minor + "." + version.Build;
            // ReSharper restore LocalizableElement
            Closed += ViewerForm_Closed;
        }

        void ViewerForm_Closed(object sender, EventArgs e)
        {
            foreach (var tempFolder in _tempFolders)
            {
                //if (Directory.Exists(tempFolder))
                //    Directory.Delete(tempFolder, true);
            }
        }

        private void SelectButton_Click(object sender, EventArgs e)
        {
            /*var text = System.IO.File.ReadAllText("d:\\Test_with_2 _Excel_Objects.vsd");
            if (text.IndexOf("Excel") > 0)
                MessageBox.Show("Excel found");*/

            // Create an instance of the opeKn file dialog box.
            var openFileDialog1 = new OpenFileDialog
            {
                // ReSharper disable once LocalizableElement
                Filter = "Microsoft Office files|*.ODT;*.DOC;*.DOCM;*.DOCX;*.DOT;*.DOTM;*.DOTX;*.RTF;*.XLS;*.XLSB;*.XLSM;*.XLSX;*.XLT;" +
                                                     "*.XLTM;*.XLTX;*.XLW;*.POT;*.PPT;*.POTM;*.POTX;*.PPS;*.PPSM;*.PPSX;*.PPTM;*.PPTX;*.HWP;*.HWPX",
                FilterIndex = 1,
                Multiselect = false
            };

            // Process input if the user clicked OK.
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Open the selected file to read.
                string tempFolder = null;

                try
                {
                    //tempFolder = GetTemporaryFolder();
                    tempFolder = "C:\\Users\\smkoo\\Desktop\\DRMTEST";
                    _tempFolders.Add(tempFolder);

                    var extractor = new OfficeExtractor.Extractor();
                    var files = extractor.Extract(openFileDialog1.FileName, tempFolder);
                    FilesListBox.Items.Clear();

                    if (files == null) return;
                    foreach (var file in files)
                        FilesListBox.Items.Add(file);

                    GC.Collect();
                }
                catch (Exception ex)
                {
                    //if (tempFolder != null && Directory.Exists(tempFolder))
                    //    Directory.Delete(tempFolder, true);

                    MessageBox.Show(GetInnerException(ex));
                }
            }
        }

        #region GetTemporaryFolder
        private static string GetTemporaryFolder()
        {
            var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDirectory);
            return tempDirectory;
        }
        #endregion

        #region GetInnerException
        /// <summary>
        /// Get the complete inner exception tree
        /// </summary>
        /// <param name="e">The exception object</param>
        /// <returns></returns>
        private static string GetInnerException(Exception e)
        {
            var exception = e.Message + Environment.NewLine;
            if (e.InnerException != null)
                exception += GetInnerException(e.InnerException);
            return exception;
        }
        #endregion
    }
}
