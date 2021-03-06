﻿using System.IO;
using System.Text;
using System.Threading.Tasks;
using WedChecker.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks.Plannings
{
    public sealed partial class MusicLayout : BaseTaskControl
    {
        private string PlannedLayout { get; set; } = string.Empty;

        public override string TaskName
        {
            get
            {
                return "Music layout";
            }
        }

        public override string EditHeader
        {
            get
            {
                return "Here you can save the notes about music layout, planned for the wedding";
            }
        }

        public static new string DisplayHeader
        {
            get
            {
                return "This is the music layout you have planned";
            }
        }

        public override string TaskCode
        {
            get
            {
                return TaskData.Tasks.MusicLayout.ToString();
            }
        }

        public MusicLayout()
        {
            this.InitializeComponent();
        }

        public MusicLayout(string value)
        {
            this.InitializeComponent();
            PlannedLayout = value;
        }

        public override void DisplayValues()
        {
            tbMusicLayoutDisplay.Text = PlannedLayout;
            displayPanel.Visibility = Visibility.Visible;
            tbMusicLayout.Visibility = Visibility.Collapsed;
        }

        public override void EditValues()
        {
            tbMusicLayout.Text = tbMusicLayoutDisplay.Text;
            tbMusicLayout.Visibility = Visibility.Visible;
            displayPanel.Visibility = Visibility.Collapsed;
        }


        public override void Serialize(BinaryWriter writer)
        {
            var decoration = tbMusicLayout.Text;
            PlannedLayout = decoration;

            writer.Write(PlannedLayout);
        }

        public override async Task Deserialize(BinaryReader reader)
        {
            PlannedLayout = reader.ReadString();
        }

        private void tbMusicLayout_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbMusicLayoutDisplay.Text = tbMusicLayout.Text;
        }

        protected override void LoadTaskDataAsText(StringBuilder sb)
        {
            sb.AppendLine(tbMusicLayoutDisplay.Text);
        }
    }
}
