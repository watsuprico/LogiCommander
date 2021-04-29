﻿using LogiGraphics;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LogiCommand {
    /// <summary>
    /// Interaction logic for Editor.xaml
    /// </summary>
    public partial class Editor : Window {
        public LogiGraphics.Image Image;

        public Editor() {
            InitializeComponent();
            PEditor.Updated += PushImage;
        }

        private void Pen_Click(object sender, RoutedEventArgs e) {
            PEditor.tool = Tool.PEN;
            PEditor.Cursor = PEditor.tool.GetCursor();
        }

        private void Eraser_Click(object sender, RoutedEventArgs e) {
            PEditor.tool = Tool.ERASER;
            PEditor.Cursor = PEditor.tool.GetCursor();
        }

        private void Bucket_Click(object sender, RoutedEventArgs e) {
            PEditor.tool = Tool.BUCKET;
            PEditor.Cursor = PEditor.tool.GetCursor();
        }

        private void PushImage(CanvasUpdated e) {
            Image = e.image;

            Image.Draw();
        }




        private void Load_Click(object sender, RoutedEventArgs e) {
            // First find the file
            OpenFileDialog openFileDialog = new OpenFileDialog {
                Title = "Select a LogiCommandPicture",
                Filter = "LogiCommandImage (*.lci)|*.lci|All files (*.*)|*.*",
                InitialDirectory = Environment.CurrentDirectory
            };

            if (openFileDialog.ShowDialog() == true) {
                string splash = File.ReadAllText(openFileDialog.FileName);
                PEditor.surface.image.Clear();
                PEditor.surface.image = new LogiGraphics.Image(PEditor.display, splash); // This will update the image
                PEditor.surface.UpdateCanvas();
                PEditor.surface.image.Draw();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            SaveFileDialog saveFileDialog = new SaveFileDialog {
                Title = "Save a LogiCommandPicture",
                Filter = "LogiCommandImage (LCI) (*.lci)|*.lci|Raw byte LCI (*.blci)|*.blci|All files (*.*)|*.*",
                InitialDirectory = Environment.CurrentDirectory
            };

            if (saveFileDialog.ShowDialog() == true) {
                switch (System.IO.Path.GetExtension(saveFileDialog.FileName)) {
                    case ".blci":
                        // Raw byte file
                        File.WriteAllBytesAsync(saveFileDialog.FileName, PEditor.surface.image.GetMatrixComposite());
                        break;
                    case ".rlcp":
                        // Raw LCP file (point strings)
                        //File.WriteAllTextAsync(saveFileDialog.FileName, PEditor.surface.image.ToString(false, true));
                        break;

                    case ".mlcp":
                        // Mono file
                        //File.WriteAllTextAsync(saveFileDialog.FileName, PEditor.surface.image.ToString(true));
                        break;

                    case ".clcp":
                        // Compressed file (using 'commands')
                        // yeah we don't do this yet.
                        //File.WriteAllTextAsync(saveFileDialog.FileName, PEditor.surface.image.ToString());
                        break;

                    default:
                        // Regular file
                        File.WriteAllTextAsync(saveFileDialog.FileName, PEditor.surface.image.ToString());
                        break;

                }
            }

        }

        private void Clear_Click(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Are you sure you want to clear the image? There's no going back.", "Clear image?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.Yes) {
                PEditor.surface.image.Clear();
                PEditor.surface.UpdateCanvas();
            }
        }

        private void ColorChange_Click(object sender, RoutedEventArgs e) {
            ColorDialog colorDialog = new ColorDialog();

            PEditor.toolColor = colorDialog.GetColor();
            btnColor.Background = new SolidColorBrush(PEditor.toolColor);
        }
    }
}
