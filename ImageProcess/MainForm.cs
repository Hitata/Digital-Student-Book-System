using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

using AForge;
using AForge.Imaging;
using ImageProcess.KohonenNetwork;



namespace ImageProcessing
{
    internal partial class MainForm : Form
    {
        // selected folder containing images to process
        private string selectedFolder;
        // list of found image processing routines
        //private Dictionary<string, IImageProcessingRoutine> processingRoutines = new Dictionary<string,IImageProcessingRoutine>( );
        //// currently active image processing routine to use
        private Processing ipRoutineToUse = new Processing();
        // image processing log
        private ImageProcessingLog processingLog = new ImageProcessingLog( );
        private String test = "a";
        private Bitmap image;
        const int DOWNSAMPLE_WIDTH = 10;
        const int DOWNSAMPLE_HEIGHT = 15;
        const int DOWNSAMPLE_AREA = DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH;
        private Network network;
        public bool needThres;
        public bool hasNetwork;
        const int OUTPUT_COUNT = 100 * 100;
        public bool[] downsampled;
        
        //blob from image
        private Dictionary<string, Bitmap> blobs = new Dictionary<string, Bitmap>();

        private Dictionary<string, bool[]> data = new Dictionary<string, bool[]>();

        private Dictionary<string, string> lines = new Dictionary<string, string>();
        
        // list of recently used folders
        private List<string> recentFolders = new List<string>( );

        // stopwatch to measure time taken by image processing routine
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch( );

        #region Configuration Option Names
        private const string mainFormXOption = "MainFormX";
        private const string mainFormYOption = "MainFormY";
        private const string mainFormWidthOption = "MainFormWidth";
        private const string mainFormHeightOption = "MainFormHeight";
        private const string mainFormStateOption = "MainFormState";
        private const string splitter1Option = "Splitter1";
        private const string splitter2Option = "Splitter2";
        private const string splitter3Option = "Splitter3";
        private const string recentFolderOption = "RecentFolder";
        private const string pictureSizeModeOption = "PictureSizeMode";
        private const string openLastOption = "OpenLastFolder";
        #endregion


        public MainForm( )
        {
            InitializeComponent( );

        }

        // On form loading
        private void MainForm_Load( object sender, EventArgs e )
        {
            //.Text = "a('";
            
            //ImageProcess.VI
            ViqrConverter vs = new ViqrConverter();
            string s = "ho^m nay tro+`i dde.p";
            string m = vs.Convert(s, false);
            textBox1.AppendText(m);
            // declare downsample size [8,12]
            downsampled = new bool[DOWNSAMPLE_AREA];
            //downsampled = testsampled;
            Configuration config = Configuration.Instance;
            needThres = false;
            map = new string[OUTPUT_COUNT];
            hasNetwork = false;
            recognize.Enabled = false;
            loadMap.Enabled = false;
            loadSOM.Enabled = false;
            trainButton.Enabled = false;
            saveImage.Enabled = false;
            loadSOM.Enabled = false;
            deleteImage.Enabled = false;

            if ( config.Load( ) )
            {
                try
                {
                    bool windowPositionIsValid = false;
                    // get window location/size
                    Size windowSize = new Size(
                        int.Parse( config.GetConfigurationOption( mainFormWidthOption ) ),
                        int.Parse( config.GetConfigurationOption( mainFormHeightOption ) ) );
                    System.Drawing.Point windowTopLeft = new System.Drawing.Point(
                        int.Parse( config.GetConfigurationOption( mainFormXOption ) ),
                        int.Parse( config.GetConfigurationOption( mainFormYOption ) ) );
                    System.Drawing.Point windowTopRight = new System.Drawing.Point(
                        windowTopLeft.X + windowSize.Width, windowTopLeft.Y );

                    // check if window location is within of the displays
                    foreach ( Screen screen in Screen.AllScreens )
                    {
                        if ( ( screen.WorkingArea.Contains( windowTopLeft ) ) ||
                             ( screen.WorkingArea.Contains( windowTopRight ) ) )
                        {
                            windowPositionIsValid = true;
                            break;
                        }
                    }

                    if ( windowPositionIsValid )
                    {
                        Location = windowTopLeft;
                        Size = windowSize;

                        WindowState = (FormWindowState) Enum.Parse( typeof( FormWindowState ),
                            config.GetConfigurationOption( mainFormStateOption ) );

                        mainSplitContainer.SplitterDistance = int.Parse( config.GetConfigurationOption( splitter1Option ) );
                        splitContainer1.SplitterDistance = int.Parse( config.GetConfigurationOption( splitter2Option ) );
                        splitContainer2.SplitterDistance = int.Parse( config.GetConfigurationOption( splitter3Option ) );
                    }

                    // get size mode of picture box
                    SetPictureBoxSizeMode( (PictureBoxSizeMode) Enum.Parse( typeof( PictureBoxSizeMode ),
                        config.GetConfigurationOption( pictureSizeModeOption ) ) );

                    // get recent folders
                    for ( int i = 0; i < 7; i++ )
                    {
                        string rf = config.GetConfigurationOption( recentFolderOption + i );

                        if ( rf != null )
                            recentFolders.Add( rf );
                    }

                    RebuildRecentFoldersList( );

                    bool openLast = bool.Parse( config.GetConfigurationOption( openLastOption ) );
                    openLastFolderOnStartToolStripMenuItem.Checked = openLast;

                    if ( ( openLast ) && ( recentFolders.Count > 0 ) )
                    {
                        OpenFolder( recentFolders[0] );
                    }
                }
                catch
                {
                }
            }
        }

        // Rebuild menu with the list of recently used folders
        private void RebuildRecentFoldersList( )
        {
            // unsubscribe from events
            foreach ( ToolStripItem item in recentFoldersToolStripMenuItem.DropDownItems )
            {
                item.Click -= new EventHandler( recentFolder_Click );
            }

            // remove all current items
            recentFoldersToolStripMenuItem.DropDownItems.Clear( );

            // add new items
            foreach ( string folderName in recentFolders )
            {
                ToolStripItem item = recentFoldersToolStripMenuItem.DropDownItems.Add( folderName );

                item.Click += new EventHandler( recentFolder_Click );
            }
        }

        // On form closing
        private void MainForm_FormClosing( object sender, FormClosingEventArgs e )
        {
            Configuration config = Configuration.Instance;

            // save window location/size
            if ( WindowState != FormWindowState.Minimized )
            {
                if ( WindowState != FormWindowState.Maximized )
                {
                    config.SetConfigurationOption( mainFormXOption, Location.X.ToString( ) );
                    config.SetConfigurationOption( mainFormYOption, Location.Y.ToString( ) );
                    config.SetConfigurationOption( mainFormWidthOption, Width.ToString( ) );
                    config.SetConfigurationOption( mainFormHeightOption, Height.ToString( ) );
                }
                config.SetConfigurationOption( mainFormStateOption, WindowState.ToString( ) );

                config.SetConfigurationOption( splitter1Option, mainSplitContainer.SplitterDistance.ToString( ) );
                config.SetConfigurationOption( splitter2Option, splitContainer1.SplitterDistance.ToString( ) );
                config.SetConfigurationOption( splitter3Option, splitContainer2.SplitterDistance.ToString( ) );
            }

            // save size mode of picture box
            config.SetConfigurationOption( pictureSizeModeOption, pictureBox.SizeMode.ToString( ) );

            // save recent folders
            for ( int i = 0, n = recentFolders.Count; i < n; i++ )
            {
                config.SetConfigurationOption( recentFolderOption + i, recentFolders[i] );
            }
            config.SetConfigurationOption( openLastOption, openLastFolderOnStartToolStripMenuItem.Checked.ToString( ) );

            try
            {
                config.Save( );
            }
            catch ( IOException ex )
            {
                MessageBox.Show( "Failed saving confguration file.\r\n\r\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        // Add folder to the list of recently used folders
        public void AddRecentFolder( string folderName )
        {
            int index = recentFolders.IndexOf( folderName );

            if ( index != 0 )
            {
                if ( index != -1 )
                {
                    // remove previous entry
                    recentFolders.RemoveAt( index );
                }

                // put this folder as the most recent
                recentFolders.Insert( 0, folderName );

                if ( recentFolders.Count > 7 )
                {
                    recentFolders.RemoveAt( 7 );
                }
            }
        }

        // Remove specified folder from the list of recently used folders
        public void RemoveRecentFolder( string folderName )
        {
            recentFolders.Remove( folderName );
        }

        // Exit from application
        private void exitToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.Close( );
        }

        // Open folder
        private void openFolderToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( folderBrowserDialog.ShowDialog( ) == DialogResult.OK )
            {
                if ( OpenFolder( folderBrowserDialog.SelectedPath ) )
                {
                    // remember this folder
                    AddRecentFolder( selectedFolder );
                    RebuildRecentFoldersList( );
                }
            }
        }

        // Item is clicked in recent folders list
        private void recentFolder_Click( object sender, EventArgs e )
        {
            string folderName = ( (ToolStripMenuItem) sender ).Text;

            if ( OpenFolder( folderName ) )
            {
                // move the folder up in the list
                AddRecentFolder( folderName );
            }
            else
            {
                // remove failing folder
                RemoveRecentFolder( folderName );
            }
            RebuildRecentFoldersList( );
        }

        // Open specified folder
        private bool OpenFolder( string folderName )
        {
            bool success = false;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo( folderName );
                FileInfo[] fileInfos = dirInfo.GetFiles( );

                filesListView.Items.Clear( );

                // collect all image files
                foreach ( FileInfo fi in fileInfos )
                {
                    string ext = fi.Extension.ToLower( );

                    // check for supported extension
                    if (
                        ( ext == ".jpg" ) || ( ext == ".jpeg" ) ||
                        ( ext == ".bmp" ) || ( ext == ".png" ) )
                    {
                        filesListView.Items.Add( fi.Name );
                    }
                }

                logListView.Items.Clear( );
                filesListView.Focus( );
                ProcessSelectedImage( );

                selectedFolder = folderName;
                success = true;
            }
            catch
            {
                MessageBox.Show( "Failed opening the folder:\n" + folderName, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            }

            UpdateImageCountStatus( );

            return success;
        }

        //downsampling
        //private void normalizeBlob(Bitmap bitmap)
        //{
        //    DownSample ds = new DownSample(bitmap);
        //    downsampled =  ds.downSample(DOWNSAMPLE_WIDTH, DOWNSAMPLE_HEIGHT);
            
        //}


        // Selection has changed in files list view control
        private void filesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ProcessSelectedImage( );
            
            loadMap.Enabled = true;
            loadSOM.Enabled = true;
            trainButton.Enabled = true;
            saveImage.Enabled = true;
            loadSOM.Enabled = true;
            deleteImage.Enabled = true;
        }

        // Process currently selected image
        private void ProcessSelectedImage( )
        {
            if ( filesListView.SelectedItems.Count == 1 )
            {
                image = null;
                this.blobs.Clear();
                this.data.Clear();
                    
                
                try
                {
                    image = (Bitmap) Bitmap.FromFile( Path.Combine( selectedFolder, filesListView.SelectedItems[0].Text ) );
                }
                catch
                {
                }

                if ( image != null )
                {
                    ProcessImage( image );
                    ShowLogMessages( );
                    UpdateLogView( );
                    assignBlobName(ipRoutineToUse.blobs, ipRoutineToUse.inputData);
                    UpdateBlobView();
                   
                    
                    return;
                }
                else
                {
                    MessageBox.Show( "Failed loading file: " + filesListView.SelectedItems[0].Text,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    filesListView.Items.Remove( filesListView.SelectedItems[0] );
                    UpdateImageCountStatus( );
                }
            }

            pictureBox.Image = null;
            logBox.Text = string.Empty;
        }

        private void assignBlobName(Dictionary<string, Bitmap> blobs, Dictionary<string, bool[]> inputData)
        {
            int i = 0, j = 0;
            foreach(KeyValuePair<string, Bitmap> pair in blobs)
            {
                this.blobs.Add(pair.Key, pair.Value);
                i++;
            }
            foreach(KeyValuePair<string, bool[]> pair2 in inputData)
            {
                this.data.Add(pair2.Key, pair2.Value);
                j++; 
            }
            

        }
        // Process specified image
        private void ProcessImage( Bitmap image )
        {
            processingLog.Clear( );
            processingLog.AddImage( "Source", image );
            
            if ( ipRoutineToUse != null )
            {
                stopWatch.Reset( );
                stopWatch.Start( );

                // process image with selected image processing routine
                ipRoutineToUse.Process( image, processingLog );

                stopWatch.Stop( );

                UpdateProcessingTimeStatus( stopWatch.ElapsedMilliseconds );
            }
        }

        //Update Blob View
        private void UpdateBlobView()
        {
            string currentSelection = string.Empty;
            int newSelectionIndex = 0;
            int i = 0;

            if (blobView.SelectedIndices.Count > 0)
            {
                currentSelection = blobView.Items[blobView.SelectedIndices[0]].Text;
            }

            blobView.Items.Clear();

            foreach (KeyValuePair<string, Bitmap> blob in blobs)
            {

                //lines.Add(blob.Key, CheckStraightLine(downsampled).ToString());
                //string[] s = {blob.Key};
                ListViewItem list = new ListViewItem(blob.Key);
                blobView.Items.Add(list);

                if (blob.Key == currentSelection)
                    newSelectionIndex = i;

                i++;
            }
            //blobView.SelectedIndices.Add(newSelectionIndex);i
            //blobView.EnsureVisible(newSelectionIndex);

            this.sample.Invalidate();
        }
        private int CheckStraightLine(bool [] image)
        {
            //Accept An binary Arrray representation of image
            bool value = true;
            bool valueminus = false;
            int ind = 1;
           int count = 0;
           int row = -1;
           int max = 7;//DOWNSAMPLE_WIDTH * (1 / 3);
           bool yes = valueminus;
           if (image[0] == value)
           {
               yes = value;
           }
            while ( ind < image.Length/2)
            {

                if (image[ind] == value)
                {
                    if (yes == value)
                    {
                        count++;

                    }
                    yes = value;

                }
                else
                {
                    yes = valueminus;
                }

                    if (ind % 10 == 9)
                    {
                        if (count > max)
                        {
                            row++;
                            count = 0;
                        }
                    }
                ind++;
            }
            return row;
        }

        // Update log view
        private void UpdateLogView()
        {
            string currentSelection = string.Empty;
            int newSelectionIndex = 0;
            int i = 0;

            if ( logListView.SelectedIndices.Count > 0 )
            {
                currentSelection = logListView.Items[logListView.SelectedIndices[0]].Text;
            }

            logListView.Items.Clear( );

            foreach ( KeyValuePair<string, Bitmap> kvp in processingLog.Images )
            {
                logListView.Items.Add( kvp.Key );

                if ( kvp.Key == currentSelection )
                    newSelectionIndex = i;

                i++;
            }

            logListView.SelectedIndices.Add( newSelectionIndex );
            logListView.EnsureVisible( newSelectionIndex );
        }

        // Display log messages
        private void ShowLogMessages( )
        {
            StringBuilder sb = new StringBuilder( );

            foreach ( string message in processingLog.Messages )
            {
                sb.Append( message );
                sb.Append( "\r\n" );
            }

            logBox.Text = sb.ToString( );
        }

        // Selecetion has changed in blob list view - update blob image
        private void blobView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blobView.SelectedIndices.Count == 1)
            {
                string stepName = blobView.SelectedItems[0].Text;
                 blobImage.Image = (Bitmap)blobs[stepName].Clone();

                //downsampled = downSample((Bitmap)blobs[stepName].Clone()); 
                //blobImage.Image = (Bitmap)temp;
                downsampled = (bool[])data[stepName].Clone();
                //lineBox.Text = lines[stepName];
                
                //normalizeBlob((Bitmap)blobs[stepName].Clone());
            }
            
            this.sample.Invalidate();
        }
        // Selection has changed in log list view - update image
        private void logListView_SelectedIndexChanged( object sender, EventArgs e )
        {
            if ( logListView.SelectedIndices.Count == 1 )
            {
                string stepName = logListView.SelectedItems[0].Text;

                Bitmap oldImage = (Bitmap) pictureBox.Image;
                pictureBox.Image = (Bitmap) processingLog.Images[stepName].Clone( );

                if ( oldImage != null )
                {
                    oldImage.Dispose( );
                }

                 UpdateImageSizeStatus( );
            }
        }

        // Update status of menu items in Settings->Image view
        private void imageviewToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            normalToolStripMenuItem.Checked   = ( pictureBox.SizeMode == PictureBoxSizeMode.Normal );
            centerToolStripMenuItem.Checked   = ( pictureBox.SizeMode == PictureBoxSizeMode.CenterImage );
            stretchToolStripMenuItem.Checked  = ( pictureBox.SizeMode == PictureBoxSizeMode.StretchImage );
            autoSizeToolStripMenuItem.Checked = ( pictureBox.SizeMode == PictureBoxSizeMode.AutoSize );
        }

        // Set Normal view for images
        private void normalToolStripMenuItem_Click( object sender, EventArgs e )
        {
            SetPictureBoxSizeMode( PictureBoxSizeMode.Normal );
        }

        // Set Centred view for images
        private void centerToolStripMenuItem_Click( object sender, EventArgs e )
        {
            SetPictureBoxSizeMode( PictureBoxSizeMode.CenterImage );
        }

        // Set Stretched view for images
        private void stretchToolStripMenuItem_Click( object sender, EventArgs e )
        {
            SetPictureBoxSizeMode( PictureBoxSizeMode.StretchImage );
        }

        // Set Auto Size view for image
        private void autoSizeToolStripMenuItem_Click( object sender, EventArgs e )
        {
            SetPictureBoxSizeMode( PictureBoxSizeMode.AutoSize );
        }

        // Set size mode for picture box
        private void SetPictureBoxSizeMode( PictureBoxSizeMode sizeMode )
        {
            if ( sizeMode == PictureBoxSizeMode.AutoSize )
            {
                pictureBox.Dock = DockStyle.None;
                pictureBox.Location = new System.Drawing.Point( 0, 0 );
                splitContainer2.Panel1.AutoScroll = true;
            }
            else
            {
                pictureBox.Dock = DockStyle.Fill;
                splitContainer2.Panel1.AutoScroll = false;
            }

            pictureBox.SizeMode = sizeMode;
        }

        // Switch option for openning last folder on application load
        private void openLastFolderOnStartToolStripMenuItem_Click( object sender, EventArgs e )
        {
            openLastFolderOnStartToolStripMenuItem.Checked = !openLastFolderOnStartToolStripMenuItem.Checked;
        }

        // Show about form
        private void aboutToolStripMenuItem_Click( object sender, EventArgs e )
        {
            AboutForm form = new AboutForm( );

            form.ShowDialog( );
        }

        // Copy current image to clipboard
        private void copyImageClipboardToolStripMenuItem_Click( object sender, EventArgs e )
        {
            if ( pictureBox.Image != null )
            {
                Clipboard.SetImage( pictureBox.Image );
            }
        }

        // Update status of "copy to clipboard" menu item
        private void toolsToolStripMenuItem_DropDownOpening( object sender, EventArgs e )
        {
            copyImageClipboardToolStripMenuItem.Enabled = ( pictureBox.Image != null );
        }
        private void sample_MouseUP(object sender, MouseEventArgs e)
        {
            Graphics g = sample.CreateGraphics();
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                g.FillRectangle(new SolidBrush(Color.Red), e.X, e.Y, e.X + 5, e.Y + 5);
            }
            g.Dispose();
        }

        //sample paint grid 8 12
        private void sample_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int x, y;
            int vcell = (sample.Height) / MainForm.DOWNSAMPLE_HEIGHT;
            int hcell = (sample.Width) / MainForm.DOWNSAMPLE_WIDTH;
            Brush whiteBrush = new SolidBrush(Color.White);
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen blackPen = new Pen(Color.Black);

            g.FillRectangle(whiteBrush, 0, 0, sample.Width, sample.Height);



            for (y = 0; y < MainForm.DOWNSAMPLE_HEIGHT; y++)
            {
                g.DrawLine(blackPen, 0, y * vcell, sample.Width, y * vcell);
            }
            for (x = 0; x < MainForm.DOWNSAMPLE_WIDTH; x++)
            {
                g.DrawLine(blackPen, x * hcell, 0, x * hcell, sample.Height);
            }

            int index = 0;
            for (y = 0; y < MainForm.DOWNSAMPLE_HEIGHT; y++)
            {
                for (x = 0; x < MainForm.DOWNSAMPLE_WIDTH; x++)
                {
                    if (this.downsampled[index++])
                    {
                        g.FillRectangle(blackBrush, x * hcell, y * vcell, hcell, vcell);
                    }
                }
            }

            g.DrawRectangle(blackPen, 0, 0, sample.Width-1, sample.Height -1);
        }
        

        // Update status label displaying number of available images
        private void UpdateImageCountStatus( )
        {
            imagesCountLabel.Text = "Images: " + filesListView.Items.Count.ToString( );
        }

        // Update staus label displaying processing time of last routine
        private void UpdateProcessingTimeStatus( long msTime )
        {
            double sTime = (double) msTime / 1000;

            processingTimeLabel.Text = string.Format( "Processing time: {0}s", sTime.ToString( "F3" ) );
        }

        // Update status label displaying size of selected image
        private void UpdateImageSizeStatus( )
        {
            if ( pictureBox.Image != null )
            {
                imageSizeLabel.Text = string.Format( "Image size: {0}x{1}",
                    pictureBox.Image.Width, pictureBox.Image.Height );
            }
            else
            {
                imageSizeLabel.Text = string.Empty;
            }
        }

        // Resize columns of file list view and log list view
        private void mainSplitContainer_Panel1_Resize( object sender, EventArgs e )
        {
            if ( fileNameColumn != null )
            {
                fileNameColumn.Width = filesListView.Width - 24;
            }
            if ( processingStepsColumn != null )
            {
                processingStepsColumn.Width = logListView.Width - 24;
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void logBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void splitContainer2_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void mainSplitContainer_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void blobImage_Click(object sender, EventArgs e)
        {

        }

        private void splitContainer4_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void trainButton_Click(object sender, EventArgs e)
        {
            ImageProcess.TrainSOM tS = new ImageProcess.TrainSOM(this.data);
            tS.ShowDialog();
            //ImageProcess.SOM_Image_Form ts = new ImageProcess.SOM_Image_Form();
            //ts.ShowDialog();
        }

        

        private void loadSOM_Click(object sender, EventArgs e)
        {
            String strFileName = string.Empty;
            OpenFileDialog loadNetworkDialog = new OpenFileDialog();
            loadNetworkDialog.Filter = "network files (*.koh)|*.koh|All files (*.*)|*.*";

            //loadTrainDataDialog.InitialDirectory = "..\\..\\";
            loadNetworkDialog.Title = "Select a Network file";

            //Present to the user.
            if (loadNetworkDialog.ShowDialog() == DialogResult.OK)
                strFileName = loadNetworkDialog.FileName;
            if (strFileName == String.Empty)
                return;//user didn't select a file

            network = Network.Load(strFileName);
            //network.MapChar = new string[OUTPUT_COUNT];

            MessageBox.Show(this, "Loaded from network file");
            hasNetwork = true;
            recognize.Enabled = true;
        }

                public string[] map;
        private void loadMap_Click(object sender, EventArgs e)
        {
            
            
            String strFileName = string.Empty;

            
            OpenFileDialog loadTrainDataDialog = new OpenFileDialog();
            loadTrainDataDialog.Filter = "map files (*.map)|*.map|All files (*.*)|*.*";
            //loadTrainDataDialog.InitialDirectory = "..\\..\\";
            loadTrainDataDialog.Title = "Select a Map file";
 
            //Present to the user.
            if (loadTrainDataDialog.ShowDialog() == DialogResult.OK)
                strFileName = loadTrainDataDialog.FileName;
            if (strFileName == String.Empty)
                 return;//user didn't select a file
            try
            {
                StreamReader f = new StreamReader(strFileName);

                String line;
                int i = 0;

                while ((line = f.ReadLine()) != null)
                {
                        for(int m = 1; m < line.Length; m++)
                                map[i] += line[m];
                        i++;
                }

                f.Close();

                MessageBox.Show(this, "Loaded from map data");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        
        


        }

        private void deleteImage_Click(object sender, EventArgs e)
        {
            try
            {
                string str = blobView.SelectedItems[0].Text;
                data.Remove(str);
                blobView.SelectedItems[0].Remove();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Select an sample in Array inorder to Delete\nError " + ex.Message);
            }
        
        }
        private Dictionary<string, string> recogMap = new Dictionary<string, string>(); 
        
        private void recognize_Click(object sender, EventArgs e)
        {
            textBox1.Clear();
            string result = "";
            double[] value = new double[DOWNSAMPLE_AREA];
            int winner = -1;
            int previous = 0;
            foreach (KeyValuePair<string, bool[]> d in data)
            {
                if (Int32.Parse(d.Key.Substring(0, 1)) > previous)
                {
                    textBox1.AppendText("\n");
                }
                previous = Int32.Parse(d.Key.Substring(0,1));

                for (int i = 0; i < DOWNSAMPLE_AREA; i++)
                {
                    if (d.Value[i])
                        value[i] = (double)1;
                    else
                        value[i] = (double)0;
                }
                network.Compute(value);
                winner = network.GetWinner();
                recogMap.Add(d.Key, map[winner]);
                textBox1.AppendText("" + map[winner]);
                
            }
            //textBox1.Text = result;
            result = textBox1.Text;

            ViqrConverter vs = new ViqrConverter();
            string s = vs.Convert(result, false);
            textBox1.AppendText("\n-----------------------");
            textBox1.AppendText(s);            

        }

        private void processingTimeLabel_Click(object sender, EventArgs e)
        {

        }
        private Dictionary<string, Bitmap> rowInstance = new Dictionary<string,Bitmap>();
        private Dictionary<string, Bitmap> digit = new Dictionary<string, Bitmap>();
        private void button1_Click(object sender, EventArgs e)
        {
            rowInstance = ipRoutineToUse.columnBinImage;
            digit = ipRoutineToUse.blobs;
            ImageProcess.Form1 ds = new ImageProcess.Form1(rowInstance, digit, recogMap);
            ds.ShowDialog(); 
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            //string stepName = blobView.SelectedItems[0].Text;
            lineBox.Text = CheckStraightLine(downsampled).ToString();

        }

        
       

    }
}
   