using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;

namespace ImageProcess
{
    public partial class Form1 : Form
    {

        //Dictionary will store the attachments
        Dictionary<int, byte[]> _myAttachments;
        Dictionary<int, string> _myAttachmentFileNames;
        Dictionary<string, Bitmap> _localInstance;
        Dictionary<string, Bitmap> _digit;
        Dictionary<string, string> _map;
        bool toggleValue = false;
        public Form1(Dictionary<string, Bitmap> instances, Dictionary<string, Bitmap> digit, Dictionary<string, string> map)
        {
            InitializeComponent();
            
            _myAttachments = new Dictionary<int, byte[]>();
            _myAttachmentFileNames = new Dictionary<int, string>();
            _localInstance = new Dictionary<string, Bitmap>();
            _digit = new Dictionary<string, Bitmap>();
            _localInstance = instances;
            _digit = digit;
            PopulateDataGridView();
            _map = map;
        }

        /// <summary>
        /// Populate the data
        /// </summary>
        

        public void PopulateDataGridView()
        {

            
 
            dataGridView1.ColumnCount = 14;

            dataGridView1.Columns[0].Name = "colNo";
            dataGridView1.Columns[1].Name = "colAttach";
            
            dataGridView1.Columns[2].Name = "cell11";
            dataGridView1.Columns[3].Name = "cell12";

            dataGridView1.Columns[4].Name = "cell21";
            dataGridView1.Columns[5].Name = "cell22";

            dataGridView1.Columns[6].Name = "cell31";
            dataGridView1.Columns[7].Name = "cell32";

            dataGridView1.Columns[8].Name = "cell41";
            dataGridView1.Columns[9].Name = "cell42";

            dataGridView1.Columns[10].Name = "cell51";
            dataGridView1.Columns[11].Name = "cell52";

            dataGridView1.Columns[12].Name = "cell61";
            dataGridView1.Columns[13].Name = "cell62";
            
            dataGridView1.Columns[0].HeaderText = "No.";
            dataGridView1.Columns[1].HeaderText = "Attachment Column";


            dataGridView1.Columns[2].HeaderText = "cell1.1";
            dataGridView1.Columns[3].HeaderText = "cell1.2";

            dataGridView1.Columns[4].HeaderText = "cell2.1";
            dataGridView1.Columns[5].HeaderText = "cell2.2";

            dataGridView1.Columns[6].HeaderText = "cell3.1";
            dataGridView1.Columns[7].HeaderText = "cell3.2";

            dataGridView1.Columns[8].HeaderText = "cell4.1";
            dataGridView1.Columns[9].HeaderText = "cell4.2";

            dataGridView1.Columns[10].HeaderText = "cell5.1";
            dataGridView1.Columns[11].HeaderText = "cell5.2";

            dataGridView1.Columns[12].HeaderText = "cell6.1";
            dataGridView1.Columns[13].HeaderText = "cell6.2";
            
            
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Width = 150;
            dataGridView1.Columns[2].Width = 40;
            dataGridView1.Columns[3].Width = 40;
            dataGridView1.Columns[4].Width = 40;
            dataGridView1.Columns[5].Width = 40;
            dataGridView1.Columns[6].Width = 40;
            dataGridView1.Columns[7].Width = 40;
            dataGridView1.Columns[8].Width = 40;

            dataGridView1.Columns[9].Width = 40;
            dataGridView1.Columns[10].Width = 40;
            dataGridView1.Columns[11].Width = 40;
            dataGridView1.Columns[12].Width = 40;
            dataGridView1.Columns[13].Width = 40;
            //dataGridView1.Rows.Add(new string[] { "1", "0.1", "1.1", "1.2", "2.1", "2.2", "3.1", "3.2", "4.1", "4.2", "5.1", "5.2", "6.1", "6.2" });
            string[] row = new string [] { "1", "0.1", "1.1", "1.2", "2.1", "2.2", "3.1", "3.2", "4.1", "4.2", "5.1", "5.2", "6.1", "6.2" };
            foreach (KeyValuePair<string, Bitmap> pair in _localInstance)
            {

                for (int cellNum = 0; cellNum < dataGridView1.ColumnCount; cellNum++)
                {
                    row[cellNum] = "null";
                }
                dataGridView1.Rows.Add(row);
            }
            dataGridView1.Columns[0].ReadOnly = true;
            //dataGridView1.Columns[1].DefaultCellStyle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridView1.Columns[1].DefaultCellStyle.ForeColor = Color.Blue;
            

        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                showInstances();
                //test();
                columnGrid();
                //Throw error if attachment cell is not selected.
                //make sure user select only single cell
                //if (dataGridView1.SelectedCells.Count == 1 && dataGridView1.SelectedCells[0].ColumnIndex == 1)
                //{
                    //UploadAttachment(dataGridView1.SelectedCells[0]);
                //}
                //else
                 //   MessageBox.Show("Select a single cell from Attachment column", "Error uploading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error uploading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnDownload_Click(object sender, EventArgs e)
        {
            //Throw error if attachment cell is not selected.
            //make sure user select only single cell
            //and the cell have a value in it
            if (dataGridView1.SelectedCells.Count == 1 && dataGridView1.SelectedCells[0].ColumnIndex == 1 && dataGridView1.SelectedCells[0].Value != null)
            {
                DownloadAttachment(dataGridView1.SelectedCells[0]);
            }
            else
                MessageBox.Show("Select a single cell from Attachment column", "Error uploading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Throw error if attachment cell is not selected.
            //make sure user select only single cell
            //and the cell have a value in it
            if (dataGridView1.SelectedCells.Count == 1 && dataGridView1.SelectedCells[0].Value != null)
            {
                //if (!toggleValue)
                    ShowPicture(dataGridView1, dataGridView1.SelectedCells[0], chkFitIn.Checked);
                //else
                    //HidePicture(dataGridView1, dataGridView1.SelectedCells[0]);
                //DownloadAttachment(dataGridView1.SelectedCells[0]);
            }
            else
                MessageBox.Show("Select a single cell from Attachment column", "Error uploading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Upload Attachment at the provided DataGridViewCell
        /// </summary>
        /// <param name="dgvCell"></param>
        /// 
        private void showInstances()
        {
            int count = 0;
            foreach (KeyValuePair<string, Bitmap> pair in _localInstance)
            {
                
                Byte[] binData; 
                using (var memoryStream = new MemoryStream())
                {
                    pair.Value.Save(memoryStream, ImageFormat.Bmp);
                    binData = memoryStream.ToArray();
                    _myAttachments.Add(count, binData);
                    
                }
                _myAttachmentFileNames.Add(count, pair.Key);
                
                //DataGridViewRow row = new DataGridViewRow();
                dataGridView1[0, count].Value = count.ToString();
                dataGridView1[1, count].Value = _myAttachmentFileNames[count];
                
                count++;
            }
            

        }

        private void test()
        {
            string index  = "3.2.1.1";
            if (_digit.ContainsKey(index))
            {
                string[] spil = index.Split('.');
                foreach (KeyValuePair<int, string> p in _myAttachmentFileNames)
                {
                    if (p.Value == spil[0] + "." + spil[1])
                    {
                        dataGridView1[Convert.ToInt32(spil[2]) * 2 + (Convert.ToInt32(spil[3]) - 1), p.Key].Value = "good";

                    }

                }
            }
            else
            {
                MessageBox.Show("booooooooo");
            }
            
        }
        private void columnGrid()
        {
            //index = "3.1.1.2";
            foreach (KeyValuePair<string, Bitmap> pair in _digit)
            {
                string tmp = pair.Key;
                string[] split = tmp.Split('.');
                //string row = split[0] + "." + split[1];


                
                foreach (KeyValuePair<int, string> p in _myAttachmentFileNames)
                {
                    if (p.Value == split[0] + "." + split[1])
                    {
                        dataGridView1[Convert.ToInt32(split[2]) * 2 + (Convert.ToInt32(split[3]) - 1), p.Key].Value = _map[pair.Key];
                        
                    }
                    
                }
            }
            
        }
        private void UploadAttachment(DataGridViewCell dgvCell)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                //Set File dialog properties
                fileDialog.CheckFileExists = true;
                fileDialog.CheckPathExists = true;
                fileDialog.Filter = "All Files|*.*";
                fileDialog.Title = "Select a file";
                fileDialog.Multiselect = false;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    FileInfo fileInfo = new FileInfo(fileDialog.FileName);
                    byte[] binaryData = File.ReadAllBytes(fileDialog.FileName);
                    dataGridView1.Rows[dgvCell.RowIndex].Cells[1].Value = fileInfo.Name;
                    
                    //Save Attachement
                    if (_myAttachments.ContainsKey(dgvCell.RowIndex))
                        _myAttachments[dgvCell.RowIndex] = binaryData;
                    else
                        _myAttachments.Add(dgvCell.RowIndex, binaryData);

                    //Save File Name
                    if (_myAttachmentFileNames.ContainsKey(dgvCell.RowIndex))
                        _myAttachmentFileNames[dgvCell.RowIndex] = Convert.ToString(dgvCell.Value);
                    else
                        _myAttachmentFileNames.Add(dgvCell.RowIndex,Convert.ToString(dgvCell.Value));
                }
            }
        }

        /// <summary>
        /// Download Attachment from the provided DataGridViewCell
        /// </summary>
        /// <param name="dgvCell"></param>
        private void DownloadAttachment(DataGridViewCell dgvCell)
        {
            string fileName = Convert.ToString(dgvCell.Value);

            //Return if the cell is empty
            if (fileName == string.Empty)
                return;

            FileInfo fileInfo = new FileInfo(fileName);
            string fileExtension = fileInfo.Extension;

            byte[] byteData = null;

            //show save as dialog
            using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
            {
                //Set Save dialog properties
                saveFileDialog1.Filter = "Files (*" + fileExtension + ")|*" + fileExtension;
                saveFileDialog1.Title = "Save File as";
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.FileName = fileName;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    byteData = _myAttachments[dgvCell.RowIndex];
                    File.WriteAllBytes(saveFileDialog1.FileName, byteData);
                }
            }
        }

        private void btnShow_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 1 && dataGridView1.SelectedCells[0].Value != null)
                ShowPicture(dataGridView1, dataGridView1.SelectedCells[0], chkFitIn.Checked);
            else
                MessageBox.Show("Select a single cell from Attachment column", "Error uploading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }


        private void btnHide_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count == 1  && dataGridView1.SelectedCells[0].Value != null)
                HidePicture(dataGridView1, dataGridView1.SelectedCells[0]);
            else
                MessageBox.Show("Select a single cell from Attachment column", "Error uploading file", MessageBoxButtons.OK, MessageBoxIcon.Error); 
        }

        private void HidePicture(DataGridView myGrid, DataGridViewCell dgvCell)
        {
            //toggleValue = false;
            Type tp = dgvCell.GetType();
            //Check the cell type of DatabaseStorage Field is "DataGridViewTextBoxCell"
            if (tp.Name == "DataGridViewImageCell")
            {

                //Initialize new DataGridViewTextBoxCell for the text to load
                DataGridViewTextBoxCell dgTxtCol = new DataGridViewTextBoxCell();
                //Assigne the file name to the cell
                
                dgTxtCol.Value = _myAttachmentFileNames[dgvCell.RowIndex];
                if (dgvCell.ColumnIndex == 1)
                {
                    dgTxtCol.Value = _myAttachmentFileNames[dgvCell.RowIndex];
                }
                else
                {
                    string s = _myAttachmentFileNames[dgvCell.RowIndex] + "." + ((dgvCell.ColumnIndex) / 2) + "." + ((dgvCell.ColumnIndex) % 2 + 1);

                    dgTxtCol.Value = _map[s]; 
                    
                }
                //Assigne the DataGridViewTextBoxCell to the Cell of DatabaseStorage Field 
                myGrid[dgvCell.ColumnIndex, dgvCell.RowIndex] = dgTxtCol;
                myGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                myGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            }
            
        }

        /// <summary>
        /// Routine Perform ShowPicture for the
        /// selected cell in the provided GridControl
        /// </summary>
        /// <param name="myGrid"></param>
        /// <param name="dgvCell"></param>
        /// <param name="fillIn"></param>
        private void ShowPicture(DataGridView myGrid, DataGridViewCell dgvCell, bool fillIn)
        {
            string s = _myAttachmentFileNames[dgvCell.RowIndex] + "." + ((dgvCell.ColumnIndex) / 2) + "." + ((dgvCell.ColumnIndex) % 2 + 1);
            //toggleValue = true;
            
            
            

            //get the image
            byte[] byteData;
            if (dgvCell.ColumnIndex == 1)
            {
                if (!_myAttachments.ContainsKey(dgvCell.RowIndex) || _myAttachments[dgvCell.RowIndex] == null)
                {
                    //toggleValue = false;
                    return;
                }
                 byteData = _myAttachments[dgvCell.RowIndex];
            }
            else
            {
                if (!_digit.ContainsKey(s) || _digit[s] == null)
                {
                    //toggleValue = false;
                    return;
                }
                using (var memoryStream = new MemoryStream())
                {
                    _digit[s].Save(memoryStream, ImageFormat.Bmp);
                    byteData = memoryStream.ToArray();
                }
            }
                Image returnImage = null;

            //I was not able to figure out a way to check the format of bytearray
            //instead i'm using a try-catch statement. which will throw exception if not an image
            try
            {
                //Convert byte to Image
                MemoryStream ms = new MemoryStream(byteData);
                returnImage = Image.FromStream(ms, false, true);
            }
            catch
            {
                MessageBox.Show("Not an Image File", "Show Picture", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //toggleValue = false;
                return;
            }

            //Initialize new DataGridViewImageCell for the image to load
            DataGridViewImageCell dgImageCol = new DataGridViewImageCell();

            //Resize the Image
            if (fillIn)
                returnImage =ResizeImageToFitInsideTheGrid(returnImage, myGrid);

            dgImageCol.Value = returnImage;

            //When ImageWidth is greater than ColumnWidth
            //Increase the width of the column to show full size of the image
            if (myGrid.Columns[dgvCell.ColumnIndex].Width < returnImage.Width)
                myGrid.Columns[dgvCell.ColumnIndex].Width = returnImage.Width;

            //Increase the height of the column to show full size of the image   
            if (myGrid.Rows[dgvCell.RowIndex].Height < returnImage.Height)
                myGrid.Rows[dgvCell.RowIndex].Height = returnImage.Height;

            //Assigne the DataGridViewImageCell to the Selected Cell
            myGrid[dgvCell.ColumnIndex, dgvCell.RowIndex] = dgImageCol;
            
        }

        private Image ResizeImageToFitInsideTheGrid(Image returnImage, DataGridView myGrid)
        {
            //Case 1: Image with Large Width & Height - 
            //      Reduce image height to display height & proportionate width
            //      if the proportionate width is still greater than the display width
            //      Reduce image width to display width & proportionate height
            //Case 2: Image with Large Width - 
            //      Reduce image width to display Width & proportionate Height
            //Case 3: Image with Large Height - 
            //      Reduce image height to display Height + proportionate Width
            //Case 4: Image with Small Width & Height - Do Nothing

            //Multiplying by 0.95 helps the image to fit inside the display area

            int dgvDisplayHeight = Convert.ToInt32((myGrid.DisplayRectangle.Height - myGrid.ColumnHeadersHeight) * .95);
            int dgvDisplayWidth = Convert.ToInt32((myGrid.DisplayRectangle.Width - myGrid.RowHeadersWidth) * .95);

            //Large Width & Large Height 
            if (returnImage.Width >= dgvDisplayWidth && returnImage.Height >= dgvDisplayHeight)
            {
                int h = Convert.ToInt32(dgvDisplayHeight);
                int w = Convert.ToInt32(h * (Convert.ToDouble(returnImage.Width) / Convert.ToDouble(returnImage.Height)));
                //if the proportionate width is still greater than the display width
                if (w > dgvDisplayWidth)
                {
                    w = Convert.ToInt32(dgvDisplayWidth);
                    h = Convert.ToInt32(w * (Convert.ToDouble(returnImage.Height) / Convert.ToDouble(returnImage.Width)));
                }
                returnImage = ResizeImage(returnImage, w, h);
            }
            //Large Height 
            else if (returnImage.Width <= dgvDisplayWidth && returnImage.Height >= dgvDisplayHeight)
            {
                int h = Convert.ToInt32(dgvDisplayHeight);
                int w = Convert.ToInt32(h * (Convert.ToDouble(returnImage.Width) / Convert.ToDouble(returnImage.Height)));
                returnImage = ResizeImage(returnImage, w, h);
            }
            //Large Width
            else if (returnImage.Width >= dgvDisplayWidth && returnImage.Height <= dgvDisplayHeight)
            {
                int w = Convert.ToInt32(dgvDisplayWidth);
                int h = Convert.ToInt32(w * (Convert.ToDouble(returnImage.Height) / Convert.ToDouble(returnImage.Width)));
                returnImage = ResizeImage(returnImage, w, h);
            }
            return returnImage;
        }


        /// <summary>
        /// Routine will resize the given images with
        /// the provided height and width
        /// </summary>
        /// <param name="actualImage">Original Image</param>
        /// <param name="width">new width</param>
        /// <param name="height">new height</param>
        /// <returns>Resized Image</returns>
        private Image ResizeImage(Image actualImage, int width, int height)
        {
            Bitmap bitmap = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage((Image)bitmap))
                graphics.DrawImage(actualImage, 0, 0, width, height);

            return (Image)bitmap;
        }


        




    }
}
