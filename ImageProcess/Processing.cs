// Sample of IPPrototyper usage

// AForge.NET framework
// http://www.aforgenet.com/framework/
//
// Copyright © AForge.NET, 2007-2011
// contacts@aforgenet.com
//

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AForge;
using AForge.Math.Geometry;
using AForge.Imaging;
using AForge.Imaging.Filters;
using System.IO;




namespace ImageProcessing
{
    public class Processing //: IImageProcessingRoutine
    {
        //Fix InputData size
        const int DOWNSAMPLE_HEIGHT = 15;
        const int DOWNSAMPLE_WIDTH = 10;
        //Blob stored
        public  Dictionary<string, Bitmap> blobs = new Dictionary<string, Bitmap>(); //1.1.1.1, 1.1.1.2
        public  Dictionary<string, Bitmap> columnBinImage = new Dictionary<string, Bitmap>(); //1.1 - 3.30

        public  Dictionary<string, Bitmap> blobsSquare = new Dictionary<string, Bitmap>(); //1.1.1 - 3.30.5
        public  Dictionary<string, bool[]> inputData = new Dictionary<string, bool[]>(); 
        // Image processing routine's name
        public int thesValue;
        public bool isThresh;
        public Bitmap[] blobArray = new Bitmap[10];
        public Bitmap[] column = new Bitmap[10];

        private Bitmap blankImage;
        public void drawBlankImage ()
        {
            blankImage = new Bitmap(25, 60);
            using (Graphics gfx = Graphics.FromImage(blankImage))
                using (SolidBrush brush = new SolidBrush(Color.White))
                {
                    gfx.FillRectangle(brush, 0, 0, 25, 60);
                }
        }

        public Processing()
        {
            isThresh = false;
            thesValue = 128;

        }
        public void getInputData()
        {
            
            //int i = 0;
            foreach (KeyValuePair<string, Bitmap> pair in blobs)
            {
                DownSample ds = new DownSample(pair.Value);
                inputData.Add(pair.Key, ds.downSample(DOWNSAMPLE_WIDTH, DOWNSAMPLE_HEIGHT));
                //i++;

            }
        }
        
        
        public string Name
        {
            get { return "IPPrototyper Sample"; }
        }

        // Process specified image trying to recognize counter's image
        public void Process( Bitmap image, IImageProcessingLog log )
        {
            drawBlankImage();
            this.blobs.Clear();
            this.blobsSquare.Clear();
            this.inputData.Clear();
            log.AddMessage( "Image size: " + image.Width + " x " + image.Height );

            //grayscale and binirazation
            Bitmap grayImage = Grayscale.CommonAlgorithms.BT709.Apply(image);
            BradleyLocalThresholding threshold = new BradleyLocalThresholding();
            Bitmap binImage = threshold.Apply(grayImage);
            log.AddImage("Binary", binImage);
            log.AddMessage("image binarization");

            DocumentSkewChecker skewChecker = new DocumentSkewChecker();
            // get documents skew angle
            double angle = skewChecker.GetSkewAngle(binImage);
            // create rotation filter
            RotateBilinear rotationFilter = new RotateBilinear(-angle);
            rotationFilter.FillColor = Color.White;
            // rotate image applying the filter
            Bitmap rotatedImage = rotationFilter.Apply(binImage);
            log.AddImage("skew", rotatedImage);
            

            Invert invert = new Invert();
            Bitmap invertImage = invert.Apply(rotatedImage);
            log.AddImage("invert", invertImage);

            //save row of instances
            
            //Dictionary<string, Bitmap> cells = new Dictionary<string, Bitmap>();


            //extract the table big blob
            BlobCounter blobCounter = new BlobCounter();
            blobCounter.FilterBlobs = true;
            //blobCounter.MinHeight = binImage.Height;
            blobCounter.MinWidth = rotatedImage.Width / 4;
            blobCounter.ProcessImage(invertImage);
            Rectangle[] recs = blobCounter.GetObjectsRectangles();
            //Blob[] blobs = blobCounter.GetObjectsInformation();

            int s = 0;
            foreach (Rectangle rec in recs)
            {
                // extractig blob
                //blobCounter.ExtractBlobsImage(binImage, blob, false);
                //blobArray[i] = blob.Image.ToManagedImage();
                //Crop rectable edge

                //show each table
                Crop cr = new Crop(rec);
                blobArray[s] = cr.Apply(rotatedImage);
                log.AddImage("image: " + s, blobArray[s]);

                
                Crop cro = new Crop(new Rectangle(0, 0, blobArray[s].Width / 30, blobArray[s].Height));
                Bitmap columnCrop = cro.Apply(blobArray[s]);
                log.AddImage("Row segmentation" + s, columnCrop);


                //Cut column
                BlobCounter blobCount = new BlobCounter();
                blobCount.FilterBlobs = true;
                blobCount.MinWidth = columnCrop.Width*8/10;
                blobCount.MaxHeight = columnCrop.Height/3;
                //blobCount.MinHeight = columnCrop.Height * 8 / 10;
                blobCount.ObjectsOrder = ObjectsOrder.YX;
                blobCount.ProcessImage(columnCrop);
                Rectangle[] recCol = blobCount.GetObjectsRectangles();
                int t = 0;
                foreach (Rectangle rect in recCol)
                {
                    //extractig blob
                    //blobCounter.ExtractBlobsImage(binImage, blob, false);
                    //blobArray[i] = blob.Image.ToManagedImage();
                    //Crop rectable edge


                    log.AddMessage(string.Format("row height: {0} X n Y of: {1}, {2}", rect.Height, rect.X, rect.Y));

                    //Crop Column Out from binImage
                    Crop crCol = new Crop(new Rectangle(rect.X, rect.Y, blobArray[s].Width, rect.Height));
                    columnBinImage.Add((s+1).ToString() + "." + (t+1).ToString(),crCol.Apply(blobArray[s]));
                    //log.AddImage("Row instance: " + columnBinImage., columnBinImage.);
                    t++;
                }
                foreach (KeyValuePair<string, Bitmap> row in columnBinImage)
                {
                    log.AddImage("Row instance: " + row.Key, row.Value);
                }
                s++;
            }
            //log.AddImage("image 2.2", columnBinImage["2.2"]);
            //log.AddImage("image 3.2", columnBinImage["3.2"]);

                    ////each blob segment
            foreach (KeyValuePair<string, Bitmap> row in columnBinImage)
            {

                BlobCounter rowBlob = new BlobCounter();
                rowBlob.FilterBlobs = true;
                rowBlob.MinHeight = row.Value.Height * 8 / 10;
                rowBlob.MinWidth = row.Value.Height;
                rowBlob.MaxWidth = row.Value.Height*3;
                rowBlob.ObjectsOrder = ObjectsOrder.XY;

                rowBlob.ProcessImage(row.Value);
                Blob[] cellImage = rowBlob.GetObjectsInformation();
                int p = 0;
                foreach (Blob cell in cellImage)
                {
                    rowBlob.ExtractBlobsImage(row.Value, cell, false);
                    //Bitmap sm = cell.Image.ToManagedImage();
                    this.blobsSquare.Add(row.Key + "." + (p + 1).ToString(), cell.Image.ToManagedImage());
                    //log.AddImage("" + p, sm);
                    p++;
                }
                
            }

            Dictionary<string, Bitmap> tmpBit = new Dictionary<string,Bitmap>();

            foreach (KeyValuePair<string, Bitmap> blob in this.blobsSquare)
            {

                

                Invert inp = new Invert();
                inp.ApplyInPlace(blob.Value);
                removeOutLine(blob.Value);
                BlobCounter bl = new BlobCounter();
                        bl.FilterBlobs = true;
                        bl.MinHeight = blob.Value.Height / 2;
                        //bl.MinWidth = cells[0].Width / 3;
                        //bl.MaxHeight = blob.Value.Height * 9 / 10;
                        bl.ObjectsOrder = ObjectsOrder.XY;
                        
                        bl.ProcessImage(blob.Value);

                        Blob[] go = bl.GetObjectsInformation();
                        
                        int ce = 1;
                        foreach (Blob g in go)
                        {
                                bl.ExtractBlobsImage(blob.Value, g, false);
                                tmpBit.Add(blob.Key + "."  + ce.ToString(), Thinning(g.Image.ToManagedImage()));
                                
                                //log.AddImage("blob" + ce, gb[ce]);
                                ce++;
                        }
                            
            }

            //log.AddImage("Image square blob", this.blobsSquare["1.2.3"]);
            //Bitmap temp =  this.blobsSquare["1.2.3"];

            //Invert invt = new Invert();
            //Bitmap ebbimage = invt.Apply(this.blobsSquare["3.2.1"]);
            
            
            foreach (KeyValuePair<string, Bitmap> row in tmpBit)
            {
                //SimpleSkeletonization sk = new SimpleSkeletonization();
                
                Invert inCu = new Invert();
                
                //sk.ApplyInPlace(row.Value);
                //inCu.ApplyInPlace(row.Value);
                //Bitmap tmp =  Thinning(row.Value);
                
                
                this.blobs.Add(row.Key, row.Value);
                //row.Value.Save(row.Key + ".bmp");
               
            }

            //this.blobs["3.2.2.1"].Save("damnuthinning2.bmp");
            //rotatedImage.Save("rotatetoat.bmp");
                    //{
             getInputData();

            
                  
                        
                    //}

        }

        private void removeOutLine(Bitmap image)
        {
            Bitmap temp = new Bitmap(image.Width, image.Height);
            Graphics g = Graphics.FromImage(temp);
            g.FillRectangle(new SolidBrush(Color.Black), new Rectangle(0, 0, temp.Width, temp.Height));
            g.DrawRectangle(new Pen(Color.White, 4), 0, 0, temp.Width, temp.Height);
           
            //ExtractBiggestBlob ebb = new ExtractBiggestBlob();
            //Bitmap ebbimage = ebb.Apply(temp);
            Bitmap tempformat = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), image.PixelFormat);
            Subtract sub = new Subtract(tempformat);
            sub.ApplyInPlace(image);
            Threshold thres = new Threshold();
            thres.ApplyInPlace(image);
            
        }
                
                //log.AddMessage(string.Format("bob number: {0} height n width of: {1}, {2}", blob.ID, blob.Rectangle.Width, blob.Rectangle.Height));

            

            //Display 2 table
            
            //log.AddImage("image: 2", blobArray[1]);




            //Row segment
            

          

            
                
                
                
         
            //dt = new DataTable();
            //if (blobs[0] != null)
            //{
            //    Bitmap finalImage = blobs[0].Image.ToManagedImage();
            //    log.AddImage("Final", finalImage);
            //}

       

        public static Bitmap CreateNoneIndexedImage(Bitmap original)
        {
            Bitmap image = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb );
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.DrawImage(original, 0, 0);
            }
            return image;
        }


        public Bitmap Thinning(Bitmap image)
        {
            Threshold st = new Threshold();
                
            FiltersSequence filterSeq = new FiltersSequence();
            filterSeq.Add(new HitAndMiss(new short[,] { { 0, 0, 0 }, { -1, 1, -1 }, { 1, 1, 1 } }, HitAndMiss.Modes.Thinning));
            filterSeq.Add(new AForge.Imaging.Filters.HitAndMiss(
    new short[,] { { -1, 0, 0 }, { 1, 1, 0 }, { -1, 1, -1 } },
    HitAndMiss.Modes.Thinning));
            filterSeq.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, -1, 0 }, { 1, 1, 0 }, { 1, -1, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSeq.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 1, 1, 0 }, { -1, 0, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSeq.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 1, 1, 1 }, { -1, 1, -1 }, { 0, 0, 0 } },
                HitAndMiss.Modes.Thinning));
            filterSeq.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { -1, 1, -1 }, { 0, 1, 1 }, { 0, 0, -1 } },
                HitAndMiss.Modes.Thinning));
            filterSeq.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, -1, 1 }, { 0, 1, 1 }, { 0, -1, 1 } },
                HitAndMiss.Modes.Thinning));
            filterSeq.Add(new AForge.Imaging.Filters.HitAndMiss(
                new short[,] { { 0, 0, -1 }, { 0, 1, 1 }, { -1, 1, -1 } },
                HitAndMiss.Modes.Thinning));
            FilterIterator inter = new FilterIterator(filterSeq, 1);
            
            st.ApplyInPlace(image);
            Bitmap newImage = inter.Apply(image);

            Invert inv = new Invert();
            inv.ApplyInPlace(newImage);
            return newImage;
        }
 

        

    }
}
