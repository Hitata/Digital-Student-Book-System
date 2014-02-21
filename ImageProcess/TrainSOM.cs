using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using AForge;


//using System.Threading;
using System.IO;
using ImageProcess.KohonenNetwork;
using System.Collections;

namespace ImageProcess
{
    public partial class TrainSOM : Form
    {
        System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();

        public Dictionary<string, bool[]> data = new Dictionary<string, bool[]>();
        private bool[] downsampled;
        const int DOWNSAMPLE_HEIGHT = 15;
        const int DOWNSAMPLE_WIDTH = 10;
        const int DOWNSAMPLE_AREA = DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH;
        public string end = "";
        //Panel point click
        private double[] value;
        public static string [] map;
        public static int[] win;
        public static string [] dataMap;
        //public static int nodeWidth = 384/20 ;
        //public static int nodeHeight = 247/20;

        public bool m_isNodeSelected;
        public System.Drawing.Point m_selectedNodeCoords;
        
        //private class
        private CSOM m_SOM;

        private int mapCount;
        //Trainning set 0 1
        private double[][] trainingSet;

        //Output Network. Kohonen Size
        
        public const int NUM_NODES_DOWN = 100;
        
        public const int NUM_NODES_ACROSS = 100;

        private const int OUTPUT_COUNT = NUM_NODES_DOWN*NUM_NODES_ACROSS;
        //Kohonen
        private Network network;

        private bool hasNetwork;
        private Random rand = new Random();

        private int iterations = 5000;
        private double learningRate = 0.5;
        private double radius = 50;
        private int numberWin = 0;
        private Thread workerThread = null;
        private volatile bool needToStop = false;
        private AppSettings m_appSettings;
        public static int pos = 0;
        //private System.Windows.Forms.Timer m_timer;
        Graphics s;
        Bitmap b;
        Graphics g;

        public TrainSOM(Dictionary<string, bool[]> data)
        {
            InitializeComponent();
            map = new string[OUTPUT_COUNT];
            win = new int[OUTPUT_COUNT];
            for (int i = 0; i < map.Length; i++)
            {
                map[i] = "7.2";
                win[i] = 0;
            }
            this.data = data;
            downsampled = new bool[DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH];
            value = new double[DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH];
            
            mapCount = -1;
            dataMap = new string[65000];
            for (int i = 0; i < dataMap.Length; i++)
            {
                dataMap[i] = "&";
            }


            UpdateBlobView();

            //errorMap = new double[NUM_NODES_ACROSS, NUM_NODES_DOWN];             
               m_isNodeSelected = false;
              m_selectedNodeCoords = new System.Drawing.Point( 0, 0 );
            //Pannel point initialize
            
            //each node size
              //s = new Graphics();
              s = mapSOM.CreateGraphics();
              b = new Bitmap(mapSOM.Width, mapSOM.Height);
              g = Graphics.FromImage(b);

            hasNetwork = false;
            //Initialize Kohonen
                        //// Create map bitmap
            //m_timer = new System.Windows.Forms.Timer();
            //m_timer.Enabled = true;
            //m_timer.Interval = 0.01;
            //m_timer.Tick += new System.EventHandler(timer_tick);
            //button disable
            trainButton.Enabled = false;
            stopButton.Enabled = false;
            saveNetwork.Enabled = false;
            
            //update status
            numInput.Text = "" + data.Count();

            //update value 
            UpdateSettings();

            m_appSettings = new AppSettings();
            onReset();
        }

        private void updateAppSettings()
        {
            
            // Assumes both rendering windows are the same dimensions.
            //Size s = panel_Network.Size;
            m_appSettings.width = (float)mapSOM.Width;
            m_appSettings.height = (float)mapSOM.Height;

            // Calculate node square dimensions.
            m_appSettings.nodeWidth = m_appSettings.width / (float)TrainSOM.NUM_NODES_ACROSS;
            m_appSettings.nodeHeight = m_appSettings.height / (float)TrainSOM.NUM_NODES_DOWN;
            for (int ms = 0; ms < map.Length; ms++)
            {
                m_appSettings.map[ms] = map[ms];
                m_appSettings.win[ms] = win[ms];
            }
        }

        private void onReset()
        {
            updateAppSettings();
            m_SOM = new CSOM(m_appSettings);
            

        }
        public class AppSettings
        {
            //public INIT_FILL initFill;
            public string[] map;
            public int[] win;
            public float width, height;
            public float nodeWidth, nodeHeight;     // Used to represent nodes as colored squares.
            //public float startLearningRate_P1, startLearningRate_P2;
            //public int numIterations_P1, numIterations_P2;
            //public ArrayList inputVectors;          // Contains InputVectors.
            //public float mapRadius;
            //public string imageDirectory;
            //public bool isXMLProvided;      // TODO:
            //public string XMLFileName;      // TODO:
            //public bool recalculateDataVectors; // TODO:

            //public ArrayList images;    // Used to store the image data each application run.

            public AppSettings()
            {
                map = new string[NUM_NODES_ACROSS*NUM_NODES_DOWN];
                win = new int[NUM_NODES_DOWN*NUM_NODES_ACROSS];
                //inputVectors = new ArrayList();
                //images = new ArrayList();
            }
        }


        //update iteration, learning rate, radius
        private void UpdateSettings()
        {
            iterationsBox.Text = iterations.ToString();
            rateBox.Text = learningRate.ToString();
            radiusBox.Text = radius.ToString();
        }

        //start train on click button
        private void trainButton_Click(object sender, EventArgs e)
        {
            try
            {
                iterations = Math.Max(10, Math.Min(1000000, int.Parse(iterationsBox.Text)));
            }
            catch
            {
                iterations = 500;
            }
            // get learning rate
            try
            {
                learningRate = Math.Max(0.00001, Math.Min(1.0, double.Parse(rateBox.Text)));
            }
            catch
            {
                learningRate = 0.1;
            }
            // get radius
            try
            {
                radius = Math.Max(5, Math.Min(75, int.Parse(radiusBox.Text)));
            }
            catch
            {
                radius = 10;
            }
            // update settings controls
			UpdateSettings( );

            //converting
            numberWin = averageDataConvert();
            // disable all settings controls except "Stop" button
			EnableControls( false );

			// run worker thread
			needToStop = false;
			workerThread = new Thread( new ThreadStart( SearchSolution ) );
			workerThread.Start( );

            //network.Save("tempNetwork.koh");
            //MessageBox.Show("network train complete and saving");
        }
        private List<string> CharList;
        private int averageDataConvert()
        {
            int inputCount = DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH;
            //int letterCount = this.blobView.Items.Count;
            List<double[]> listSet = new List<double[]>();
            CharList = new List<string>();
            //declare lower
            //this.trainingSet = new double[letterCount][];
            //int index = 0;
            CharList.Add(dataMap[0]);
            for(int i = 1; i < this.data.Count; i++)
            {
                if(!CharList.Contains(dataMap[i]))
                    CharList.Add(dataMap[i]);
            }
            for(int k= 0; k < CharList.Count; k++)
            {
                double [] avrData = new double [inputCount];
                for(int t = 0; t < inputCount; t++)
                        {
                            avrData[t] = 0; 
                        }
                int totalData = 0;
                for(int j = 0; j < this.data.Count; j++)
                {
                    if(dataMap[j] == CharList[k])
                    {
                        totalData ++;
                        bool [] tempData = this.data.Values.ToList()[j];
                        double [] tempDataDouble = new double [inputCount];
                        for(int s = 0; s < inputCount; s++)
                        {
                            tempDataDouble[s] = tempData[s] ? 1 : 0;
                        }
                        for(int m = 0; m < inputCount;  m++)
                        {
                            avrData[m] +=tempDataDouble[m];
                        }
                        
                    }
                  }
                for(int p = 0; p < inputCount; p++)
                {
                    avrData[p] = avrData[p]/totalData;
                }
                listSet.Add(avrData);
            }
            this.trainingSet = new double[listSet.Count][];
            for (int v = 0; v < listSet.Count; v++)
            {
                trainingSet[v] = listSet[v];
            }
            for (int h = 0; h < CharList.Count; h++)
            {
                dataMap[h] = CharList[h];
            }
        return listSet.Count;
        }
        
        
        //convert from Dictionary<String, bool[]> to trainingSet
        private int inputDataConvert()
        {
            int inputCount = DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH;
            int letterCount = this.blobView.Items.Count;
            this.trainingSet = new double[letterCount][];
            int index = 0;
            
            foreach (string ch in this.data.Keys)
            {
                //this.trainingSet[index] = new double[inputCount];
                bool[] data = this.data[ch];    
                for (int i = 0; i < inputCount; i++)
                {
                    this.trainingSet[index][i] = data[i] ? 1 : 0;
                }
                index++;
            }
            return letterCount;
        }
        //map for winning neuron
        //public char[] mapNeurons()
        //{
        //    char[] map = new char[this.blobView.Items.Count];

        //    for (int i = 0; i < map.Length; i++)
        //    {
        //        map[i] = '?';
        //    }
        //    for (int i = 0; i < this.blobView.Items.Count; i++)
        //    {

                
        //        double[] input = new double[DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH];
        //        char ch = ((string)(this.blobView.Items[i]))[0];
        //        bool[] data = this.letterData[ch];
        //        for (int j = 0; j < input.Length; j++)
        //        {
        //            input[j] = data[j] ? 0.5 : -0.5;
        //        }
        //        int best = this.network.GetWinner(input);
        //        map[best] = ch;
        //    }
        //    return map;
        //}

        //run thread to train
        private Random rnd = new Random();
        void SearchSolution()
        {
            // create learning algorithm
            SOMLearning trainer = new SOMLearning(network, NUM_NODES_ACROSS, NUM_NODES_DOWN);
            
            double fixedLearningRate = learningRate / 10;
            double driftingLearningRate = fixedLearningRate * 9;
            stopWatch.Reset();
            stopWatch.Start();
            // iterations
            int i = 0;

            // loop
            while (!needToStop) 
            {
                trainer.LearningRate = driftingLearningRate * (iterations - i) / iterations + fixedLearningRate;
                trainer.LearningRadius = (double)radius * (iterations - i) / iterations;

                //trainer.LearningRadius = radius * Math.Exp(-(double)i / (iterations / 5));
                //trainer.LearningRate = learningRate * Math.Exp(-(double)i / (iterations / 5));
                List<double[]> shuffleSet = new List<double[]>();
                foreach (double[] train in trainingSet)
                {
                    shuffleSet.Add(train);
                }
                for (int s = 0; s < shuffleSet.Count; s++)
                {
                    int index = rnd.Next(shuffleSet.Count);
                    //double[] patern = shuffleSet[)];
                    trainer.Run(shuffleSet[index]);
                    shuffleSet.RemoveAt(index);
                        
                }
                //trainer.RunEpoch(trainingSet);
                

                // update map once per 50 iterations
                

                // increase current iteration
                i++;

                // set current iteration's info
                SetText(iterationLabel, (i + " | " + Convert.ToInt16(trainer.LearningRadius) + " | " + Convert.ToInt16(trainer.LearningRate*100)).ToString());

                // stop ?
                if (i >= iterations)
                    break;
            }
            stopWatch.Stop();

            UpdateProcessingTimeStatus(stopWatch.ElapsedMilliseconds);
            // enable settings controls
            EnableControls(true);
        }
        private void UpdateProcessingTimeStatus(long msTime)
        {
            network.TimeTrain = (double)msTime / 1000;
            
            TimeSpan t = TimeSpan.FromSeconds(network.TimeTrain);
            
            trainTime.Text = string.Format("Processing time: {0}s", t.ToString());
        }

        private delegate void EnableCallback( bool enable );

        private void EnableControls(bool enable)
        {
            if (InvokeRequired)
            {
                EnableCallback d = new EnableCallback(EnableControls);
                Invoke(d, new object[] { enable });
            }
            else
            {
                iterationsBox.Enabled = enable;
                rateBox.Enabled = enable;
                radiusBox.Enabled = enable;

                trainButton.Enabled = enable;
                //randomizeButton.Enabled	= enable;
                stopButton.Enabled = !enable;
            }
        }

        // Delegates to enable async calls for setting controls properties
        
        private delegate void SetTextCallback(System.Windows.Forms.Control control, string text);

        // Thread safe updating of control's text property
        private void SetText(System.Windows.Forms.Control control, string text)
        {
            if (control.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                Invoke(d, new object[] { control, text });
            }
            else
            {
                control.Text = text;
            }
        }

        // On main form closing
        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // check if worker thread is running
            if ((workerThread != null) && (workerThread.IsAlive))
            {
                needToStop = true;
                while (!workerThread.Join(100))
                    Application.DoEvents();
            }
            g.Dispose();
            s.Dispose();
        }

        private void blobView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (blobView.SelectedIndices.Count == 1)
            {
                string stepName = blobView.SelectedItems[0].SubItems[1].Text;
                
                //downsampled = downSample((Bitmap)blobs[stepName].Clone());
                //blobImage.Image = (Bitmap)temp;
                downsampled = (bool[])data[stepName].Clone();
                //normalizeBlob((Bitmap)blobs[stepName].Clone());
                label5.Text = dataMap[blobView.SelectedIndices[0]];
            }
            
            this.sample.Invalidate();
        }

        private void UpdateBlobView()
        {
            int i = 0;
            //string currentSelection = string.Empty;
            //int newSelectionIndex = 0;
            

            //if (blobView.SelectedIndices.Count > 0)
            //{
            //    currentSelection = blobView.Items[blobView.SelectedIndices[0]].Text;
            //}
            blobView.BeginUpdate();
            blobView.Items.Clear();

            foreach (KeyValuePair<string, bool[]> blob in data)
            {
                ListViewItem lvi = new ListViewItem(dataMap[i]);
                lvi.SubItems.Add(blob.Key);
                //blobView.Items[1].SubItems.Add(dataMap[i]);
                //blobView.Items.Add("Mapped").SubItems.Add(dataMap);
                //blobView.Items.Add("Mapped").SubItems.Add(dataMap[i]);
                
                blobView.Items.Add(lvi);
                
                //if (blob.Key == currentSelection)
                //    newSelectionIndex = i;

                i++;
            }
            blobView.EndUpdate();
            blobView.Refresh();
            //blobView.SelectedIndices.Add(newSelectionIndex);
            //blobView.EnsureVisible(newSelectionIndex);

            this.sample.Invalidate();
        }

        //check number in save data 
        private void checknumber(string strFileName)
        {
            int temp;
            using (FileStream fs = new FileStream(strFileName, FileMode.Open, FileAccess.Read))
            {
                    fs.Seek(-10, SeekOrigin.End);
                    if ( (Convert.ToChar(fs.ReadByte())) == '#')
                    {
                        while ((temp = fs.ReadByte()) > 0)
                        {
                            end += Convert.ToChar(temp);
                        }
                    }
            }
            iterationLabel.Text = "dude " + end + " end";
        }
        
        private void loadData_Click(object sender, EventArgs e)
        {
            String strFileName = string.Empty;

            loadTrainDataDialog.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";

            //loadTrainDataDialog.InitialDirectory = "..\\..\\";
            loadTrainDataDialog.Title = "Select a Data file";
 
            //Present to the user.
            if (loadTrainDataDialog.ShowDialog() == DialogResult.OK)
                strFileName = loadTrainDataDialog.FileName;
            if (strFileName == String.Empty)
                 return;//user didn't select a file
            try
            {
                StreamReader f = new StreamReader(strFileName);

                String line;

                this.data.Clear();
                blobView.Items.Clear();
                int sampleSize = DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH;
                int count = 0;
                

                while ((line = f.ReadLine()) != null)
                {
                    string ch = "";
                    //string mapN = "";
                    bool[] sample = new bool[sampleSize];
                    //int i = 2;
                    //get map 
                    
                    if (line[0] == '_')
                        dataMap[count] = "" + line[1];
                    else
                        dataMap[count] = "" + line[0] + line[1];

                    
                    //get sample
                    
                    //mapN += line[i] + line[i++];
                    for (int i = 0; i < sampleSize; i++)
                    {
                        if (line[i + 2] == '1')
                            sample[i] = true;
                        else
                            sample[i] = false;
                        //inputMap.Text = "" + sample.Length;
                    }

                    //get number
                    //for (int j = sampleSize + 2; j < line.Length; j++)
                    //    ch += line[j]; 
                    ch = "#" + count.ToString();
                    this.data.Add(ch, sample);
                    
                    ListViewItem lvi = new ListViewItem(dataMap[count]);
                    lvi.SubItems.Add(ch);
                    this.blobView.Items.Add(lvi);
                    count++;
                }

                f.Close();
               
                MessageBox.Show(this, "Dataset of: " + data.Count + " " + blobView.Items.Count);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void saveData_Click(object sender, EventArgs e)
        {
            String strFileName = string.Empty;

            saveTrainDataDialog.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";

            //loadTrainDataDialog.InitialDirectory = "..\\..\\";
            saveTrainDataDialog.Title = "Select a Data file";
            saveTrainDataDialog.CheckPathExists = true;
            saveTrainDataDialog.RestoreDirectory = true; 
            //Present to the user.
            if (saveTrainDataDialog.ShowDialog() == DialogResult.OK)
                strFileName = saveTrainDataDialog.FileName;
            if (strFileName == String.Empty)
                return;//user didn't select a file
            
            try
            {
                StreamWriter f = new StreamWriter(strFileName, true);
                int size = DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH;
                
                
                for (int i = 0; i < blobView.Items.Count; i++)
                {
                    string ch = blobView.Items[i].SubItems[1].Text;
                    bool[] boolData = this.data[ch];
                    string map = blobView.Items[i].SubItems[0].Text;
                    if (map.Length == 1)
                        map = "_" + map;
                    //f.Write(map);
                    for (int j = 0; j < size; j++)
                    {
                        map += (boolData[j] ? "1" : "0");

                    }
                    map += ch;
                    f.WriteLine(map);


                }
                f.Close();
                MessageBox.Show("Saved to "  + strFileName + ".dat");


            }
            catch (Exception e2)
            {
                MessageBox.Show("Error: " + e2.Message, "Training");
            }
        }


        
        private void sample_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int x, y;
            int vcell = (sample.Height) / DOWNSAMPLE_HEIGHT;
            int hcell = (sample.Width) / DOWNSAMPLE_WIDTH;
            Brush whiteBrush = new SolidBrush(Color.White);
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen blackPen = new Pen(Color.Black);
            
            g.FillRectangle(whiteBrush, 0, 0, sample.Width, sample.Height);



            for (y = 0; y < DOWNSAMPLE_HEIGHT; y++)
            {
                g.DrawLine(blackPen, 0, y * vcell, sample.Width, y * vcell);
            }
            for (x = 0; x < DOWNSAMPLE_WIDTH; x++)
            {
                g.DrawLine(blackPen, x * hcell, 0, x * hcell, sample.Height);
            }

            int index = 0;
            for (y = 0; y < DOWNSAMPLE_HEIGHT; y++)
            {
                for (x = 0; x < DOWNSAMPLE_WIDTH; x++)
                {
                    if (this.downsampled[index++])
                    {
                        g.FillRectangle(blackBrush, x * hcell, y * vcell, hcell, vcell);
                    }
                }
            }

            g.DrawRectangle(blackPen, 0, 0, sample.Width-1, sample.Height -1);
        }

        private void deleteData_Click(object sender, EventArgs e)
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
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (m_SOM == null)
                return;
            m_SOM.render(g, s, b);
        }
        public class CSOM
        {
            public bool m_isNodeSelected;
            public System.Drawing.Point m_selectedNodeCoords;
            public TrainSOM.AppSettings m_appSettings;
            public string[] mapS;
            public int[] winNode;

            public CSOM(TrainSOM.AppSettings appSettings)
            {
                mapS = new string[NUM_NODES_ACROSS*NUM_NODES_DOWN];
                winNode = new int[NUM_NODES_ACROSS * NUM_NODES_DOWN];

                for (int i = 0; i < mapS.Length; i++)
                {
                    mapS[i] = appSettings.map[i];
                    winNode[i] = appSettings.win[i];
                }
                m_appSettings = appSettings;
                m_isNodeSelected = false;
                m_selectedNodeCoords = new System.Drawing.Point(0, 0);

            }

            public void render(Graphics theWindows, Graphics offscreenWindows, Bitmap offscreenBitmaps)
            {
                

                // Manually 'blit' to background color.
                SolidBrush whiteBrush = new SolidBrush(Color.LightGray);
                Rectangle fullRect = new Rectangle(0, 0, (int)m_appSettings.width, (int)m_appSettings.height);
                offscreenWindows.FillRectangle(whiteBrush, fullRect);

                // Network window "grid" code:
                Pen pen0;
                pen0 = new Pen(Color.DimGray, 1.0f);

                // Calculate Vertical Lines.
                System.Drawing.Point[] pTop = new System.Drawing.Point[TrainSOM.NUM_NODES_ACROSS - 1];
                System.Drawing.Point[] pBottom = new System.Drawing.Point[TrainSOM.NUM_NODES_ACROSS - 1];
                for (int i = 0; i < TrainSOM.NUM_NODES_ACROSS - 1; ++i)
                {
                    pTop[i] = new System.Drawing.Point((i + 1) * (int)m_appSettings.nodeWidth - 1, 0);
                    pBottom[i] = new System.Drawing.Point((i + 1) * (int)m_appSettings.nodeWidth - 1, (int)m_appSettings.height);
                }

                // Calculate Horizontal Lines.
                System.Drawing.Point[] pLeft = new System.Drawing.Point[TrainSOM.NUM_NODES_DOWN - 1];
                System.Drawing.Point[] pRight = new System.Drawing.Point[TrainSOM.NUM_NODES_DOWN - 1];
                for (int i = 0; i < TrainSOM.NUM_NODES_DOWN - 1; ++i)
                {
                    pLeft[i] = new System.Drawing.Point(0, (i + 1) * (int)m_appSettings.nodeHeight);
                    pRight[i] = new System.Drawing.Point((int)m_appSettings.width, (i + 1) * (int)m_appSettings.nodeHeight);
                }

                // Render grid.
                for (int i = 0; i < TrainSOM.NUM_NODES_ACROSS - 1; ++i)
                {
                    offscreenWindows.DrawLine(pen0, pTop[i], pBottom[i]);
                    offscreenWindows.DrawLine(pen0, pLeft[i], pRight[i]);
                }

                Font f = new Font("Times New Roman", 9.0f);
                SolidBrush blackBrush = new SolidBrush(Color.Black);
                SolidBrush grayBrush = new SolidBrush(Color.DimGray);

                for (int i = 0; i < TrainSOM.NUM_NODES_DOWN; ++i)
                {
                    for (int j = 0; j < TrainSOM.NUM_NODES_ACROSS; ++j)
                    {
                        
                        if (TrainSOM.win[i * NUM_NODES_ACROSS + j] == 1)//
                        {
                            System.Drawing.Point pSelected = new System.Drawing.Point((i + 1) * (int)m_appSettings.nodeWidth - (int)(1 * m_appSettings.nodeWidth),
                                                                     (j + 1) * (int)m_appSettings.nodeHeight - (int)(0.99 * m_appSettings.nodeHeight));

                            Size sSelected = new Size((int)m_appSettings.nodeWidth, (int)m_appSettings.nodeHeight);
                            Rectangle selectedRect = new Rectangle(pSelected, sSelected);
                            //SolidBrush brush1;
                            //brush1 = new SolidBrush(Color.FromArgb(
                            //   (int)TrainSOM.errorMap[i,j]*255,
                            //   (int)TrainSOM.errorMap[i,j]*255,
                            //   (int)TrainSOM.errorMap[i,j]*255)
                            //    );

                            offscreenWindows.FillRectangle(grayBrush, selectedRect);
                        }
                        if (TrainSOM.pos == (i * NUM_NODES_ACROSS + j))
                        {
                            System.Drawing.Point pSelected = new System.Drawing.Point((i + 1) * (int)m_appSettings.nodeWidth - (int)(1 * m_appSettings.nodeWidth),
                                                                     (j + 1) * (int)m_appSettings.nodeHeight - (int)(.99 * m_appSettings.nodeHeight));

                            Size sSelected = new Size((int)m_appSettings.nodeWidth, (int)m_appSettings.nodeHeight);
                            Rectangle selectedRect = new Rectangle(pSelected, sSelected);
                            SolidBrush brush1 = new SolidBrush(Color.FromArgb(0, 255, 0));

                            offscreenWindows.FillRectangle(brush1, selectedRect);
                        }
                        // Render any selected node, then all of the grid #s (of images) on top.
                        if (m_isNodeSelected)
                        {
                            if (m_selectedNodeCoords.X == i && m_selectedNodeCoords.Y == j)
                            {

                                // Beware: Ugly code ahead. (Accounts for top row and right column being 1 pixel bigger than other cells).
                                int hackX = 1; int hackY = 1;
                                if (i == TrainSOM.NUM_NODES_DOWN - 1) hackX = 0;
                                if (j == 0) hackY = 0;
                                ////////////////////////////////

                                //System.Drawing.Point pSelected = new System.Drawing.Point((int)(m_appSettings.nodeWidth * (0.9) * i) + hackX, (int)(m_appSettings.nodeHeight * (0.9) * j) + hackY);
                                System.Drawing.Point pSelected = new System.Drawing.Point((i + 1) * (int)m_appSettings.nodeWidth - (int)(1 * m_appSettings.nodeWidth),
                                                                     (j + 1) * (int)m_appSettings.nodeHeight - (int)(.99 * m_appSettings.nodeHeight));
                        
                                Size sSelected = new Size((int)m_appSettings.nodeWidth - hackX, (int)m_appSettings.nodeHeight - hackY);
                                Rectangle selectedRect = new Rectangle(pSelected, sSelected);
                                SolidBrush brush1 = new SolidBrush(Color.FromArgb(255, 0, 0));
                                offscreenWindows.FillRectangle(brush1, selectedRect);
                                
                                
                            }
                        }
                        System.Drawing.Point pFont = new System.Drawing.Point((i + 1) * (int)m_appSettings.nodeWidth - (int)(1.1 * m_appSettings.nodeWidth),
                                                                     (j + 1) * (int)m_appSettings.nodeHeight - (int)(1 * m_appSettings.nodeHeight));
                        offscreenWindows.DrawString(map[i*NUM_NODES_ACROSS+j], f, blackBrush, pFont);
                        
                    } // End for each column.
                } // End for each row.

                theWindows.DrawImage(offscreenBitmaps, 0, 0);
                
            }

            public System.Drawing.Point clickInGrid(int xCoord, int yCoord)
            {
                // Yay integer division to the rescue!
                m_selectedNodeCoords.X = xCoord / (int)m_appSettings.nodeWidth;
                m_selectedNodeCoords.Y = yCoord / (int)m_appSettings.nodeHeight;
                m_isNodeSelected = true;

                return new System.Drawing.Point(m_selectedNodeCoords.X, m_selectedNodeCoords.Y);

            }

        }

        private void neuron_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            int x, y;
            int vcell = (sample.Height) / DOWNSAMPLE_HEIGHT;
            int hcell = (sample.Width) / DOWNSAMPLE_WIDTH;
            Brush whiteBrush = new SolidBrush(Color.White);
            Brush blackBrush = new SolidBrush(Color.Black);
            Pen blackPen = new Pen(Color.Black);

            g.FillRectangle(whiteBrush, 0, 0, sample.Width, sample.Height);



            for (y = 0; y < DOWNSAMPLE_HEIGHT; y++)
            {
                g.DrawLine(blackPen, 0, y * vcell, sample.Width, y * vcell);
            }
            for (x = 0; x < DOWNSAMPLE_WIDTH; x++)
            {
                g.DrawLine(blackPen, x * hcell, 0, x * hcell, sample.Height);
            }

            int index = 0;
            for (y = 0; y < DOWNSAMPLE_HEIGHT; y++)
            {
                for (x = 0; x < DOWNSAMPLE_WIDTH; x++)
                {
                    if (this.value[index++] > 0.5)
                    {
                        g.FillRectangle(blackBrush, x * hcell, y * vcell, hcell, vcell);
                    }
                }
            }

            g.DrawRectangle(blackPen, 0, 0, sample.Width - 1, sample.Height - 1);
        }

        private void mouseDown_mapSOM(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {

                if (m_SOM != null)
                {

                    System.Drawing.Point p = m_SOM.clickInGrid(e.X, e.Y);
                    mapCount = p.X * NUM_NODES_ACROSS + p.Y;
                    //MessageBox.Show("" + p.X + " " + p.Y);
                    
                    if (hasNetwork)
                    {
                        Neuron neu = network.Layers[0].Neurons[mapCount];
                        //outPut.Text = "" + " | " + neu.Output;

                        for (int m = 0; m < DOWNSAMPLE_AREA; m++)
                        {
                            value[m] = neu.Weights[m];
                        }
                        outPut.Text = "" + mapCount%NUM_NODES_ACROSS + " | " + mapCount/NUM_NODES_ACROSS + " || " + map[mapCount] + " | " + neu.Win + " | " + win[mapCount] ;
                        
                    }
                    
                }
            }
            Invalidate();
            this.neuron.Invalidate();


        }

        //show neuron by num count
        private void displayNeuron_Click(object sender, EventArgs e)
        {

            if (mapCount > -1)
            {
                if (mapText.Text != null)
                    map[mapCount] = mapText.Text;
                else
                    MessageBox.Show("Input the mapping value for the Neuron");
            }
            else
            {
                MessageBox.Show("Select a Neuron in the Network to map its value");
            }
            //network.Layers[0].Neurons[s] 
        }


        private void saveNetwork_Click(object sender, EventArgs e)
        {
            String strFileName = string.Empty;

            saveNetworkDialog.Filter = "koh files (*.koh)|*.koh|All files (*.*)|*.*";

            //loadTrainDataDialog.InitialDirectory = "..\\..\\";
            saveTrainDataDialog.Title = "Select a Network file";
            saveTrainDataDialog.CheckPathExists = true;
            saveTrainDataDialog.RestoreDirectory = true;
            //Present to the user.
            if (saveNetworkDialog.ShowDialog() == DialogResult.OK)
                strFileName = saveNetworkDialog.FileName;
            if (strFileName == String.Empty)
                return;//user didn't select a file
            
            network.Save(strFileName);
            MessageBox.Show("Saved to " + strFileName);

        }

        private void loadNetwork_Click(object sender, EventArgs e)
        {
            String strFileName = string.Empty;

            //loadNetworkDialog.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";

            //loadTrainDataDialog.InitialDirectory = "..\\..\\";
            loadNetworkDialog.Title = "Select a Network file";

            //Present to the user.
            if (loadNetworkDialog.ShowDialog() == DialogResult.OK)
                strFileName = loadNetworkDialog.FileName;
            if (strFileName == String.Empty)
                return;//user didn't select a file

            network = Network.Load(strFileName);
            //network.MapChar = new string[OUTPUT_COUNT];
            TimeSpan t = TimeSpan.FromSeconds(network.TimeTrain);

            trainTime.Text = string.Format("Processing time: {0}s", t.ToString());
            MessageBox.Show(this, "Loaded from 'sample.dat'.");
            trainButton.Enabled = true;
            hasNetwork = true;
            //numberWin = averageDataConvert();
        }

        private void createNetwork_Click(object sender, EventArgs e)
        {
            network = new Network(DOWNSAMPLE_HEIGHT * DOWNSAMPLE_WIDTH, OUTPUT_COUNT);

            // randomize net
            network.Randomize();
            for (int i = 0; i < map.Length; i++)
                map[i] = "/?";
            //Neuron testNeuron = network.Layers[0].Neurons
            trainButton.Enabled = true;
            saveNetwork.Enabled = true;
            hasNetwork = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        static Random random = new Random();
        private static string [] ShuffleData(string[] arr)
        {
            List<KeyValuePair<int, string>> list = new List<KeyValuePair<int, string>>();
            foreach (string s in arr)
            {
                list.Add(new KeyValuePair<int,string>(random.Next(),s));

            }
            var sorted = from item in list orderby item.Key select item;

            string[] result = new string[arr.Length];

            int index = 0;
            foreach(KeyValuePair<int, string> pair in sorted)
            {
                result[index] = pair.Value;
                index++;
            }
            return result;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {

        }

        private void saveMap_Click(object sender, EventArgs e)
        {
            
            String strFileName = string.Empty;

            saveTrainDataDialog.Filter = "map files (*.map)|*.map|All files (*.*)|*.*";

            //loadTrainDataDialog.InitialDirectory = "..\\..\\";
            saveTrainDataDialog.Title = "Select a Data file";
            saveTrainDataDialog.CheckPathExists = true;
            saveTrainDataDialog.RestoreDirectory = true; 
            //Present to the user.
            if (saveTrainDataDialog.ShowDialog() == DialogResult.OK)
                strFileName = saveTrainDataDialog.FileName;
            if (strFileName == String.Empty)
                return;//user didn't select a file
            
            try
            {
                StreamWriter f = new StreamWriter(strFileName, true);
                
                
                for (int i = 0; i < map.Length; i++)
                {
                    
                    f.WriteLine(win[i] + map[i]);


                }
                f.Close();
                MessageBox.Show("Saved to "  + strFileName + ".dat");


            }
            catch (Exception e2)
            {
                MessageBox.Show("Error: " + e2.Message, "Training");
            }
        }
        
       

        private void loadMap_Click(object sender, EventArgs e)
        {
            
            String strFileName = string.Empty;

            //loadTrainDataDialog.Filter = "dat files (*.dat)|*.dat|All files (*.*)|*.*";

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
                        win[i] = Convert.ToInt16(line[0].ToString());
                        map[i] = line.Substring(1);
                        i++;
                }

                f.Close();
                onReset();
                MessageBox.Show(this, "Loaded from map data");

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        
        

        }

        private void recognize_Click(object sender, EventArgs e)
        {
            int letterCount = this.blobView.Items.Count;
            //this.trainingSet = new double[letterCount][];
            //int index = 0;
            
            //    this.trainingSet[index] = new double[inputCount];
            //    bool[] data = this.data[ch];
            //    for (int i = 0; i < inputCount; i++)
            //    {
            //        this.trainingSet[index][i] = data[i] ? 1 : 0;
            //    }
            //    index++;
            //}
            double[] value = new double[DOWNSAMPLE_AREA];
            int winner = -1;
            string currentSelection = string.Empty;
            //for (int i = 0; i < blobView.Items[0].
            currentSelection = blobView.Items[blobView.SelectedIndices[0]].SubItems[1].Text;
            downsampled = (bool[])data[currentSelection].Clone();
            
                //bool[] data = this.data[ch];                
            for (int i = 0; i < DOWNSAMPLE_AREA; i++)
                {
                    if (downsampled[i])
                        value[i] = (double)1;
                    else
                        value[i] = (double)0;
                }
                double[] ms = network.Compute(value);
                //for (int i = 0; i < ms.Length; i++)
                //    map[i] = Convert.ToString(ms[i]);
                winner = network.GetWinner();
                //textBox1.AppendText(map[winner] + " ");
                //win[winner] = 1;
                pos = winner;
                neuronNum.Text = "" + winner + " " + (int)winner % NUM_NODES_ACROSS + " " + (int)winner / NUM_NODES_ACROSS + " " + map[winner] + " ";
                
        }
        //public static double[,] errorMap;
        ////public double windowsErrorMap()
        //{

        //    // Sum of all the average errors for each node. Can be used to determine a relative
        //    //  effectiveness of a particular mapping.
        //    double totalError = 0.0f;
        //    bool takeSquareRoot = true;
        //    double numWeightSquareRoot = (double)Math.Sqrt((double)DOWNSAMPLE_HEIGHT*DOWNSAMPLE_WIDTH);

        //    // Find the average of all the node distances (add up the 8 surrounding nodes / 8).
        //    for (int i = 0; i < NUM_NODES_DOWN; ++i)
        //    {
        //        for (int j = 0; j < NUM_NODES_ACROSS; ++j)
        //        {
        //            double sumDistance = 0.0f;
        //            double[] centerPoint = network.Layers[0].Neurons[i * NUM_NODES_ACROSS + j].Weights;
        //            int neighborCount = 0;

        //            /*      i-1,j-1 | i-1,j | i-1,j+1 
        //             *      -------------------------
        //             *      i,j-1   |   X   | i,j+1
        //             *      -------------------------
        //             *      i+1,j-1 | i+1,j | i+1,j+1
        //             */

        //            // Top row.
        //            if (i >= 1)
        //            {
        //                // Top left.
        //                if (j >= 1)
        //                {
        //                    sumDistance += util_getDistance(centerPoint, network.Layers[0].Neurons[(i - 1)*NUM_NODES_ACROSS+ (j - 1)].Weights, takeSquareRoot);
        //                    ++neighborCount;
        //                }

        //                // Top middle.
        //                sumDistance += util_getDistance(centerPoint, network.Layers[0].Neurons[(i - 1) * NUM_NODES_ACROSS + (j)].Weights, takeSquareRoot);
        //                ++neighborCount;

        //                // Top right.
        //                if (j < NUM_NODES_ACROSS - 1)
        //                {
        //                    sumDistance += util_getDistance(centerPoint, network.Layers[0].Neurons[(i - 1) * NUM_NODES_ACROSS + (j + 1)].Weights, takeSquareRoot);
        //                    ++neighborCount;
        //                }
        //            }

        //            // Left (1).
        //            if (j >= 1)
        //            {
        //                sumDistance += util_getDistance(centerPoint, network.Layers[0].Neurons[(i) * NUM_NODES_ACROSS + (j - 1)].Weights, takeSquareRoot);
        //                ++neighborCount;
        //            }

        //            // Right (1).
        //            if (j < NUM_NODES_ACROSS - 1)
        //            {
        //                sumDistance += util_getDistance(centerPoint, network.Layers[0].Neurons[(i) * NUM_NODES_ACROSS + (j + 1)].Weights, takeSquareRoot);
        //                ++neighborCount;
        //            }

        //            // Bottom row.
        //            if (i < NUM_NODES_DOWN - 1)
        //            {
        //                // Bottom left.
        //                if (j >= 1)
        //                {
        //                    sumDistance += util_getDistance(centerPoint, network.Layers[0].Neurons[(i + 1) * NUM_NODES_ACROSS + (j - 1)].Weights, takeSquareRoot);
        //                    ++neighborCount;
        //                }

        //                // Bottom middle.
        //                sumDistance += util_getDistance(centerPoint, network.Layers[0].Neurons[(i + 1) * NUM_NODES_ACROSS + (j)].Weights, takeSquareRoot);
        //                ++neighborCount;

        //                // Bottom right.
        //                if (j < NUM_NODES_ACROSS - 1)
        //                {
        //                    sumDistance += util_getDistance(centerPoint, network.Layers[0].Neurons[(i + 1) * NUM_NODES_ACROSS + (j + 1)].Weights, takeSquareRoot);
        //                    ++neighborCount;
        //                }
        //            }

        //            // Compute the average.
        //            double averageDistance = sumDistance / (double)neighborCount;
        //            totalError += averageDistance;

        //            // This produces a nice scale from 0 (black) to 1 (white) for the error map.
        //            //   The max distance possible is when a node is 0.0, and all of it's neighbors
        //            //   have 1.0 weights. This distance, then, would be NUM_WEIGHTS if no square root
        //            //   is taken of the distances, or sqrt( NUM_WEIGHTS ) if the square root is taken.
        //            //   The checks for out of bounds shouldn't be necessary, but are left in for extra
        //            //   precaution.
        //            double scaledDistance;
        //            if (takeSquareRoot)
        //            {
        //                scaledDistance = neighborCount;//numWeightSquareRoot * averageDistance; 
        //            }
        //            else { scaledDistance = (float)SOM_Image_Form.NUM_WEIGHTS * averageDistance; }
        //            if (scaledDistance > 1.0f) { scaledDistance = 1.0f; }
        //            else if (scaledDistance < 0.0f) { scaledDistance = 0.0f; }

        //            // Create a greyscale (i.e. r = g = b).
        //            errorMap[i,j] = scaledDistance;

        //        } // End for each column.
        //    } // End for each row.

        //    return totalError;
        //}


        //private void button1_Click(object sender, EventArgs e)
        //{
        //    windowsErrorMap();
        //    string s = "";
        //    for (int i = 0; i < NUM_NODES_DOWN; ++i)
        //    {
        //        for (int j = 0; j < NUM_NODES_ACROSS; ++j)
        //        {
        //            s += errorMap[i, j] + " ";
        //        }
        //    }
        //    //textBox1.Text = s;
        // }


        private void button2_Click(object sender, EventArgs e)
        {
            if (mapCount > -1)
            {
                if (textBox1.Text != null)
                    win[mapCount] = Convert.ToInt16(textBox1.Text);
                else
                    MessageBox.Show("Input the winning value:");
            }
            else
            {
                MessageBox.Show("Select a Neuron in the Network to map winning value");
            }
        }

        private void iterationLabel_TextChanged(object sender, EventArgs e)
        {

        }

        //private void button3_Click(object sender, EventArgs e)
        //{
        //    //outPut.Text = "" + network.Layers[0].Neurons.Length;
        //    ArrayList list = new ArrayList();
        //    for (int i = 0; i < TrainSOM.OUTPUT_COUNT; i++)
        //    {
        //        list.Add(network.Layers[0].Neurons[i].Win);
        //    }
        //    list.Sort();
        //    list.Reverse();
        //    list.RemoveRange(70, list.Count - 70);
            
        //    for (int i = 0; i <numInput 900; i++)
        //    {
        //        for ( int k = 0; k < list.Count; ++k ) 
        //        {
        //            if (network.Layers[0].Neurons[i].Win == (int)list[k] && (int)list[k] != 0)
        //            {
        //                win[i] = 1;
        //            }
                
        //        }
        //    }
               
        //}

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    for (int i = 0; i < 900; i++)
        //        win[i] = 0;

        //    //Invalidate();
        //}

        private void button5_Click(object sender, EventArgs e)
        {
            //dataMapint
            
            for (int i = 0; i < CharList.Count; i++)
            {
                network.Compute(trainingSet[i]);
                
                //for (int i = 0; i < ms.Length; i++)
                //    map[i] = Convert.ToString(ms[i]);
                int winner = network.GetWinner();
                
                win[winner] = 1;
                map[winner] = CharList[i];

                //inputMap.Text += dataMap[i];
            }
            
        }

        private void inputMapButton_Click(object sender, EventArgs e)
        {
            
            blobView.SelectedItems[0].SubItems[0].Text = inputMap.Text;

            //blobView.SelectedIndices[0] = blobView.SelectedIndices[0] + 1;

                //string stepName = blobView.SelectedItems[0].SubItems[1].Text;
                
                //downsampled = downSample((Bitmap)blobs[stepName].Clone());
                //blobImage.Image = (Bitmap)temp;
                //downsampled = (bool[])data[stepName].Clone();
            
        }

        public static double util_getDistance(double[] p1, double[] p2, bool squareRoot)
        {
            
            double distance = 0.0f;

            for (int i = 0; i < p1.Length; ++i)
            {
                distance += (p1[i] - p2[i])
                            *(p1[i] - p2[i]);
            }

           if (squareRoot)
              return (double)Math.Sqrt(distance);
           else
                return distance;
        }
        private void restMapButton_Click(object sender, EventArgs e)
        {
            restMapButton.Enabled = false;
            for (int i = 0; i < OUTPUT_COUNT; i++)
            {
                if(win[i] == 0)
                {
                    int pos = -1;
                    double min = 999999;
                    for(int j = 0; j < OUTPUT_COUNT; j++)
                    {
                        double distance = 0;
                        if(win[j] == 1)
                        {
                            distance += TrainSOM.util_getDistance(network.Layers[0].Neurons[i].Weights,
                                    network.Layers[0].Neurons[j].Weights, true);
                            if (distance < min)
                            {
                                min = distance;
                                pos = j;
                            }
                        }
                    }
                    if (pos > -1)
                    {
                        map[i] = map[pos];
                        inputMap.Text = "" + min;
                    }
                }
                
            }
            restMapButton.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void errorCheckButton_Click(object sender, EventArgs e)
        {

            //LOdouble [] error = new double [NUM_NODES_ACROSS*NUM_NODES_DOWN];
            
            for (int i = 0; i < NUM_NODES_ACROSS - 1; i++)
            {
                for (int j = 0; j < NUM_NODES_DOWN; j++)
                {
                    double distance = 0;
                    distance += TrainSOM.util_getDistance(network.Layers[0].Neurons[i * NUM_NODES_ACROSS + j].Weights,
                                network.Layers[0].Neurons[(i + 1) * NUM_NODES_ACROSS + j].Weights, true);
                    map[i * NUM_NODES_ACROSS + j] = Convert.ToString(distance).Substring(0, 3) ;


                }
            }
            

        }

        private void ColumnErrorCheck_Click(object sender, EventArgs e)
        {
            //double[] error = new double[NUM_NODES_ACROSS * NUM_NODES_DOWN];

            for (int i = 0; i < NUM_NODES_ACROSS; i++)
            {
                for (int j = 0; j < NUM_NODES_DOWN - 1; j++)
                {
                    double distance = 0;
                    distance += TrainSOM.util_getDistance(network.Layers[0].Neurons[i * NUM_NODES_ACROSS + j].Weights,
                                network.Layers[0].Neurons[i * NUM_NODES_ACROSS + j + 1].Weights, true);
                    map[i * NUM_NODES_ACROSS + j] = Convert.ToString(distance).Substring(0, 3);


                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

        }


}

        

        //private void timer_tick(object sender, System.EventArgs e)
        //{
        //    Invalidate();
        //}
     }



        
        

        

        

        
      

     

        

        
        
    

