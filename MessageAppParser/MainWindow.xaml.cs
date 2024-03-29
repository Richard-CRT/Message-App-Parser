using MessageAppParser.Apps.Instagram;
using Microsoft.Win32;
using ScottPlot;
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

            IEnumerable<MessageBatch> skippedMessageBatches = conversation.MessageBatches.Skip(5); // Must be at least 1

            foreach (Participant participant in conversation.Participants)
            {
                ScottPlot.WPF.WpfPlot wpfPlot = new();
                wpfPlot.Height = 200;
                IEnumerable<MessageBatch> filteredMessageBatches1 = skippedMessageBatches.Where(mB => mB.SenderParticipant == participant);
                double[] data1X = filteredMessageBatches1.Select(mB => mB.Messages.First().Timestamp.ToOADate()).ToArray();
                double[] data1Y = filteredMessageBatches1.Select(mB => conversation.ResponseTimeBeforeMessageBatches[mB].TotalHours).ToArray();
                wpfPlot.Plot.Add.Scatter(data1X, data1Y);
                wpfPlot.Plot.Axes.DateTimeTicksBottom();
                wpfPlot.Plot.Axes.AutoScaleY();
                wpfPlot.Refresh();
                this.stackPanel_Plots.Children.Add(wpfPlot);
            }
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

        #endregion
    }
}
