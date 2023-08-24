﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using SimulatedInput.Library;
using SimulatedInput.Library.Enum;

namespace SimulatedInput
{
    public partial class MainWindow : Window
    {
        private static readonly TimeSpan DelayTime = TimeSpan.FromSeconds(3);

        public MainWindow()
        {
            InitializeComponent();
            SendInputButton.Click += SendInputButtonOnClick;
            SendInputTextBox.PreviewKeyDown += SendInputTextBoxOnPreviewKeyDown;
        }

        private async void SendInputButtonOnClick(object sender, RoutedEventArgs e)
        {
            await SendDelayedInput(DelayTime, new []
            {
                VirtualKeyCode.H, 
                VirtualKeyCode.E, 
                VirtualKeyCode.L, 
                VirtualKeyCode.L, 
                VirtualKeyCode.O
            });
        }

        private async Task SendDelayedInput(TimeSpan delayTime, IEnumerable<VirtualKeyCode> virtualKeyCodes)
        {
            await Task.Delay(delayTime);

            InputSequence sequence = new();
            foreach (VirtualKeyCode keyCode in virtualKeyCodes)
            {
                sequence.Add(KeyboardInputs.KeyDown(keyCode));
                sequence.Add(KeyboardInputs.KeyUp(keyCode));
            }

            InputSender.Send(sequence);
        }
        

        private async void SendInputTextBoxOnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            
            SendInputTextBox.IsEnabled = false;
            await SendDelayedText(DelayTime, SendInputTextBox.Text);
            SendInputTextBox.Clear();
            SendInputTextBox.IsEnabled = true;
        }

        private async Task SendDelayedText(TimeSpan delayTime, string text)
        {
            await Task.Delay(delayTime);
            InputSender.Send(KeyboardInputs.TextToKeyPresses(text));
        }
    }
}