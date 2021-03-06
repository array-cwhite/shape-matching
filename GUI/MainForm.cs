﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Filmstrip;
using Adaption;
using System.Drawing.Imaging;
using System.Threading;

namespace GUI
{
    public delegate void AlgoInvoker();

    public partial class MainForm : Form
    {
        private AlgoFactory     m_Factory;
        private ICData          m_CurrAlgoResult;
        private IMatchingAlgo   m_CurrMatchingAlgo;
        private string          m_CurrAlgorithmAlias;
        private string          m_CurrTargetAlias;

        private CPCA                m_currPCAalgo;
        private CHausdorffDistance  m_currHausdorffalgo;
        private CShapeContext       m_currShapeContextalgo;
        private CComboAlgorithm     m_currPipedalgo;
        public MainForm()
        {
            InitializeComponent();
            List<Image> sourceImages = getFolderImages(Properties.Settings.Default.SourceImagesFolder);
            List<Image> targetImages = getFolderImages(Properties.Settings.Default.TargetImagesFolder);
            fillImageList(SourcesFilmStrip, sourceImages);
            fillImageList(TargetsFilmStrip, targetImages);
            m_Factory = new AlgoFactory();

            m_currPCAalgo           = new CPCA();
            m_currHausdorffalgo     = new CHausdorffDistance();
            m_currShapeContextalgo  = new CShapeContext();
            m_currPipedalgo         = new CComboAlgorithm();

            propertyGrid1.SelectedObject = m_currPCAalgo;
            propertyGrid2.SelectedObject = m_currHausdorffalgo;
            propertyGrid3.SelectedObject = m_currShapeContextalgo;
            propertyGrid4.SelectedObject = m_currPipedalgo;
        }

        private void fillImageList(FilmstripControl i_filmstripControl, List<Image> i_Images)
        {
            foreach (Image currImage in i_Images)
            {
                FilmstripImage currFSimage = new FilmstripImage(currImage, (string)currImage.Tag);
                i_filmstripControl.AddImage(currFSimage);
            }
        }

        private List<Image> getFolderImages(string i_Path)
        {
            List<Image> retImages = new List<Image>();
            DirectoryInfo imagesDir = new DirectoryInfo(i_Path);
            string[] fileTypes = Properties.Settings.Default.KnownImageTypes.Split('|');

            foreach (string fileType in fileTypes)
            {
                FileInfo[] directoryFile = imagesDir.GetFiles(fileType);
                
                foreach (FileInfo fileinf in directoryFile)
                {
                    Image currImage = Image.FromFile(fileinf.FullName);
                    currImage.Tag = fileinf.FullName;
                    if (currImage != null)
                    {
                        retImages.Add(currImage);
                    }
                }
            }


            return retImages;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Invoke(new AlgoInvoker(CalculatePCA));
        }

        private void CalculatePCA()
        {
            m_CurrAlgorithmAlias = AlgoFactory.PCA;
            m_CurrMatchingAlgo = m_currPCAalgo;
            try
            {
                m_CurrMatchingAlgo.Create(SourcesFilmStrip.SelectedImage, TargetsFilmStrip.SelectedImage);
                m_CurrAlgoResult = m_CurrMatchingAlgo.Run();
                ResultPictureBox.Image = m_CurrAlgoResult.ResultImage;
                m_CurrTargetAlias = SourcesFilmStrip.SelectedImage.Tag as string;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception was thrown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            propertyGrid1.Refresh();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Invoke(new AlgoInvoker(CalculateHausdorff));
        }

        private void CalculateHausdorff()
        {
            m_CurrAlgorithmAlias = AlgoFactory.Hausdorff;
            m_CurrMatchingAlgo = m_currHausdorffalgo;
            try
            {
                m_CurrMatchingAlgo.Create(SourcesFilmStrip.SelectedImage, TargetsFilmStrip.SelectedImage);
                m_CurrAlgoResult = m_CurrMatchingAlgo.Run();
                ResultPictureBox.Image = m_CurrAlgoResult.ResultImage;
                m_CurrTargetAlias = SourcesFilmStrip.SelectedImage.Tag as string;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception was thrown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            propertyGrid2.Refresh();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Invoke(new AlgoInvoker(CalculateShapeContext));
        }

        private void CalculateShapeContext()
        {
            m_CurrAlgorithmAlias = AlgoFactory.ShapeContext;
            m_CurrMatchingAlgo = m_currShapeContextalgo;
            try
            {
                m_CurrMatchingAlgo.Create(SourcesFilmStrip.SelectedImage, TargetsFilmStrip.SelectedImage);
                m_CurrAlgoResult = m_CurrMatchingAlgo.Run();
                ResultPictureBox.Image = m_CurrAlgoResult.ResultImage;
                m_CurrTargetAlias = SourcesFilmStrip.SelectedImage.Tag as string;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception was thrown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            propertyGrid3.Refresh();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Invoke(new AlgoInvoker(CalculateCombo));
        }

        private void CalculateCombo()
        {
            m_CurrAlgorithmAlias = AlgoFactory.Combined;
            m_CurrMatchingAlgo = m_currPipedalgo;
            try
            {
                m_CurrMatchingAlgo.Create(SourcesFilmStrip.SelectedImage, TargetsFilmStrip.SelectedImage);
                m_CurrAlgoResult = m_CurrMatchingAlgo.Run();
                ResultPictureBox.Image = m_CurrAlgoResult.ResultImage;
                m_CurrTargetAlias = SourcesFilmStrip.SelectedImage.Tag as string;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception was thrown", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            propertyGrid4.Refresh();
        }

        private void ResultPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                PictureBox currPicBox = (PictureBox)sender;

                if (currPicBox.Image != null)
                {
                    Color currColor = m_CurrAlgoResult.TargetColor;
                    m_CurrAlgoResult.TargetColor = Color.Black;

                    currPicBox.DoDragDrop(m_CurrAlgoResult.TargetImage, DragDropEffects.Copy);

                    m_CurrAlgoResult.TargetColor = currColor;
                }                
            }
        }

        private void TargetsFilmStrip_DragDrop(object sender, DragEventArgs e)
        {
            Bitmap droppedImage = (Bitmap)e.Data.GetData(DataFormats.Bitmap);

            for (int i = 0; i < droppedImage.Width; i++)
            {
                for (int j = 0; j < droppedImage.Height; j++)
                {
                    Color currPixel = droppedImage.GetPixel(i, j);
                    if (currPixel.Name == "0")
                    {
                        droppedImage.SetPixel(i, j, Color.White);
                    }
                }
            }

            ((FilmstripControl)sender).AddImage(droppedImage, "Result of " + m_CurrAlgorithmAlias + " on " + m_CurrTargetAlias);
        }

        private void TargetsFilmStrip_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Bitmap))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
    }
}
