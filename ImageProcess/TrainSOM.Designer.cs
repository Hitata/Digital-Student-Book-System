namespace ImageProcess
{
    partial class TrainSOM
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.inputMap = new System.Windows.Forms.TextBox();
            this.sample = new System.Windows.Forms.Panel();
            this.blobView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.loadTrainDataDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveTrainDataDialog = new System.Windows.Forms.SaveFileDialog();
            this.cancelButton = new System.Windows.Forms.Button();
            this.selectButton = new System.Windows.Forms.Button();
            this.loadNetworkDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveNetworkDialog = new System.Windows.Forms.SaveFileDialog();
            this.SomStatus = new System.Windows.Forms.StatusStrip();
            this.numInput = new System.Windows.Forms.ToolStripStatusLabel();
            this.kohonenNum = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.trainTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.neuronNum = new System.Windows.Forms.TextBox();
            this.iterationLabel = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radiusBox = new System.Windows.Forms.TextBox();
            this.rateBox = new System.Windows.Forms.TextBox();
            this.iterationsBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.stopButton = new System.Windows.Forms.Button();
            this.trainButton = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.mapSOM = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.createNetwork = new System.Windows.Forms.Button();
            this.loadNetwork = new System.Windows.Forms.Button();
            this.saveNetwork = new System.Windows.Forms.Button();
            this.outPut = new System.Windows.Forms.TextBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.inputMapButton = new System.Windows.Forms.Button();
            this.deleteData = new System.Windows.Forms.Button();
            this.loadData = new System.Windows.Forms.Button();
            this.saveData = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.loadMap = new System.Windows.Forms.Button();
            this.saveMap = new System.Windows.Forms.Button();
            this.mapText = new System.Windows.Forms.TextBox();
            this.displayNeuron = new System.Windows.Forms.Button();
            this.neuron = new System.Windows.Forms.Panel();
            this.recognize = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.restMapButton = new System.Windows.Forms.Button();
            this.RowErrorCheckButton = new System.Windows.Forms.Button();
            this.ColumnErrorCheck = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SomStatus.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.inputMap);
            this.groupBox1.Controls.Add(this.sample);
            this.groupBox1.Controls.Add(this.blobView);
            this.groupBox1.Location = new System.Drawing.Point(12, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(226, 176);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trainning Input";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(15, 147);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 23);
            this.button1.TabIndex = 40;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // inputMap
            // 
            this.inputMap.Location = new System.Drawing.Point(15, 122);
            this.inputMap.Name = "inputMap";
            this.inputMap.Size = new System.Drawing.Size(90, 20);
            this.inputMap.TabIndex = 3;
            // 
            // sample
            // 
            this.sample.Location = new System.Drawing.Point(15, 19);
            this.sample.Name = "sample";
            this.sample.Size = new System.Drawing.Size(90, 100);
            this.sample.TabIndex = 2;
            this.sample.Paint += new System.Windows.Forms.PaintEventHandler(this.sample_Paint);
            // 
            // blobView
            // 
            this.blobView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.blobView.FullRowSelect = true;
            this.blobView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.blobView.HideSelection = false;
            this.blobView.LabelEdit = true;
            this.blobView.Location = new System.Drawing.Point(117, 19);
            this.blobView.MultiSelect = false;
            this.blobView.Name = "blobView";
            this.blobView.Size = new System.Drawing.Size(97, 141);
            this.blobView.TabIndex = 0;
            this.blobView.UseCompatibleStateImageBehavior = false;
            this.blobView.View = System.Windows.Forms.View.Details;
            this.blobView.SelectedIndexChanged += new System.EventHandler(this.blobView_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Blobs";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Mapped";
            // 
            // loadTrainDataDialog
            // 
            this.loadTrainDataDialog.FileName = "loadTrainDataDialog";
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(1164, 441);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 33);
            this.cancelButton.TabIndex = 7;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // selectButton
            // 
            this.selectButton.Location = new System.Drawing.Point(1065, 441);
            this.selectButton.Name = "selectButton";
            this.selectButton.Size = new System.Drawing.Size(75, 33);
            this.selectButton.TabIndex = 8;
            this.selectButton.Text = "Accept";
            this.selectButton.UseVisualStyleBackColor = true;
            // 
            // loadNetworkDialog
            // 
            this.loadNetworkDialog.FileName = "openFileDialog1";
            // 
            // SomStatus
            // 
            this.SomStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.numInput,
            this.kohonenNum,
            this.toolStripProgressBar1,
            this.trainTime});
            this.SomStatus.Location = new System.Drawing.Point(0, 668);
            this.SomStatus.Name = "SomStatus";
            this.SomStatus.Size = new System.Drawing.Size(1303, 24);
            this.SomStatus.TabIndex = 21;
            this.SomStatus.Text = "statusStrip1";
            // 
            // numInput
            // 
            this.numInput.AutoSize = false;
            this.numInput.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.numInput.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
            this.numInput.Name = "numInput";
            this.numInput.Size = new System.Drawing.Size(122, 19);
            this.numInput.Text = "Input num: 476";
            this.numInput.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // kohonenNum
            // 
            this.kohonenNum.Name = "kohonenNum";
            this.kohonenNum.Size = new System.Drawing.Size(110, 19);
            this.kohonenNum.Text = "Network Size: 20x20";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 18);
            // 
            // trainTime
            // 
            this.trainTime.Name = "trainTime";
            this.trainTime.Size = new System.Drawing.Size(0, 19);
            // 
            // neuronNum
            // 
            this.neuronNum.Location = new System.Drawing.Point(112, 75);
            this.neuronNum.Name = "neuronNum";
            this.neuronNum.ReadOnly = true;
            this.neuronNum.Size = new System.Drawing.Size(102, 20);
            this.neuronNum.TabIndex = 23;
            // 
            // iterationLabel
            // 
            this.iterationLabel.Location = new System.Drawing.Point(1065, 285);
            this.iterationLabel.Name = "iterationLabel";
            this.iterationLabel.ReadOnly = true;
            this.iterationLabel.Size = new System.Drawing.Size(98, 20);
            this.iterationLabel.TabIndex = 24;
            this.iterationLabel.TextChanged += new System.EventHandler(this.iterationLabel_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.radiusBox);
            this.groupBox2.Controls.Add(this.rateBox);
            this.groupBox2.Controls.Add(this.iterationsBox);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.stopButton);
            this.groupBox2.Controls.Add(this.trainButton);
            this.groupBox2.Location = new System.Drawing.Point(1065, 21);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(226, 142);
            this.groupBox2.TabIndex = 24;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Training";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 13);
            this.label4.TabIndex = 40;
            this.label4.Text = "Learning radius";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(56, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 39;
            this.label3.Text = "Learning rate";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(80, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 38;
            this.label1.Text = "Iteration";
            // 
            // radiusBox
            // 
            this.radiusBox.Location = new System.Drawing.Point(137, 75);
            this.radiusBox.Name = "radiusBox";
            this.radiusBox.Size = new System.Drawing.Size(83, 20);
            this.radiusBox.TabIndex = 37;
            this.radiusBox.Text = "10";
            this.radiusBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // rateBox
            // 
            this.rateBox.Location = new System.Drawing.Point(137, 46);
            this.rateBox.Name = "rateBox";
            this.rateBox.Size = new System.Drawing.Size(83, 20);
            this.rateBox.TabIndex = 36;
            this.rateBox.Text = "0.1";
            this.rateBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // iterationsBox
            // 
            this.iterationsBox.Location = new System.Drawing.Point(137, 17);
            this.iterationsBox.Name = "iterationsBox";
            this.iterationsBox.Size = new System.Drawing.Size(83, 20);
            this.iterationsBox.TabIndex = 35;
            this.iterationsBox.Text = "5000";
            this.iterationsBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 134);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(0, 13);
            this.label2.TabIndex = 34;
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(145, 102);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 30);
            this.stopButton.TabIndex = 33;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // trainButton
            // 
            this.trainButton.Location = new System.Drawing.Point(32, 102);
            this.trainButton.Name = "trainButton";
            this.trainButton.Size = new System.Drawing.Size(97, 30);
            this.trainButton.TabIndex = 32;
            this.trainButton.Text = "Train";
            this.trainButton.UseVisualStyleBackColor = true;
            this.trainButton.Click += new System.EventHandler(this.trainButton_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.mapSOM);
            this.groupBox3.Location = new System.Drawing.Point(244, 21);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(815, 635);
            this.groupBox3.TabIndex = 25;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "KohonenNetwork";
            // 
            // mapSOM
            // 
            this.mapSOM.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.mapSOM.Location = new System.Drawing.Point(8, 19);
            this.mapSOM.Name = "mapSOM";
            this.mapSOM.Size = new System.Drawing.Size(800, 600);
            this.mapSOM.TabIndex = 32;
            this.mapSOM.MouseDown += new System.Windows.Forms.MouseEventHandler(this.mouseDown_mapSOM);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.createNetwork);
            this.groupBox4.Controls.Add(this.loadNetwork);
            this.groupBox4.Controls.Add(this.saveNetwork);
            this.groupBox4.Location = new System.Drawing.Point(1065, 168);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(226, 97);
            this.groupBox4.TabIndex = 26;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Network";
            // 
            // createNetwork
            // 
            this.createNetwork.Location = new System.Drawing.Point(13, 19);
            this.createNetwork.Name = "createNetwork";
            this.createNetwork.Size = new System.Drawing.Size(96, 29);
            this.createNetwork.TabIndex = 23;
            this.createNetwork.Text = "createNetwork";
            this.createNetwork.UseVisualStyleBackColor = true;
            this.createNetwork.Click += new System.EventHandler(this.createNetwork_Click);
            // 
            // loadNetwork
            // 
            this.loadNetwork.Location = new System.Drawing.Point(123, 19);
            this.loadNetwork.Name = "loadNetwork";
            this.loadNetwork.Size = new System.Drawing.Size(81, 23);
            this.loadNetwork.TabIndex = 22;
            this.loadNetwork.Text = "loadNetwork";
            this.loadNetwork.UseVisualStyleBackColor = true;
            this.loadNetwork.Click += new System.EventHandler(this.loadNetwork_Click);
            // 
            // saveNetwork
            // 
            this.saveNetwork.Location = new System.Drawing.Point(123, 51);
            this.saveNetwork.Name = "saveNetwork";
            this.saveNetwork.Size = new System.Drawing.Size(81, 23);
            this.saveNetwork.TabIndex = 21;
            this.saveNetwork.Text = "saveNetwork";
            this.saveNetwork.UseVisualStyleBackColor = true;
            this.saveNetwork.Click += new System.EventHandler(this.saveNetwork_Click);
            // 
            // outPut
            // 
            this.outPut.Location = new System.Drawing.Point(112, 32);
            this.outPut.Name = "outPut";
            this.outPut.ReadOnly = true;
            this.outPut.Size = new System.Drawing.Size(102, 20);
            this.outPut.TabIndex = 29;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.inputMapButton);
            this.groupBox6.Controls.Add(this.deleteData);
            this.groupBox6.Controls.Add(this.loadData);
            this.groupBox6.Controls.Add(this.saveData);
            this.groupBox6.Location = new System.Drawing.Point(12, 203);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(226, 79);
            this.groupBox6.TabIndex = 30;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Edit Input";
            // 
            // inputMapButton
            // 
            this.inputMapButton.Location = new System.Drawing.Point(15, 20);
            this.inputMapButton.Name = "inputMapButton";
            this.inputMapButton.Size = new System.Drawing.Size(90, 23);
            this.inputMapButton.TabIndex = 8;
            this.inputMapButton.Text = "mapData";
            this.inputMapButton.UseVisualStyleBackColor = true;
            this.inputMapButton.Click += new System.EventHandler(this.inputMapButton_Click);
            // 
            // deleteData
            // 
            this.deleteData.Location = new System.Drawing.Point(117, 19);
            this.deleteData.Name = "deleteData";
            this.deleteData.Size = new System.Drawing.Size(90, 23);
            this.deleteData.TabIndex = 7;
            this.deleteData.Text = "deleteData";
            this.deleteData.UseVisualStyleBackColor = true;
            this.deleteData.Click += new System.EventHandler(this.deleteData_Click);
            // 
            // loadData
            // 
            this.loadData.Location = new System.Drawing.Point(117, 51);
            this.loadData.Name = "loadData";
            this.loadData.Size = new System.Drawing.Size(90, 23);
            this.loadData.TabIndex = 6;
            this.loadData.Text = "loadData";
            this.loadData.UseVisualStyleBackColor = true;
            this.loadData.Click += new System.EventHandler(this.loadData_Click);
            // 
            // saveData
            // 
            this.saveData.Location = new System.Drawing.Point(15, 51);
            this.saveData.Name = "saveData";
            this.saveData.Size = new System.Drawing.Size(90, 23);
            this.saveData.TabIndex = 5;
            this.saveData.Text = "saveData";
            this.saveData.UseVisualStyleBackColor = true;
            this.saveData.Click += new System.EventHandler(this.saveData_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button2);
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Controls.Add(this.label6);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Controls.Add(this.loadMap);
            this.groupBox5.Controls.Add(this.saveMap);
            this.groupBox5.Controls.Add(this.mapText);
            this.groupBox5.Controls.Add(this.displayNeuron);
            this.groupBox5.Controls.Add(this.neuron);
            this.groupBox5.Controls.Add(this.outPut);
            this.groupBox5.Controls.Add(this.neuronNum);
            this.groupBox5.Location = new System.Drawing.Point(12, 288);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(226, 195);
            this.groupBox5.TabIndex = 31;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "NeuronMapping";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(162, 103);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(52, 23);
            this.button2.TabIndex = 37;
            this.button2.Text = "win";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(113, 105);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(41, 20);
            this.textBox1.TabIndex = 36;
            this.textBox1.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(122, 59);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 13);
            this.label6.TabIndex = 35;
            this.label6.Text = "label6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(119, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 34;
            this.label5.Text = "label5";
            // 
            // loadMap
            // 
            this.loadMap.Location = new System.Drawing.Point(122, 166);
            this.loadMap.Name = "loadMap";
            this.loadMap.Size = new System.Drawing.Size(85, 23);
            this.loadMap.TabIndex = 33;
            this.loadMap.Text = "loadMap";
            this.loadMap.UseVisualStyleBackColor = true;
            this.loadMap.Click += new System.EventHandler(this.loadMap_Click);
            // 
            // saveMap
            // 
            this.saveMap.Location = new System.Drawing.Point(122, 135);
            this.saveMap.Name = "saveMap";
            this.saveMap.Size = new System.Drawing.Size(85, 23);
            this.saveMap.TabIndex = 32;
            this.saveMap.Text = "saveMap";
            this.saveMap.UseVisualStyleBackColor = true;
            this.saveMap.Click += new System.EventHandler(this.saveMap_Click);
            // 
            // mapText
            // 
            this.mapText.Location = new System.Drawing.Point(8, 137);
            this.mapText.Name = "mapText";
            this.mapText.Size = new System.Drawing.Size(88, 20);
            this.mapText.TabIndex = 31;
            // 
            // displayNeuron
            // 
            this.displayNeuron.Location = new System.Drawing.Point(8, 166);
            this.displayNeuron.Name = "displayNeuron";
            this.displayNeuron.Size = new System.Drawing.Size(88, 23);
            this.displayNeuron.TabIndex = 30;
            this.displayNeuron.Text = "map";
            this.displayNeuron.UseVisualStyleBackColor = true;
            this.displayNeuron.Click += new System.EventHandler(this.displayNeuron_Click);
            // 
            // neuron
            // 
            this.neuron.Location = new System.Drawing.Point(6, 19);
            this.neuron.Name = "neuron";
            this.neuron.Size = new System.Drawing.Size(90, 100);
            this.neuron.TabIndex = 28;
            this.neuron.Paint += new System.Windows.Forms.PaintEventHandler(this.neuron_Paint);
            // 
            // recognize
            // 
            this.recognize.Location = new System.Drawing.Point(1065, 317);
            this.recognize.Name = "recognize";
            this.recognize.Size = new System.Drawing.Size(90, 29);
            this.recognize.TabIndex = 32;
            this.recognize.Text = "recognize";
            this.recognize.UseVisualStyleBackColor = true;
            this.recognize.Click += new System.EventHandler(this.recognize_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1188, 282);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(89, 23);
            this.button5.TabIndex = 36;
            this.button5.Text = "WinnerMap";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // restMapButton
            // 
            this.restMapButton.Location = new System.Drawing.Point(1188, 317);
            this.restMapButton.Name = "restMapButton";
            this.restMapButton.Size = new System.Drawing.Size(89, 23);
            this.restMapButton.TabIndex = 37;
            this.restMapButton.Text = "mapRemaining";
            this.restMapButton.UseVisualStyleBackColor = true;
            this.restMapButton.Click += new System.EventHandler(this.restMapButton_Click);
            // 
            // RowErrorCheckButton
            // 
            this.RowErrorCheckButton.Location = new System.Drawing.Point(1065, 387);
            this.RowErrorCheckButton.Name = "RowErrorCheckButton";
            this.RowErrorCheckButton.Size = new System.Drawing.Size(75, 23);
            this.RowErrorCheckButton.TabIndex = 38;
            this.RowErrorCheckButton.Text = "RowError";
            this.RowErrorCheckButton.UseVisualStyleBackColor = true;
            this.RowErrorCheckButton.Click += new System.EventHandler(this.errorCheckButton_Click);
            // 
            // ColumnErrorCheck
            // 
            this.ColumnErrorCheck.Location = new System.Drawing.Point(1146, 387);
            this.ColumnErrorCheck.Name = "ColumnErrorCheck";
            this.ColumnErrorCheck.Size = new System.Drawing.Size(75, 23);
            this.ColumnErrorCheck.TabIndex = 39;
            this.ColumnErrorCheck.Text = "ColumnError";
            this.ColumnErrorCheck.UseVisualStyleBackColor = true;
            this.ColumnErrorCheck.Click += new System.EventHandler(this.ColumnErrorCheck_Click);
            // 
            // TrainSOM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1303, 692);
            this.Controls.Add(this.ColumnErrorCheck);
            this.Controls.Add(this.RowErrorCheckButton);
            this.Controls.Add(this.restMapButton);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.iterationLabel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.recognize);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.SomStatus);
            this.Controls.Add(this.selectButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.groupBox1);
            this.Name = "TrainSOM";
            this.Text = "Trainning Kohonen Self-Organizing Maps";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.SomStatus.ResumeLayout(false);
            this.SomStatus.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Panel sample;
        private System.Windows.Forms.ListView blobView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.OpenFileDialog loadTrainDataDialog;
        private System.Windows.Forms.SaveFileDialog saveTrainDataDialog;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button selectButton;
        private System.Windows.Forms.OpenFileDialog loadNetworkDialog;
        private System.Windows.Forms.SaveFileDialog saveNetworkDialog;
        private System.Windows.Forms.StatusStrip SomStatus;
        private System.Windows.Forms.ToolStripStatusLabel numInput;
        private System.Windows.Forms.ToolStripStatusLabel kohonenNum;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.TextBox neuronNum;
        private System.Windows.Forms.TextBox iterationLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox radiusBox;
        private System.Windows.Forms.TextBox rateBox;
        private System.Windows.Forms.TextBox iterationsBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button trainButton;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button createNetwork;
        private System.Windows.Forms.Button loadNetwork;
        private System.Windows.Forms.Button saveNetwork;
        private System.Windows.Forms.TextBox outPut;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button deleteData;
        private System.Windows.Forms.Button loadData;
        private System.Windows.Forms.Button saveData;
        private System.Windows.Forms.Panel mapSOM;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button loadMap;
        private System.Windows.Forms.Button saveMap;
        private System.Windows.Forms.TextBox mapText;
        private System.Windows.Forms.Button displayNeuron;
        private System.Windows.Forms.Panel neuron;
        private System.Windows.Forms.Button recognize;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox inputMap;
        private System.Windows.Forms.Button inputMapButton;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button restMapButton;
        private System.Windows.Forms.Button RowErrorCheckButton;
        private System.Windows.Forms.Button ColumnErrorCheck;
        private System.Windows.Forms.ToolStripStatusLabel trainTime;
        private System.Windows.Forms.Button button1;
        
        
    }
}