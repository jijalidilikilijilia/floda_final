using Aspose.Words;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;

namespace floda_final
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private string filepath = string.Empty;
        private string folderpath = string.Empty;
        FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
        private const string Lang = "rus";
        private bool is_folder_used = false;
        private bool is_pdf_used = false;
        Tesseract tes = new Tesseract(Environment.CurrentDirectory, Lang, OcrEngineMode.TesseractLstmCombined);


        // Кнопка выбора картинки
        private void button1_Click(object sender, EventArgs e)
        {
            is_folder_used = false;
            DialogResult dr = openFileDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                filepath = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(filepath);
            }
            else
            {
                MessageBox.Show("Картинка не выбрана");
            }
        }


        // Кнопка выбора папки
        private void button2_Click(object sender, EventArgs e)
        {
            is_folder_used = true;
            DialogResult dr = folderBrowserDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                folderpath = folderBrowserDialog.SelectedPath;
            }
            else
            {
                MessageBox.Show("Папка не выбрана");
            }
        }

        private void TesseractFindTextOnPicture(string file_path)
        {
            label1.Text = file_path;
            try
            {
                if (String.IsNullOrEmpty(file_path) || String.IsNullOrWhiteSpace(filepath))
                {
                    throw new Exception("Картинка не выбрана");
                }
                else
                {
                    tes.SetImage(new Image<Bgr, byte>(filepath));
                    tes.Recognize();
                    string text = tes.GetUTF8Text().ToLower();

                    string anal_res = analys_res(text);

                    richTextBox1.Text = text;
                    label_result.Text = anal_res;
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TesseractFindTextInFolder(string folder_path)
        {
            try
            {
                foreach (var file in Directory.EnumerateFiles(folder_path, "*", SearchOption.AllDirectories))
                {
                    Console.WriteLine("aboba " + file);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (is_folder_used)
                TesseractFindTextInFolder(folderpath);
            else if (is_pdf_used)
                TesseractFromPDF(filepath);
            else
                TesseractFindTextOnPicture(filepath);
        }

        private string analys_res(string strsosat)
        {
            string govno = strsosat.ToLower();

            if (govno.Contains("счёт") && govno.Contains("фактура") || govno.Contains("исправление"))
                return "Счет-фактура";
            else if (govno.Contains("cчет на оплату") || govno.Contains("счёт на оплату") || govno.Contains("счёт") || govno.Contains("счет"))
                return "Cчет на оплату";

            return "Попуск";
        }

        //pdf
        private void button4_Click(object sender, EventArgs e)
        {
            is_pdf_used = true;
            is_folder_used = false;
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                filepath = openFileDialog1.FileName;
            }
        }

        private void TesseractFromPDF(string file_path)
        {
            var doc = new Document(filepath);

            for (int page = 0; page < doc.PageCount; page++)
            {
                var extractedPage = doc.ExtractPages(page, 1);
                extractedPage.Save($"{filepath.Remove(filepath.Length - 4)}.png");
            }

            pictureBox1.Image = Image.FromFile($"{filepath.Remove(filepath.Length - 4)}.png");
            tes.SetImage(new Image<Bgr, byte>(filepath.Remove(filepath.Length - 4) + ".png"));
            tes.Recognize();
            richTextBox1.Text = tes.GetUTF8Text();
            string a = tes.GetUTF8Text();

            string res = analys_res(a);

            label_result.Text = res; 
            tes.Dispose();
        }
    }
}