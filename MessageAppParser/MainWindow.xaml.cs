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

            wpfPlot_ResponseTime.Reset();

            wpfPlot_ResponseTime.Plot.Axes.Left.Label.Text = "Hours to reply";
            // The title seems to have a bug that causes the software to crash when you drag it
            //wpfPlot_ResponseTime.Plot.Axes.Title.Label.FontSize = 24;
            //wpfPlot_ResponseTime.Plot.Axes.Title.Label.Padding = 0;

            wpfPlot_ResponseTime.Plot.Legend.IsVisible = true;
            wpfPlot_ResponseTime.Plot.Legend.Location = ScottPlot.Alignment.UpperRight;
            wpfPlot_ResponseTime.Plot.Legend.Orientation = ScottPlot.Orientation.Vertical;
            wpfPlot_ResponseTime.Plot.Legend.AllowMultiline = true;
            wpfPlot_ResponseTime.Plot.Legend.BackgroundFill.Color = ScottPlot.Colors.White.WithOpacity(0.5);
            wpfPlot_ResponseTime.Plot.Legend.ShadowFill.Color = ScottPlot.Colors.Transparent;
            wpfPlot_ResponseTime.Plot.Grid.MajorLineColor = ScottPlot.Colors.Black.WithOpacity(0.2);
            wpfPlot_ResponseTime.Plot.Grid.MinorLineColor = ScottPlot.Colors.Black.WithOpacity(0.1);
            wpfPlot_ResponseTime.Plot.Grid.MinorLineWidth = 1;

            foreach (Participant participant in conversation.Participants)
            {
                IEnumerable<MessageBatch> filteredMessageBatches1 = skippedMessageBatches.Where(mB => mB.SenderParticipant == participant);
                double[] dataX = filteredMessageBatches1.Select(mB => mB.Messages.First().Timestamp.ToOADate()).ToArray();
                double[] dataY = filteredMessageBatches1.Select(mB => conversation.ResponseTimeBeforeMessageBatches[mB].TotalHours).ToArray();

                Scatter scatter = wpfPlot_ResponseTime.Plot.Add.Scatter(dataX, dataY);
                scatter.Label = participant.Name;
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

            Crosshair crosshair = wpfPlot_ResponseTime.Plot.Add.Crosshair(0, 0); // Must be after the whole scaling stuff
            crosshair.IsVisible = false;

            Text text = wpfPlot_ResponseTime.Plot.Add.Text("", 0, 0);
            text.BackColor = ScottPlot.Colors.LightGrey.WithOpacity(0.8);
            text.Label.BorderColor = ScottPlot.Colors.Black;
            text.Label.BorderWidth = 1;
            text.Label.Padding = 5;
            text.Label.OffsetX = -10;
            text.Label.OffsetY = -10;
            text.Label.Alignment = Alignment.LowerRight;
            text.IsVisible = false;

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

        private void button_RescaleHorizontalAxis_Click(object sender, RoutedEventArgs e)
        {
            wpfPlot_ResponseTime.Plot.Axes.AutoScaleX();
            wpfPlot_ResponseTime.Refresh();
        }

        private void button_RescaleBothAxes_Click(object sender, RoutedEventArgs e)
        {
            wpfPlot_ResponseTime.Plot.Axes.AutoScale();
            wpfPlot_ResponseTime.Refresh();
        }

        private void wpfPlot_ResponseTime_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is ScottPlot.WPF.WpfPlot wpfPlot)
            {
                Crosshair? crosshair = (Crosshair?)wpfPlot.Plot.PlottableList.FirstOrDefault(p => p is Crosshair, null);
                Text? text = (Text?)wpfPlot.Plot.PlottableList.FirstOrDefault(p => p is Text, null);

                if (crosshair is not null && text is not null)
                {
                    // determine where the mouse is and get the nearest point
                    Point mousePoint = e.GetPosition(wpfPlot);
                    Pixel mousePixel = new(mousePoint.X, mousePoint.Y);
                    Coordinates mouseLocation = wpfPlot.Plot.GetCoordinates(mousePixel);

                    IEnumerable<DataPoint> nearestPoints = wpfPlot.Plot.PlottableList.Where(p => p is Scatter).Select(p => (p as Scatter)!.Data.GetNearest(mouseLocation, wpfPlot.Plot.LastRender));
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
                        text.IsVisible = true;
                        text.Location = nearestRealPoint.Coordinates;

                        int totalSeconds = (int)Math.Round(nearestRealPoint.Y * 60 * 60);
                        int remainderSeconds = totalSeconds;

                        int hours = totalSeconds / (60 * 60);
                        remainderSeconds = totalSeconds % (60 * 60);
                        int minutes = remainderSeconds / 60;
                        remainderSeconds = remainderSeconds % 60;
                        int seconds = remainderSeconds;

                        text.LabelText = $"{nearestRealPoint.X.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss")}, {hours:00.}:{minutes:00.}:{seconds:00.}";
                        wpfPlot.Refresh();
                    }
                    else if (crosshair.IsVisible)
                    {
                        crosshair.IsVisible = false;
                        text.IsVisible = false;
                        wpfPlot.Refresh();
                        //Text = $"No point selected";
                    }
                }
            }
        }

        private void wpfPlot_ResponseTime_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        #endregion
    }
}
