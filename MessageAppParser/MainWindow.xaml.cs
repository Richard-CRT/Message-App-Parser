using MessageAppParser.Apps.Instagram;
using Microsoft.Win32;
using ScottPlot;
using ScottPlot.AxisRules;
using ScottPlot.Plottables;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MessageAppParser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string? _filePathToLoadOnWindowLoaded = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string filePathToLoad) : this()
        {
            _filePathToLoadOnWindowLoaded = filePathToLoad;
        }

        private void loadConversation(string filePathToLoad)
        {
            InstagramConversation instagramConversation = InstagramConversation.FromFile(filePathToLoad);
            Conversation conversation = instagramConversation.ToGenericConversation();

            conversation.AnalyseMessageBatches();
            conversation.AnalyseTimesBetweenMessageBatches();

            Debug.Assert(conversation.MessageBatches is not null);
            Debug.Assert(conversation.ResponseTimeBeforeMessageBatches is not null);

            IEnumerable<MessageBatch> skippedMessageBatches = conversation.MessageBatches.Skip(1); // Must be at least 1

            wpfPlot_ResponseTime.Plot.Axes.Left.Label.Text = "Hours to reply";
            wpfPlot_ResponseTime.Plot.Title("Response time");
            foreach (Participant participant in conversation.Participants)
            {
                IEnumerable<MessageBatch> filteredMessageBatches1 = skippedMessageBatches.Where(mB => mB.SenderParticipant == participant);
                double[] dataX = filteredMessageBatches1.Select(mB => mB.Messages.First().Timestamp.ToOADate()).ToArray();
                double[] dataY = filteredMessageBatches1.Select(mB => conversation.ResponseTimeBeforeMessageBatches[mB].TotalHours).ToArray();

                wpfPlot_ResponseTime.Plot.Add.Scatter(dataX, dataY);
            }
            wpfPlot_ResponseTime.Plot.Axes.DateTimeTicksBottom(); // Must be before the autoscale

            wpfPlot_ResponseTime.Plot.Axes.AutoScaleX();
            wpfPlot_ResponseTime.Plot.Axes.AutoScaleY();
            wpfPlot_ResponseTime.Plot.Axes.Rules.Add(new MaximumBoundary(
                xAxis: wpfPlot_ResponseTime.Plot.Axes.Bottom,
                yAxis: wpfPlot_ResponseTime.Plot.Axes.Left,
                wpfPlot_ResponseTime.Plot.Axes.GetLimits(
                    xAxis: wpfPlot_ResponseTime.Plot.Axes.Bottom,
                    yAxis: wpfPlot_ResponseTime.Plot.Axes.Left)));

            wpfPlot_ResponseTime.Plot.Add.Crosshair(0, 0); // Must be after the whole scaling stuff

            wpfPlot_ResponseTime.MouseMove += wpfPlot_ResponseTime_MouseMove;

            wpfPlot_ResponseTime.Refresh();
        }

        #region Events

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_filePathToLoadOnWindowLoaded is not null)
            {
                loadConversation(_filePathToLoadOnWindowLoaded!);
            }
        }

        private void button_LoadConversation_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Conversation (*.json)|*.json|All files (*.*)|*.*";
            dialog.DefaultExt = "json";
            dialog.Multiselect = false;
            bool? result = dialog.ShowDialog();
            if (result is not null && result.Value)
            {
                loadConversation(dialog.FileName);
            }
        }

        private void button_RescaleVerticalAxis_Click(object sender, RoutedEventArgs e)
        {
            wpfPlot_ResponseTime.Plot.Axes.AutoScaleY();
            wpfPlot_ResponseTime.Refresh();
        }

        private void wpfPlot_ResponseTime_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ScottPlot.WPF.WpfPlot wpfPlot)
            {
                Crosshair crosshair = wpfPlot.Plot.PlottableList.First(p => p is Crosshair) as Crosshair;

                // determine where the mouse is and get the nearest point
                Point mousePoint = e.GetPosition(wpfPlot);
                Pixel mousePixel = new(mousePoint.X, mousePoint.Y);
                Coordinates mouseLocation = wpfPlot.Plot.GetCoordinates(mousePixel);

                IEnumerable<DataPoint> nearestPoints = wpfPlot.Plot.PlottableList.Where(p => p is Scatter).Select(p => (p as Scatter).Data.GetNearest(mouseLocation, wpfPlot.Plot.LastRender));
                IEnumerable<DataPoint> nearestRealPoints = nearestPoints.Where(nP => nP.IsReal);

                if (nearestRealPoints.Count() > 0)
                {
                    DataPoint nearestRealPoint = nearestRealPoints.MinBy(nRP =>
                    {
                        double xDiff = Math.Abs(mouseLocation.X - nRP.X);
                        double yDiff = Math.Abs(mouseLocation.Y - nRP.Y);
                        double distanceSqr = (xDiff * xDiff) + (yDiff * yDiff);
                        return distanceSqr;
                    });
                    crosshair.IsVisible = true;
                    crosshair.Position = nearestRealPoint.Coordinates;
                    wpfPlot.Refresh();
                    //Text = $"Selected Index={nearest.Index}, X={nearest.X:0.##}, Y={nearest.Y:0.##}";
                }
                else if (crosshair.IsVisible)
                {
                    crosshair.IsVisible = false;
                    wpfPlot.Refresh();
                    //Text = $"No point selected";
                }
            }
        }

        #endregion
    }
}
