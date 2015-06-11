﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace WedChecker.UserControls.Tasks
{
    public abstract partial class BaseTaskControl : UserControl
    {
        public BaseTaskControl()
        {
            this.InitializeComponent();
        }

        public virtual string TaskName
        {
            get;
            set;
        }
        
        public virtual void DisplayValues()
        {

        }

        public virtual void EditValues()
        {

        }

        public virtual void Serialize(BinaryWriter writer)
        {

        }

        public virtual void Deserialize(BinaryReader reader)
        {
        }

        public virtual async Task SubmitValues()
        {
        }
    }
}
