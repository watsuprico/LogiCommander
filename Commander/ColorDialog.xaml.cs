using System;
using System.Collections.Generic;
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
    /// Interaction logic for ColorDialog.xaml
    /// </summary>
    public partial class ColorDialog : Window {
        public Color curColor;
        public Color prevColor;

        public ColorDialog() {
            InitializeComponent();
        }

        public Color GetColor(Color color) {
            prevColor = curColor = color;
            recCurColor.Fill = new SolidColorBrush(color);
            recPrevColor.Fill = new SolidColorBrush(color);
            ShowDialog();
            return curColor;
        }
        public Color GetColor() {
            return GetColor(Colors.Transparent);
        }

        private void Red_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            curColor.R = (byte)sldRed.Value;
            recCurColor.Fill = new SolidColorBrush(curColor);
        }
        private void Green_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            curColor.G = (byte)sldGreen.Value;
            recCurColor.Fill = new SolidColorBrush(curColor);
        }
        private void Blue_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            curColor.B = (byte)sldBlue.Value;
            recCurColor.Fill = new SolidColorBrush(curColor);
        }
        private void Alpha_Changed(object sender, RoutedPropertyChangedEventArgs<double> e) {
            curColor.A = (byte)sldAlpha.Value;
            recCurColor.Fill = new SolidColorBrush(curColor);
        }

        private void Select_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
